using NetTopologySuite.Geometries;
using System.Reflection;

namespace StrDss.Common
{
    public static class CommonUtils
    {
        public static Coordinate[] PointsToCoordinate(double[][] points)
        {
            var coordinates = new List<Coordinate>();
            foreach (var point in points)
            {
                coordinates.Add(new Coordinate(point[0], point[1]));
            }

            return coordinates.ToArray();
        }

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
    }
}
