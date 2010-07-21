using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandoraMusicBox.Engine.Data {
    public class PandoraUser: PandoraData {
        public string UserName {
            get;
            set;
        }

        public string Password {
            get;
            set;
        }

    }
}
