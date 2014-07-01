using System.CodeDom;
using System.Reflection;

namespace WrapAndCast
{
    internal sealed class PropertyBuilder
    {
        private Factory factory { get; set; }
        private PropertyInfo propertyInfo { get; set; }
        private CodeMemberProperty memberProperty { get; set; }

        public PropertyBuilder(Factory factory, PropertyInfo propertyInfo)
        {
            this.factory = factory;
            this.propertyInfo = propertyInfo;
        }

        public CodeMemberProperty Build()
        {
            InitializeMemberProperty();
            InitializeGetSetMethods();
            InitializeGenericParameters();
            BuildGetter();
            BuildSetter();
            return memberProperty;
        }

        #region Private methods

        private void InitializeMemberProperty()
        {
            memberProperty = factory.CreateMemberProperty(propertyInfo);
        }

        private void InitializeGetSetMethods()
        {
            var gm = propertyInfo.GetGetMethod();
            var sm = propertyInfo.GetSetMethod();
            memberProperty.HasGet = gm != null;
            memberProperty.HasSet = sm != null;
        }

        private void InitializeGenericParameters()
        {
            if (propertyInfo.ReflectedType != null) memberProperty.ImplementationTypes.Add(propertyInfo.ReflectedType);
        }

        private void BuildGetter()
        {
            if (!memberProperty.HasGet) return;
            memberProperty.GetStatements.Add(factory.CreateMethodReturnStatement(propertyInfo));
        }

        private void BuildSetter()
        {
            if (!memberProperty.HasSet) return;
            memberProperty.SetStatements.Add(factory.CreateAssignStatement(propertyInfo));
        }

        #endregion
    }
}
