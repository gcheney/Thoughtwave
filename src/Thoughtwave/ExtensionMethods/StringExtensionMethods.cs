using System;
using System.Text;

namespace Thoughtwave.ExtensionMethods
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Takes a string and returns the same string with its first letter capitalized 
        /// </summary>
        /// <param name="str"> The string to capitalize</param>
        /// <returns>
        /// The capitalized version of the provided string
        /// </returns>
        public static string Capitalize(this String str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }

        /// <summary>
        /// Takes a string and returns the truncated version of the string 
        /// </summary>
        /// <param name="str"> The string to capitalize</param>
        /// <param name="maxLength"> The maximum length for the truncated string</param>
        /// <returns>
        /// The truncacted string
        /// </returns>
        public static string TruncateString(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        /// <summary>
        /// Takes a string and returns the string with all whitespace removed
        /// </summary>
        /// <param name="str"> The string to remove whitespace from</param>
        /// <returns>
        /// The string without whitespace
        /// </returns>
        public static string RemoveWhiteSpaces(this string str)
        {
            if (str == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!Char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a bool result indicating if the provided string is present
        /// </summary>
        /// <param name="str"> The string to be checked</param>
        /// <param name="toCheck">The string that is being checked for</param>
        /// <param name="ignoreCase">Wether or not to ignore the case of the strings</param>
        /// <returns>
        /// true if the toCheck parameter is present in this string, false otherwise
        /// </returns>
        public static bool Contains(this string str, string toCheck, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return str.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return str.IndexOf(toCheck) >= 0;
        }
    }   
}