using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM.Initiator.Classes
{
    public static class TextRoutine
    {
        private static readonly Random Rnd;
        public static char[] Chars1 = new[] { 
            'q', 'w', 'r', 't', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 
            'v', 'b', 'n', 'm'
         };
        public static char[] Chars2 = new[] { 'e', 'y', 'u', 'i', 'o', 'a' };

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static TextRoutine()
        {
            Rnd = new Random(DateTime.Now.Millisecond);
        }


        public static string GetVariousWord()
        {
            int length = Rnd.Next(5, 12);
            return GetVariousWord(length);
        }

        public static string GetVariousWord(int length)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                if ((i % 2) == 0)
                {
                    sb.Append(Chars1[Rnd.Next(0, Chars1.Length)]);
                }
                else
                {
                    sb.Append(Chars2[Rnd.Next(0, Chars2.Length)]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates random numbers to mask.
        /// Example: 'AB###' or '109'
        /// </summary>
        /// <param name="mask">use symbol '#' or set maximumn random number</param>
        /// <returns>string representation of a number</returns>
        public static string GetMaskNumber(string mask)
        {
            if (mask.Contains("#"))
            {
                var sb = new StringBuilder(mask.Length);
                foreach (char mchar in mask)
                {
                    sb.Append(mchar == '#' ? Rnd.Next(10).ToString()[0] : mchar);
                }
                return sb.ToString();
            }
            // parse maximum random number
            return Rnd.Next(1, int.Parse(mask)).ToString();
        }

    }
}
