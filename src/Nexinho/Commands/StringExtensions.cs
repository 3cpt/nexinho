using System;
using System.Text;

namespace Nexinho.Commands
{
    public static class StringExtensions
    {
        public static string Mask(this string word)
        {
            var rand = new Random();
            var num = rand.Next(0, word.Length - 1);
            var sb = new StringBuilder();

            for (int i = 0; i < word.Length; i++)
            {
                if (i == num)
                {
                    sb.Append(word[num]);
                }
                else
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }

        public static string Remask(this string mask, string word)
        {
            var rand = new Random();

            int num = -1;

            while (num == -1 || mask[num] != '-')
            {
                num = rand.Next(0, mask.Length);
            }

            var sb = new StringBuilder();

            for (int i = 0; i < mask.Length; i++)
            {
                if (i == num)
                {
                    sb.Append(word[i]);
                }
                else if (mask[i] == word[i])
                {
                    sb.Append(word[i]);
                }
                else
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }
    }
}