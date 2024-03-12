using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.Security.Principal;

namespace StrDss.Common
{
    public static class CommonExtensions
    {
        private static readonly Regex numeric = new Regex(@"^\d+(\.\d+)?$");
        private static readonly Regex integer = new Regex(@"^\d+$");

        public static string GetCustomClaim(this IPrincipal user, string claimName)
        {
            return (((ClaimsPrincipal)user).FindFirst(claimName)?.Value ?? "").Trim();
        }

        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static decimal[] ToDecimalArray(this string str)
        {
            if (str == null) return new decimal[] { };

            var result = new List<decimal>();

            try
            {
                string[] tokens = str.Split(',');

                foreach(var token in tokens)
                {
                    var decToken = decimal.Parse(token);

                    if (!result.Contains(decToken))
                        result.Add(decToken);
                }
            }
            catch
            {
                return new decimal[] { };
            }

            return result.ToArray();
        }

        public static string[] ToStringArray(this string str)
        {
            return str == null ? (new string[] { }) : str.Split(',');
        }

        public static string WordToWords(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", x => $"{x.Value[0]} {char.ToUpper(x.Value[1])}");
        }

        public static string RemoveLineBreak(this string str)
        {
            return Regex.Replace(str, @"\r\n?|\n", "");
        }

        public static bool IsInteger(this string str)
        {
            return integer.IsMatch(str);
        }

        public static bool IsNumeric(this string str)
        {
            return numeric.IsMatch(str);
        }

        public static void AddItem(this Dictionary<string, List<string>> dictionary, string keyName, string item)
        {
            if (dictionary.ContainsKey(keyName))
            {
                if (!dictionary[keyName].Contains(item))
                    dictionary[keyName].Add(item);
            }
            else
            {
                dictionary.Add(keyName, new List<string> { item });
            }
        }

        public static string SanitizeFileName(this string fileName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var newFileName = String.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            newFileName = Path.GetFileNameWithoutExtension(newFileName);

            if (newFileName.Length > 100)
            {
                newFileName = newFileName.Substring(0, 100);
            }

            return newFileName;
        }

        public static bool IsCsvFile(this string fileName)
        {
            if (!Path.HasExtension(fileName))
                return false;

            return Path.GetExtension(fileName).ToLowerInvariant() == ".csv";
        }

        /// <summary>
        /// Modified original code from
        /// https://stackoverflow.com/questions/286813/how-do-you-convert-html-to-plain-text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlToPlainText(this string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            const string lineBrakAndtab = @"<(li|LI)>";

            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);
            var lineBrakAndtabRegex = new Regex(lineBrakAndtab, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = WebUtility.HtmlDecode(text);

            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");

            //Add one line for </ul>
            text = text.Replace("<ul>", string.Empty);
            text = text.Replace("</ul>", Environment.NewLine + Environment.NewLine);

            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Replace <li> with line breaks and tab
            text = lineBrakAndtabRegex.Replace(text, Environment.NewLine + "\t");
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public static string[] ToLowercase(this string[] items)
        {
            var lowerCaseItems = new string[items.Length];

            var i = 0;
            foreach (var item in items)
            {
                lowerCaseItems[i] = item.ToLowerInvariant();
                i++;
            }

            return lowerCaseItems;
        }

        public static string RemoveQuestionMark(this string query)
        {
            return query.StartsWith("?") ? query.Substring(1) : query;
        }

        public static string ComputeSHA256Hash(this string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static string GetWordAfterLastDot(this string input)
        {
            if (input.IsEmpty()) return "";

            var lastDotIndex = input.LastIndexOf('.');

            return lastDotIndex >= 0 ? input.Substring(lastDotIndex + 1) : input;
        }
    }
}
