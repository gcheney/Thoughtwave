using System;

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
        /// Takes a stringand returns the truncated version of the string 
        /// </summary>
        /// <param name="str"> The string to capitalize</param>
        /// <param name="maxLength"> The maximum length for the truncated string</param>
        /// <returns>
        /// Teh truncacted string
        /// </returns>
        public static string TruncateString(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }
    }   
}