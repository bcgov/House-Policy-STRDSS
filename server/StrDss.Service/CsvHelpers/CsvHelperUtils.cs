using CsvHelper;
using CsvHelper.Configuration;
using StrDss.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Service.CsvHelpers
{
    public class CsvHelperUtils
    {
        public static CsvConfiguration GetConfig(Dictionary<string, List<string>> errors, bool checkHeader = true)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            config.PrepareHeaderForMatch = (args) => args.Header.ToLower();

            config.TrimOptions = TrimOptions.Trim;

            if (checkHeader)
            {
                config.HeaderValidated = (args) => {
                    foreach(var header in args.InvalidHeaders)
                    {
                        errors.AddItem($"{header.Names.First()}", $"The header [{header.Names.First()}] is missing.");
                    }
                };
            }
            else
            {
                config.MissingFieldFound = null;
                config.HeaderValidated = null;
            }

            config.IgnoreBlankLines = true;
            config.ShouldSkipRecord = (record) =>
            {
                for (int i = 0; i < record.Row.ColumnCount; i++)
                {
                    if (!string.IsNullOrEmpty(record.Row.GetField(i)))
                    {
                        return false; 
                    }
                }
                return true;
            };

            return config;
        }

        public static string[] GetLowercaseFieldsFromCsvHeaders(string[] headers)
        {
            var fields = new string[headers.Length];

            var i = 0;
            foreach (var header in headers)
            {
                fields[i] = Regex.Replace(header.ToLowerInvariant(), @"[\s]", string.Empty);
                i++;
            }

            return fields;
        }

        public static string GetBase64CsvString<T>(List<T> records) where T : class
        {
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                Encoding = Encoding.UTF8
            };

            using var ms = new MemoryStream();
            using var sr = new StreamWriter(ms);
            using var csvWriter = new CsvWriter(sr, csvConfig);

            csvWriter.WriteRecords(records);
            sr.Flush();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
