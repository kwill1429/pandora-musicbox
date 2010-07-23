using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine.Encryption {
    public partial class PandoraCryptKeys {
        public static BlowfishKey In;
        public static BlowfishKey Out;

        static PandoraCryptKeys() {
            initializeInKey();
            initializeOutKey();
        }
    }
}
