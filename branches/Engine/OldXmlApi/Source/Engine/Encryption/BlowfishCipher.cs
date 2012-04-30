using System;
using System.Collections.Generic;
using System.Text;

namespace PandoraMusicBox.Engine.Encryption {
    public class BlowfishKey {
        public int N;
        public uint[] P;
        public uint[,] S;
    }

    public class BlowfishCipher {
        BlowfishKey key;

        public BlowfishCipher(BlowfishKey key) {
            this.key = key;
        }

        /// <summary>
        /// Takes a string, encrypts it and outputs the byte stream in hexadecimal. Assumes UTF-8
        /// encoded input string.
        /// </summary>
        /// <param name="input">string to be encrypted.</param>
        /// <returns></returns>
        public string Encrypt(string input) {
            string paddedInput = input;            
            
            // if required, pad the string to ensure we have 64bit blocks for the encrypter
            int paddingRequired = input.Length % 8 == 0 ? 0 : 8 - (input.Length % 8);
            for (int i = 0; i < paddingRequired; i++)
                paddedInput += '\0';

            // convert input to byte stream and encrypt
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] encryptedBytes = Encrypt(encoder.GetBytes(paddedInput));

            // convert to hex string and return
            string output = "";
            int currPos = 0;
            while (currPos < encryptedBytes.Length) {
                uint block = BitConverter.ToUInt32(encryptedBytes, currPos);
                output += String.Format("{0:x8}", block);                
                currPos += 4;
            }

            return output;
        }

        /// <summary>
        /// Decrypts an encrypted string that has been hex encoded. Using this will decrypt a
        /// strign encrypted with the Encrypt(string) method.
        /// </summary>
        /// <param name="encryptedHexStr"></param>
        /// <returns></returns>
        public string Decrypt(string encryptedHexStr) {
            byte[] encryptedBytes = new byte[encryptedHexStr.Length / 2];

            // convert the string of hex values to a series of unsigned integers
            int currPos = 0;
            while (currPos < encryptedHexStr.Length) {
                string hexStr = encryptedHexStr.Substring(currPos, 8);
                uint intValue = Convert.ToUInt32(hexStr, 16);
                BitConverter.GetBytes(intValue).CopyTo(encryptedBytes, currPos / 2);

                currPos += 8;
            }

            // decrypt the encyrpted bytes and convert them back to text
            ASCIIEncoding encoder = new ASCIIEncoding();
            string output = encoder.GetString(Decrypt(encryptedBytes)).Trim(new char[] {'\0'});

            return output;
        }


        public byte[] Encrypt(byte[] input) {
            if (input.Length % 8 != 0)
                return null;

            byte[] output = new byte[input.Length];

            int currPos = 0;
            byte[] currBlock = new byte[8];
            while (currPos < input.Length) {                
                Array.Copy(input, currPos, currBlock, 0, 8);
                EncryptBlock(currBlock).CopyTo(output, currPos);
                currPos += 8;
            }

            return output;
        }

        public byte[] Decrypt(byte[] input) {
            if (input.Length % 8 != 0)
                return null;

            byte[] output = new byte[input.Length];

            int currPos = 0;
            byte[] currBlock = new byte[8];
            while (currPos < input.Length) {
                Array.Copy(input, currPos, currBlock, 0, 8);
                DecryptBlock(currBlock).CopyTo(output, currPos);

                currPos += 8;
            }

            return output;
        }

        public byte[] EncryptBlock(byte[] block) {
            if (block.Length != 8)
                return null;

            uint tmp;
            uint left = ByteSwap(BitConverter.ToUInt32(block, 0));
            uint right = ByteSwap(BitConverter.ToUInt32(block, 4));

            for (int i = 0; i < key.N; i++) {
                // magic!
                left ^= key.P[i];
                right ^= F(left);

                // exchange left and right
                tmp = left;
                left = right;
                right = tmp;
            }

            // exchange left and right
            tmp = left;
            left = right;
            right = tmp;

            right ^= key.P[key.N];
            left ^= key.P[key.N + 1];

            // all done, return the results
            byte[] encryptedBlock = new byte[8];
            BitConverter.GetBytes(left).CopyTo(encryptedBlock, 0);
            BitConverter.GetBytes(right).CopyTo(encryptedBlock, 4);

            return encryptedBlock;
        }

        public byte[] DecryptBlock(byte[] block) {
            if (block.Length != 8)
                return null;

            uint tmp;
            uint left = BitConverter.ToUInt32(block, 0);
            uint right = BitConverter.ToUInt32(block, 4);

            for (int i = key.N + 1; i > 1; --i) {
                // magic!
                left ^= key.P[i];
                right ^= F(left);

                // exchange left and right
                tmp = left;
                left = right;
                right = tmp;
            }

            tmp = left;
            left = right;
            right = tmp;

            left ^= key.P[0];
            right ^= key.P[1];

            // all done, return the results
            byte[] decryptedBlock = new byte[8];
            BitConverter.GetBytes(ByteSwap(left)).CopyTo(decryptedBlock, 0);
            BitConverter.GetBytes(ByteSwap(right)).CopyTo(decryptedBlock, 4);

            return decryptedBlock;
        }


        private uint F(uint x) {
            byte a = (byte)((x >> 24) & 0x000000ff);
            byte b = (byte)((x >> 16) & 0x000000ff);
            byte c = (byte)((x >> 8) & 0x000000ff);
            byte d = (byte)(x & 0x000000ff);

            uint y = key.S[0, a];
            y += key.S[1, b];
            y ^= key.S[2, c];
            y += key.S[3, d];

            return y;
        }

        private uint ByteSwap(uint x) {
            return ((x >> 24) & 0x000000ff) | ((x >> 8) & 0x0000ff00) | ((x << 8) & 0x00ff0000) | ((x << 24) & 0xff000000);
        }
    }
}
