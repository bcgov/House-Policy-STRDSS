﻿using Ganss.Xss;
using System.Collections;
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
            return contentType == "text/plain"
               || contentType == "text/csv"
               || contentType == "application/csv"
               || contentType == "application/vnd.ms-excel"
               || contentType == "application/x-csv"
               || contentType == "text/x-csv"
               || contentType == "text/tab-separated-values"
               || contentType == "application/octet-stream";
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

        private static readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();

        public static void SanitizeObject<T>(T input)
        {
            if (input == null) return;

            // Handle if the input is a list or collection
            if (input is IEnumerable collection && input.GetType() != typeof(string))
            {
                foreach (var item in collection)
                {
                    if (item != null && item.GetType().IsClass)
                    {
                        SanitizeObject(item);
                    }
                }
                return;
            }

            Type type = input.GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.PropertyType == typeof(string) && property.CanWrite)
                {
                    string? value = property.GetValue(input) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        // Sanitize the string property
                        string sanitizedValue = _sanitizer.Sanitize(value);
                        property.SetValue(input, sanitizedValue);
                    }
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    // Recursively sanitize nested objects
                    object? nestedObject = property.GetValue(input);
                    if (nestedObject != null)
                    {
                        SanitizeObject(nestedObject);
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.CanWrite)
                {
                    // Sanitize elements in collections (lists, arrays, etc.)
                    IEnumerable? nestedCollection = property.GetValue(input) as IEnumerable;
                    if (nestedCollection != null)
                    {
                        foreach (var item in nestedCollection)
                        {
                            if (item != null && item.GetType().IsClass)
                            {
                                SanitizeObject(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
