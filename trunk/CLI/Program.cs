using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Encryption;
using PandoraMusicBox.Engine;

namespace CLI {
    class Program {
        static void Main(string[] args) {
            MusicBoxCore musicBox = new MusicBoxCore();

            string user = "your user name";
            string pw = "your password";
            
            if (musicBox.AuthenticateListener(user, pw)) {
                Console.WriteLine("successfully logged in as " + user);
            } else {
                Console.WriteLine("invalid username or password.");
            }

            Console.ReadKey();
        }
    }
}
