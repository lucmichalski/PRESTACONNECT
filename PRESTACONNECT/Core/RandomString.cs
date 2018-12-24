using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core
{
    public static class RandomString
    {
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numeric = "0123456789";
        private const string Special = "&é#'{([-|è_çà@)]°+=}$£µ*%ù!§:;.?";
        private const string SpecialAdvanced = "&é~\"#'{([-|è`_\\ç^à@)]°+=}¨^$£¤µ*%ù!§:/;.?²";

        public static string GetRandomstring(int Length, bool UseLower, bool UseUpper, bool UseNumeric, bool UseSpecial)
        {
            string Result = "hy7L1Pf6Q4";
            try
            {
                Result = GenerateRandom(Length, GetBase(UseLower, UseUpper, UseNumeric, UseSpecial));
            }
            catch (Exception)
            {
                // Not implemented
            }
            return Result;
        }

        private static string GenerateRandom(int Length, char[] Chars)
        {
            string Password = string.Empty;
            System.Random rnd = new System.Random();
            for (int i = 0; i < Length; i++)
                Password += Chars[rnd.Next(Chars.Length)];
            return Password;
        }

        private static char[] GetBase(bool UseLower, bool UseUpper, bool UseNumeric, bool UseSpecial)
        {
            string temp = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string Base = "";
            if (UseNumeric)
            {
                Base += Numeric;
            }
            if (UseLower)
            {
                Base += Lower;
            }
            if (UseLower && UseUpper && UseNumeric)
            {
                Base += Numeric;
            }
            if (UseUpper)
            {
                Base += Upper;
            }
            if ((UseNumeric || UseLower || UseUpper) && UseSpecial)
            {
                Base += Special;
            }
            if (Base == string.Empty)
            {
                Base = temp;
            }
            //return Shuffle(Base).ToCharArray();
            return Base.ToCharArray();
        }

        private static string Shuffle(string Base)
        {
            string Result = string.Empty;
            Random r = new Random();
            do
            {
                int index = r.Next(Base.Length);// tire l'index d'un caractère de la chaine

                Result += Base[index];//mets ce caractère dans le mélange

                Base = Base.Remove(index, 1);//enlève le caractère de la chaine d'origine pour ne pas les réutiliser

            } while (Base.Length > 0);

            return Result;
        }

        // <JG> 27/02/2013 déplacement méthode
        public static string HashMD5(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
