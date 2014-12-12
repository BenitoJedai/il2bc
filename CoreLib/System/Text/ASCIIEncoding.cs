﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Apache License 2.0 (Apache)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace System.Text
{
    using System;
    public class ASCIIEncoding : Encoding
    {
        public ASCIIEncoding()
        {
        }

        public override byte[] GetBytes(String s)
        {
            var chars = s.ToCharArray();

            var bytes = new byte[chars.Length];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)chars[i];
            }

            return bytes;
        }

        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotImplementedException();
        }


        public override char[] GetChars(byte[] bytes)
        {
            var chars = new char[bytes.Length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }

            return chars;
        }

        public override char[] GetChars(byte[] bytes, int byteIndex, int byteCount)
        {
            var chars = new char[byteCount];
            for (var i = 0; i < byteCount; i++)
            {
                chars[i] = (char)bytes[i + byteIndex];
            }

            return chars;
        }

        public override Decoder GetDecoder()
        {
            throw new NotImplementedException();
        }
    }
}


