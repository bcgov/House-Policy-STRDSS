var t = typeof(AutoMapper.MapperConfiguration); foreach(var c in t.GetConstructors()) { Console.WriteLine(string.Join(", ", c.GetParameters().Select(p => p.ParameterType + " " + p.Name))); }
