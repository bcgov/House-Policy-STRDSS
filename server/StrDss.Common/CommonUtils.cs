using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StrDss.Common
{
    public static class CommonUtils
    {
        public static string GetFullName(string firstName, string lastName)
        {
            return string.IsNullOrEmpty(lastName)
                ? $"{firstName?.Trim() ?? string.Empty}"
                : $"{lastName.Trim()}, {firstName?.Trim() ?? string.Empty}";
        }

        public static T CloneObject<T>(T obj)
        {
            T newObj = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(newObj, property.GetValue(obj));
                }
            }

            return newObj;
        }

        public static bool IsTextFile(string contentType)
        {
            return contentType == "text/plain" ||
                   contentType == "text/csv" ;
        }

        public static string StreamToBase64(Stream stream)
        {
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static short? StringToShort(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (short.TryParse(str, out short result))
            {
                return result;
            }

            return null;
        }
        public static byte[] CreateZip(string csvContent, string fileName)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = archive.CreateEntry($"{fileName}.csv");

                using var entryStream = demoFile.Open();
                using var streamWriter = new StreamWriter(entryStream);
                streamWriter.Write(csvContent);
            }

            return memoryStream.ToArray();
        }

        public static string SanitizeAndUppercaseString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            // Remove leading zeroes
            input = Regex.Replace(input, @"^0+", "");

            // Remove non-alphanumeric characters and convert to uppercase
            return Regex.Replace(input, @"[^a-zA-Z0-9]", "").ToUpper();
        }

        public static bool IsValidEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var valid = Regex.IsMatch(email, RegexDefs.GetRegexInfo(RegexDefs.Email).Regex);

            if (!valid) return false;

            if (email.StartsWith(".") || email.EndsWith(".") || email.Contains(".@") || email.Contains("@.") || email.Contains("..")) return false;

            return true;
        }
    }
}
