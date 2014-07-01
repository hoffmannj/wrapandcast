using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WrapAndCast
{
    internal sealed class Castor
    {
        private readonly Factory factory = new Factory();

        public object To(Type type, object instance)
        {
            AssertInterface(type);
            AssertInstance(instance);
            if (type.IsInstanceOfType(instance)) return instance;
            CacheType(type);
            return GetInitializedInstance(CastIt.typeDict[type], instance);
        }

        public void CacheType(Type type)
        {
            AssertInterface(type);
            CastIt.typeDict[type] = GetNewDynamicType(type);
        }

        public void CacheFromThisAssembly(object instance)
        {
            AssertInstance(instance);
            GetInterfacesInAssemblyByType(instance.GetType()).ForEach(CacheType);
        }

        public void CleanType(Type type)
        {
            if (CastIt.typeDict.ContainsKey(type)) CastIt.typeDict.Remove(type);
        }

        #region Private methods

        private object GetInitializedInstance(Type t, object instance)
        {
            var o = Activator.CreateInstance(t);
            InitializePrivateInstanceField(t, o, instance);
            return o;
        }

        private void InitializePrivateInstanceField(Type t, object instance, object value)
        {
            GetPrivateInstanceField(t).SetValue(instance, value);
        }

        private FieldInfo GetPrivateInstanceField(Type t)
        {
            return t.GetField(factory.GetInstanceFieldName(), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void AssertInterface(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsInterface) throw new ArgumentException("Type is not interface.", "type");
        }

        private void AssertInstance(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
        }

        private Type GetNewDynamicType(Type iType)
        {
            Type retType = null;
            if (CastIt.typeDict.TryGetValue(iType, out retType)) return retType;
            return factory.BuildNewTypeFromType(iType);
        }

        private IEnumerable<Type> GetInterfacesInAssemblyByType(Type baseType)
        {
            return FilterTypesToInterfaces(GetTypesInAssemblyByType(baseType));
        }

        private Type[] GetTypesInAssemblyByType(Type baseType)
        {
            return baseType.Assembly.GetTypes();
        }

        private IEnumerable<Type> FilterTypesToInterfaces(Type[] types)
        {
            return types.Where(InterfaceFilter);
        }

        private bool InterfaceFilter(Type t)
        {
            return t.IsInterface;
        }

        #endregion
    }
}
