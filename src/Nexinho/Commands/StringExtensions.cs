﻿using System;
using System.Text;

namespace Nexinho.Commands
{
    public static class StringExtensions
    {
        public static string Mask(this string word)
        {
            Random rand = new();
            int num = rand.Next(0, word.Length - 1);
            StringBuilder sb = new();

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
            Random rand = new();

            int num = rand.Next(0, mask.Length - 1);
            StringBuilder sb = new();

            for (int i = 0; i < mask.Length; i++)
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
    }
}