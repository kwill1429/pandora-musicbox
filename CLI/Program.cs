using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandoraMusicBox.Engine.Encryption;
using PandoraMusicBox.Engine;

namespace CLI {
    class Program {
        private static void EncryptTest() {
            BlowfishCipher crypter = new BlowfishCipher(PandoraCryptKeys.Out);
            string input = "you silly son of a bitch you have fucking DONE IT!!!";
            string encrypted = crypter.Encrypt(input);
            string decrypted = crypter.Decrypt(encrypted);

            Console.WriteLine("input:\n" + input);
            Console.WriteLine();
            Console.WriteLine("encrypted:\n" + encrypted);
            Console.WriteLine();
            Console.WriteLine("decrypted:\n" + decrypted);
            Console.WriteLine();
        }

        private static void DecryptTest() {
            string input = "9ebed1aacea9295ab1ab523568438752fe64a70592e6346627a06c221c3da2c873bfc2c1cf00a112fb8a9de154383daf1e75029aa23910bb0cd755308e918d34adba0939fe6c4f4252bd1f406fe26eb6ce9003054c4052015667259d5746aeb231ffda321cb78fb82d768f8ca659a6e445a1f35aad9124e3896d21444ab148f59aec3ab8bbd684893c9027cce40dccee841874016a3ba033f0a694b72ac574b7b0f76b02df583856019544d8e2aa75fb740683a327c8b598686a3a7ff29fdb5f013fb60818cf2c3142aa988c7a037e03bff86f2067fef3a77f2c4d19372d8a704c5c530b9c2fbf7ffc004cdbc92aff43c3e4ab7fdf3a7a5258708facd6413efaffc45bb806d4dac65375b90656a30d43b0ad72e919f890bf66148bd6a73ac72cfe23b6a5986a8b25";
            string input2 = "9ebed1aacea9295ab1ab523568438752fe64a70592e6346627a06c221c3da2c873bfc2c1cf00a112fb8a9de154383daf037bc7ac0cc952e4980cece77314beab467fdb2d88bd015fd5efa894a2c5d47be33e8c45732b4bd8364949543d39a3f0eaf1bb72d2ab3f3ab7838bdc8a96a9ba83bbd6365bc0c720ffc45bb806d4dac65375b90656a30d4331ffda321cb78fb87b938e1c4862d5fc93329eb63e2efe08b8686cd26f252023009f7613ca7571f69fd7607e8b0ace8b7482602199a804dd1f40a0f35030191d9b2e267bb449b68f4bb0c01719f48cc2a86d75e4f1b6a28dfbdd40e603359893e1a0963fdd70f84acfd97a22fc62997658e3dc252cb74fa2c2f654017b00b5b2b40f0b12296e0b70cc928a870b8d877ac101f2a824bf211a44234115f30c2c243c9027cce40dccee34f993aa595fac5745d27e56aa582f5349b02632bfca9b07";
            string input3 = "9ebed1aacea9295ab1ab523568438752fe64a70592e6346627a06c221c3da2c873bfc2c1cf00a11235b7eda513f963345097314f05e9359c25b0bc11baacd68166148bd6a73ac72c6b11ed8ca80af39dab21f250ae859d1b95250c2329fecafab0a6575aeca67002a424a72bbeb2b54857147bd8d6881e4542aa988c7a037e03bff86f2067fef3a77f2c4d19372d8a704c5c530b9c2fbf7fe09cdefc4e9875e26a12ac65c58d0d575ef47e71ed08eb6f9b50df688464ca19a957f36b7da1cfda013fb60818cf2c3142aa988c7a037e03bff86f2067fef3a7bc6acacddef299f8088faf907de69ba274bc66c1ac2e3b85";
            string input4 = "9ebed1aacea9295ab1ab523568438752fe64a70592e6346627a06c221c3da2c873bfc2c1cf00a112c99b646c364e895a968e8d3e3047230211c128d3589585e8f43aa3b3a696004b9a05c314c256353307e03ddc04fb3e177482602199a804ddd2e8db6eda13d14b2465f9e01752bbf7e7482eb6b24784f63d1ed34737d28d0e408b84be0d341bbaee314f6f7ec03487f24e57718af2dc846c06648b9c3660ea5ef44ba6354c75be2d2bfbd29d1890f42ef3427132e83efbff2af1d0186c7ce1b02b7164250cd5e73d1ed34737d28d0e408b84be0d341bbaee314f6f7ec03487f24e57718af2dc843a74211ae44b259bc9b859093352eaabd8c2f6e07922810ac4c70a889082568d44234115f30c2c243c9027cce40dccee841874016a3ba033f0a694b72ac574b7b0f76b02df5838567605b1be339a38f9b02b7164250cd5e73d1ed34737d28d0e408b84be0d341bbaee314f6f7ec03487f24e57718af2dc84bda4123ee9bdc0da5aa9a5c535d410fab8686cd26f252023009f7613ca7571f69fd7607e8b0ace8b7482602199a804dd1f40a0f35030191d739b85e437556c89ce503e2351e76c9f1f40215bd56d564e6c9b88b93d138de995250c2329fecafaaf5e73c969731f43b83cdd860d71fd58b17a9e83c7a37647dcd209b813d561b210dbcb0359ea995b4f8e026797a37938364949543d39a3f08ffff799196a4ade0144c669807d42b0b8686cd26f252023009f7613ca7571f69fd7607e8b0ace8b7482602199a804dd1f40a0f35030191d0b5d280bf116f54fb8686cd26f252023009f7613ca7571f6af91da6b4285d21d5667259d5746aeb2b4313db7c2b4088c1d27d835226f73e0";
            BlowfishCipher cipher = new BlowfishCipher(PandoraCryptKeys.Out);
            Console.WriteLine("\n\ndecrypted payload:\n" + cipher.Decrypt(input4));
        }

        static void Main(string[] args) {
            EncryptTest();
            MusicBoxCore musicBox = new MusicBoxCore();
            Console.WriteLine("RouteID: " + musicBox.RouteID);

            //musicBox.AuthenticateListener();

            DecryptTest();
            Console.ReadKey();

        }
    }
}
