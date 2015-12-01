using System;
using System.Collections.Generic;
using System.Text;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public static class Reflect
    {
        public static string GetTypeNameOfConcreteAndParentTypes(Type baseType)
        {
            var sb = new StringBuilder(baseType.AssemblyQualifiedName);

            var parentTypes = GetParentTypes(baseType);

            foreach (var parentType in parentTypes)
            {
                sb.Append(String.Concat(";", parentType.AssemblyQualifiedName));
            }

            return sb.ToString();
        }

        public static IEnumerable<Type> GetParentTypes(Type type)
        {
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            var objectType = typeof(Object);
            while (currentBaseType != null && currentBaseType != objectType)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }
    }
}
