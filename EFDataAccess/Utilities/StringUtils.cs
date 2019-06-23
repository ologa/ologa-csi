using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public class StringUtils
    {
        public static string MaskIfConfIsEnabled(string input)
        {
            //take first 2 characters
            string firstPart = input.Substring(0, 1);

            //take last 24 characters
            int len = input.Length;
            string lastPart = input.Substring(len - 1, 1);

            //take the middle part (****)
            int middlePartLenght = len - (firstPart.Length + lastPart.Length);
            middlePartLenght = (middlePartLenght < 0) ? 0 : middlePartLenght;
            string middlePart = new String('*', middlePartLenght);

            return System.Configuration.ConfigurationManager.AppSettings["AppVersion"].Contains("cloud") ? firstPart + middlePart + lastPart : input;
        }

        public static string RemoveNewLine(string value)
        {
            return Regex.Replace(value, @"\t|\n|\r", "");
        }
    }
}
