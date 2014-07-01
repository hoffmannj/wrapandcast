using System;

namespace WrapAndCast
{
    internal sealed class TypeBuilder
    {
        private Type iType { get; set; }
        private readonly Random random = new Random();

        private Factory factory { get; set; }
        private string className { get; set; }

        public TypeBuilder(Factory factory, Type type)
        {
            this.factory = factory;
            iType = type;
        }

        public Type Build()
        {
            InitializeClassName();
            var tc = factory.CreateTypeCompiler(iType.Namespace);
            tc.SetTypeDeclaration(factory.CreateTypeDeclaration(className, iType));
            return tc.Compile();
        }

        #region Private method

        private void InitializeClassName()
        {
            className = "DynamicClass" + iType.Name + random.Next(1000000);
        }

        #endregion
    }
}
