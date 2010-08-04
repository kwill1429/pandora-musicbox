﻿using System;
using System.Collections.Generic;
using System.Text;
using PandoraMusicBox.Engine.Encryption;
using System.Xml;
using System.Text.RegularExpressions;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraSong: PandoraData {
        private static BlowfishCipher decrypter = new BlowfishCipher(PandoraCryptKeys.In);

        public string MusicId {
            get;
            internal set;
        }

        public string Artist {
            get;
            internal set;
        }

        public string Album {
            get;
            internal set;
        }

        public string Title {
            get;
            internal set;
        }

        public string AudioURL {
            get;
            internal set;
        }

        public string ArtworkURL {
            get;
            internal set;
        }

        public PandoraRating Rating {
            get;
            internal set;
        }

        public bool TemporarilyBanned {
            get;
            internal set;
        }

        internal PandoraSong(Dictionary<string, string> variables) {
            this.Variables = variables;
        }

        internal static List<PandoraSong> Parse(string xmlStr) {
            List<PandoraSong> songs = new List<PandoraSong>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);

            // loop through each song in the XML document
            foreach (XmlNode currSongNode in xml.SelectNodes("//methodResponse/params/param/value/array/data/value/struct")) {
                Dictionary<string, string> variables = GetVariables(currSongNode);
                PandoraSong song = new PandoraSong(variables);

                song.MusicId = song["musicId"];
                song.Artist = song["artistSummary"];
                song.Album = song["albumTitle"];
                song.Title = song["songTitle"];
                song.AudioURL = DecodeUrl(song["audioURL"]);
                song.ArtworkURL = song["artistArtUrl"];
                if (song["rating"] == "1") song.Rating = PandoraRating.Love;
                else song.Rating = PandoraRating.Unrated;

                songs.Add(song);
            }

            return songs;
        }

        internal static PandoraSong ParseAdvertisement(string xmlStr) {
            // fix broken ass xml response and parse out variables. thanks for the malformed xml doubleclick!
            Regex pattern = new Regex("<!--.*-->");
            string cleanReply = pattern.Replace(xmlStr, "").Replace("&", "&amp;").Trim();
            Dictionary<string, string> variables = GetVariables(cleanReply);

            PandoraSong ad = new PandoraSong(variables);
            ad.Artist = "Advertisement";
            ad.Album = "Advertisement";
            ad.Title = "Advertisement";

            ad.AudioURL = ad["audio"];
            ad.ArtworkURL = "http://pandora.com" + ad["image"];

            return ad;
        }

        private static string DecodeUrl(string input) {
            int encryptedLength = 48;
            string encryptedStr = input.Substring(input.Length - encryptedLength);
            return input.Substring(0, input.Length - encryptedLength) + decrypter.Decrypt(encryptedStr).Trim(new char[] {'\b'});
        }
    }
}
