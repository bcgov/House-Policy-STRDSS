using System.Reflection;

namespace StrDss.Common
{
    public static class CommonUtils
    {
        public static string GetFullName(string firstName, string lastName)
        {
            if (!firstName.IsEmpty() && !lastName.IsEmpty())
            {
                return $"{lastName}, {firstName}";
            }
            else if (!firstName.IsEmpty())
            {
                return lastName;
            }
            else
            {
                return firstName;
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
            if (str == null)
            {
                return null;
            }

            return Convert.ToInt16(str);
        }
    }
}
