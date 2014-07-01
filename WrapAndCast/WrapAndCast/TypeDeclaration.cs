using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WrapAndCast
{
    internal sealed class TypeDeclaration
    {
        private string name { get; set; }
        private Type baseType { get; set; }
        private Factory factory { get; set; }

        private IEnumerable<Type> types { get; set; }
        private IEnumerable<PropertyInfo> properties { get; set; }
        private IEnumerable<MethodInfo> methods { get; set; }
        private CodeTypeDeclaration newClass { get; set; }

        public TypeDeclaration(Factory factory, string className, Type baseType)
        {
            this.factory = factory;
            name = className;
            this.baseType = baseType;
            Initialize();
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return types;
        }

        public string GetClassName()
        {
            return name;
        }

        public CodeTypeDeclaration Build()
        {
            return GetTypeDeclaration();
        }

        #region Private methods

        private void Initialize()
        {
            InitializeDependencies();
            InitializePropertiesAndMethods();
        }

        private void InitializeDependencies()
        {
            types = GetDependencies(baseType);
        }

        private void InitializePropertiesAndMethods()
        {
            properties = GetProperties(types);
            methods = GetMethods(types);
        }

        private CodeTypeDeclaration GetTypeDeclaration()
        {
            InitializeNewClass();
            AddMethodsPropertiesAndInstanceField();
            return newClass;
        }

        private void InitializeNewClass()
        {
            newClass = CreateCodeTypeDeclaration(name);
            newClass.BaseTypes.AddRange(factory.GetTypeReferenceArray(types));
        }

        public CodeTypeDeclaration CreateCodeTypeDeclaration(string className)
        {
            return factory.CreateDefaultTypeDeclaration(className);
        }

        private void AddMethodsPropertiesAndInstanceField()
        {
            AddInstanceField();
            AddPropertiesToClass(newClass);
            AddMethodsToClass(newClass);
        }

        public void AddInstanceField()
        {
            newClass.Members.Add(factory.CreateMemberField());
        }

        private void AddPropertiesToClass(CodeTypeDeclaration newClass)
        {
            AddTypeMembers(newClass, GetMemberPropertyArray());
        }

        private void AddMethodsToClass(CodeTypeDeclaration newClass)
        {
            AddTypeMembers(newClass, GetMemberMethodArray());
        }

        private CodeMemberProperty[] GetMemberPropertyArray()
        {
            return properties.Select(PropertyInfoConverter).ToArray();
        }

        private CodeMemberProperty PropertyInfoConverter(PropertyInfo p)
        {
            return factory.BuildMemberFromProperty(p);
        }

        private CodeMemberMethod[] GetMemberMethodArray()
        {
            return methods.Select(MethodInfoConverter).ToArray();
        }

        private CodeMemberMethod MethodInfoConverter(MethodInfo m)
        {
            return factory.BuildMemberFromMethod(m);
        }

        private void AddTypeMembers(CodeTypeDeclaration newClass, CodeTypeMember[] members)
        {
            newClass.Members.AddRange(members);
        }

        private IEnumerable<Type> GetDependencies(Type type)
        {
            return (new[] { type }).Union(type.GetInterfaces().SelectMany(GetDependencies));
        }

        private IEnumerable<PropertyInfo> GetProperties(IEnumerable<Type> types)
        {
            return types.SelectMany(PropertyFilter);
        }

        private IEnumerable<MethodInfo> GetMethods(IEnumerable<Type> types)
        {
            return types.SelectMany(MethodFilter);
        }

        private IEnumerable<PropertyInfo> PropertyFilter(Type t)
        {
            return t.GetProperties().Where(p => !p.IsSpecialName);
        }

        private IEnumerable<MethodInfo> MethodFilter(Type t)
        {
            return t.GetMethods().Where(m => !m.IsSpecialName);
        }

        #endregion
    }
}
