using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine.Encryption {
    public partial class PandoraCryptKeys {
        public static BlowfishKey In;
        public static BlowfishKey Out;
        public static BlowfishKey PW;

        static PandoraCryptKeys() {
            initializeInKey();
            initializeOutKey();
            initializePasswordKey();
        }
    }
}
