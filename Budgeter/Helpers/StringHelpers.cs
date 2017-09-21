using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Budgeter.Helpers
{
    public class StringHelpers
    {
        public static string CreateDisplayName(string email)
        {

            return email.Remove(email.IndexOf('@'));
        }

        public static bool SearchString(string searchFor, string searchIn)
        {

            if (searchIn.Contains(searchFor))
            {
                return true;
            }
            else return false;
        }

        public static string ShortString(string strIn)
        {
            string shortString;
            if (!String.IsNullOrWhiteSpace(strIn) && strIn.Length > 35)
            {
                if (strIn.IndexOf(" ", 35) > 0)
                {
                    shortString = strIn.Substring(0, strIn.IndexOf(" ", 35)) + "...";
                    return shortString;
                }

                else
                {

                    shortString = strIn.Substring(0, 35);
                    return shortString + "...";
                }
            }
            return strIn;
        }

        public static string URLFriendly(string title)
        {
            if (title == null)
            {
                return "";
            }
            const int MAXLEN = 80;
            int len = title.Length;
            bool prevDash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevDash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)(c | 32));
                    prevDash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevDash = true;
                    }
                }

                else if (c == '#')
                {
                    if (i > 0)
                    {
                        if (title[i - 1] == 'C' || title[i - 1] == 'F')
                        {
                            sb.Append("-sharp");
                        }
                    }
                }

                else if (c == '+')
                {
                    sb.Append("-plus");
                }
                else if ((int)c >= 128)
                {
                    int prevLen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevLen != sb.Length) prevDash = false;
                }
                if (sb.Length == MAXLEN)
                {
                    break;
                }
            }
            if (prevDash)
            {
                return sb.ToString().Substring(0, sb.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }

        }
    }
}
