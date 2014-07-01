using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WrapAndCast
{
    public static class CastIt
    {
        internal static readonly Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();

        public static void CacheFromThisAssembly(object instance)
        {
            new Castor().CacheFromThisAssembly(instance);
        }

        public static void CacheType<T>()
        {
            CacheType(typeof(T));
        }

        public static void CacheType(Type type)
        {
            new Castor().CacheType(type);
        }

        public static void CleanType<T>()
        {
            CleanType(typeof(T));
        }

        public static void CleanType(Type type)
        {
            new Castor().CleanType(type);
        }

        public static T As<T>(this object self)
        {
            return To<T>(self);
        }

        public static T To<T>(object instance)
        {
            return (T)new Castor().To(typeof(T), instance);
        }
    }
}
