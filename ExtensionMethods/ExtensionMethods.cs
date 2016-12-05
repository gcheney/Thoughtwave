using System;

namespace Thoughtwave.ExtensionMethods
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Takes a string as its input and returns the same string with its first letter capitalized 
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
    }   
}