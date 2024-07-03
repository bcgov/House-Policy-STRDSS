using System.IO.Compression;
using System.Reflection;

namespace StrDss.Common
{
    public static class CommonUtils
    {
        public static string GetFullName(string firstName, string lastName)
        {
            if (firstName.IsNotEmpty() && lastName.IsNotEmpty())
            {
                return $"{lastName}, {firstName}";
            }
            else if (firstName.IsNotEmpty())
            {
                return firstName;
            }
            else
            {
                return lastName;
            }
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
    }
}
