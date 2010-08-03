#define ALLOW_UNTESTED_INTERFACES

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using PandoraMusicBox.Engine.Data;
using System.Threading;

namespace Engine { 
    public class DirectShowPlayer{

        public delegate void DirectShowEventHandler(EventCode eventCode);
        public event DirectShowEventHandler PlaybackEvent;

        private IGraphBuilder  graphBuilder;
        private IMediaControl  mediaControl;
        private IMediaEventEx  mediaEventEx;
        private IMediaSeeking  mediaSeeking;
        private IMediaPosition mediaPosition;
        private IBasicAudio    basicAudio;

        private object lockingToken = new object();

        Thread eventListenerThread;

        private PandoraSong loadedSong;

        private const int MAX_VOLUME = 0;
        private const int MIN_VOLUME = -10000;
        
        private bool _IsPlaying;
        private double? PreviousVolume;

        // absolute control over volume
        private double RealVolume {
            get {
                if (basicAudio != null) {
                    int dsVol;
                    
                    int hr = 0;
                    hr = basicAudio.get_Volume(out dsVol);
                    DsError.ThrowExceptionForHR(hr);

                    // convert to 0.0 - 1.0 value
                    _Volume = ((double) dsVol - MIN_VOLUME) / (MAX_VOLUME - MIN_VOLUME);
                }

                return _Volume;
            }

            set {
                int dsVol = (int) (MIN_VOLUME + (MAX_VOLUME - MIN_VOLUME) * value);

                // make sure we don't go outside our bounds
                if (dsVol < MIN_VOLUME) dsVol = MIN_VOLUME;
                if (dsVol > MAX_VOLUME) dsVol = MAX_VOLUME;

                if (basicAudio != null) {
                    int hr = 0;
                    hr = basicAudio.put_Volume(dsVol);
                    DsError.ThrowExceptionForHR(hr);
                }

                _Volume = value;
            }
        } protected double _Volume;

        /// <summary>
        /// Adjusts volume using smooth transitions.
        /// </summary>
        public double Volume {
            set { 
                _targetVolume = value;
                if (_targetVolume < 0) _targetVolume = 0;
                if (_targetVolume > 1) _targetVolume = 1;
            }
            get {
                if (_targetVolume == null)
                    return RealVolume;

                return (double)_targetVolume;
            }
        } private double? _targetVolume = null;


        public int Position {
            get {
                double position;
                
                int hr = 0;
                hr = mediaPosition.get_CurrentPosition(out position);
                DsError.ThrowExceptionForHR(hr);
                
                // psoition returned in terms of seconds so multiply by 1000
                // for milliseconds
                return (int) (position * 1000);
            }
            set {
            }
        }

        public int Length {
            get { return 0; }
        }

        /**
         * Constructor for Player Object
         */
        public DirectShowPlayer() {
            graphBuilder = null;
            _IsPlaying = false;
            PreviousVolume = null;
            loadedSong = null;
        }

        /**
         * Destructor 
         */
        ~DirectShowPlayer() {
            Close();
        }

        public bool Open(PandoraSong file) {
            try {
                Close();

                graphBuilder = (IGraphBuilder)new FilterGraph();

                // create interface objects for various actions
                mediaControl = (IMediaControl)graphBuilder;
                mediaEventEx = (IMediaEventEx)graphBuilder;
                mediaSeeking = (IMediaSeeking)graphBuilder;
                mediaPosition = (IMediaPosition)graphBuilder;
                basicAudio = (IBasicAudio)graphBuilder;

                int hr = 0;

                StartEventLoop();

                //hr = graphBuilder.AddFilter(

                // Have the graph builder construct its the appropriate graph automatically
                hr = graphBuilder.RenderFile(file.AudioURL, null);
                DsError.ThrowExceptionForHR(hr);

                // maintain previous volume level so it persists from track to track
                if (PreviousVolume != null)
                    RealVolume = (int)PreviousVolume;

                loadedSong = file;
                return true;
            }
            catch (Exception) {
                return false;
            }

        }

        public void Close() {
            // store current volume so it persists from track to track
            if (loadedSong != null)
                PreviousVolume = RealVolume;

            Stop();

            lock (lockingToken) {
                mediaEventEx = null;
                mediaSeeking = null;
                mediaControl = null;
                mediaPosition = null;
                basicAudio = null;

                if (this.graphBuilder != null)
                    Marshal.ReleaseComObject(this.graphBuilder);
                this.graphBuilder = null;
            }

            loadedSong = null;
            _IsPlaying = false;
        }

        public PandoraSong GetLoadedAudioFile() {
            return loadedSong;
        }

        public void Play() {
            if (graphBuilder == null)
                return;

            int hr = 0;

            // Run the graph to play the media file
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            _IsPlaying = true;
        }

        public void Stop() {
            if (graphBuilder == null)
                return;
            
            mediaControl.Pause();
            _IsPlaying = false;
        }

        public bool IsPlaying() {
            return _IsPlaying;
        }

        private void StepVolume() {
            if (_targetVolume == null)
                return;

            double currVol = RealVolume;
            double diff = Math.Abs(currVol - (double)_targetVolume);

            if (diff <= 0.01) {
                RealVolume = (double)_targetVolume;
                _targetVolume = null;
                return;
            }

            if (currVol > _targetVolume) {
                RealVolume -= 0.01;
            }
            else {
                RealVolume += 0.01;
            }

        }

        private void StartEventLoop() {

            ThreadStart actions = delegate {
                bool done = false;
                while (mediaEventEx != null && !done) {
                    EventCode evCode;
                    IntPtr param1;
                    IntPtr param2;

                    lock (lockingToken)
                        if (mediaEventEx != null)
                            mediaEventEx.GetEvent(out evCode, out param1, out param2, 100);
                        else return;
                     
                    if (evCode.GetHashCode() != 0 && PlaybackEvent != null) 
                        PlaybackEvent(evCode);

                    lock (lockingToken) if (mediaEventEx != null) mediaEventEx.FreeEventParams(evCode, param1, param2);
                    done = (evCode == EventCode.Complete);

                    StepVolume();
                }
            };

            eventListenerThread = new Thread(actions);
            eventListenerThread.Name = "DirectShowPlayer Event Listener";
            eventListenerThread.IsBackground = true;
            eventListenerThread.Start();
        }
    }
}
