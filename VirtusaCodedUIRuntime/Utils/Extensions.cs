using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsInput;

namespace CodedUI.Virtusa.Utils
{
    static class Extensions
    {

        public static string Substring(this string s, string start, string end)
        {
            int startI = s.IndexOf(start);
            int endI = s.IndexOf(end, startI);
            return s.Substring(startI, endI - startI + 1);
        }

        public static string Substring(this string s, int start, string end)
        {
            int startI = start;
            int endI = s.IndexOf(end, startI) -1;
            return s.Substring(startI, endI - startI + 1);
        }

        public static string Substring(this string s, string start, int end)
        {
            int startI = s.IndexOf(start)+1;
            int endI = end;
            int length = endI - startI;
            return s.Substring(startI, length);
        }
               
        
    }
}
