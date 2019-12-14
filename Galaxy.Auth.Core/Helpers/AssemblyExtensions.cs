using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Galaxy.Auth.Presentation.Ioc
{
    public static class AssemblyExtensions
    {
        public static List<Type> GetTypesForPath(this Assembly assembly, string path)
        {
            return (from t in assembly.GetTypes()
                where t.IsClass &&
                      t.IsNotAbstractNorNested() &&
                      t.IsPathValid(path)
                select t).ToList();
        }  
    }
}