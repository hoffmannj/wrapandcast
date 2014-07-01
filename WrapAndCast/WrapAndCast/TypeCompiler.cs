using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;

namespace WrapAndCast
{
    internal sealed class TypeCompiler
    {
        private readonly string[] defaultNamespaces = {
                                                      "System",
                                                      "System.Data",
                                                      "System.Data.DataSetExtensions",
                                                      "System.Xml",
                                                      "System.Xml.Linq",
                                                      "Microsoft.CSharp.RuntimeBinder"
                                                      };

        private Factory factory { get; set; }
        private string namespaceName { get; set; }
        private CodeNamespace nameSpace { get; set; }
        private CodeCompileUnit compileUnit { get; set; }
        private TypeDeclaration typeDeclaration { get; set; }

        public TypeCompiler(Factory factory, string nameSpace)
        {
            this.factory = factory;
            this.namespaceName = nameSpace;
            InitializeNamespace();
            InitializeCompileUnit();
        }

        public void SetTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            this.typeDeclaration = typeDeclaration;
        }

        public Type Compile()
        {
            nameSpace.Types.Add(typeDeclaration.Build());
            return BuildNewType();
        }

        #region Private methods

        private void InitializeNamespace()
        {
            nameSpace = CreateNamespace(namespaceName);
        }

        private CodeNamespace CreateNamespace(string name)
        {
            var ns = factory.CreateNamespace(name);
            ns.Imports.AddRange(GetNamespaceArray());
            return ns;
        }

        private CodeNamespaceImport[] GetNamespaceArray()
        {
            return defaultNamespaces.Select(NamespaceConverter).ToArray();
        }

        private CodeNamespaceImport NamespaceConverter(string nsName)
        {
            return factory.CreateNamespaceImport(nsName);
        }

        private void InitializeCompileUnit()
        {
            compileUnit = CreateCompilerUnit(nameSpace);
        }

        public CodeCompileUnit CreateCompilerUnit(CodeNamespace ns)
        {
            var ccu = factory.CreateCompileUnit();
            ccu.Namespaces.Add(ns);
            return ccu;
        }

        private Type BuildNewType()
        {
            var results = GetCompilerResults();
            AssertCompilingOk(results);
            return GetTypeFromResults(results);
        }

        private CompilerResults GetCompilerResults()
        {
            return factory.CreateCodeProvider().CompileAssemblyFromDom(CreateCompilerParameters(typeDeclaration), compileUnit);
        }

        public CompilerParameters CreateCompilerParameters(TypeDeclaration typeDeclaration)
        {
            return factory.CreateCompilerParameterCreator(typeDeclaration).CreateCompilerParameters();
        }

        private void AssertCompilingOk(CompilerResults results)
        {
            if (results.Errors.Count > 0)
                ThrowCompileFailedException();
        }

        private Type GetTypeFromResults(CompilerResults results)
        {
            return results.CompiledAssembly.GetType(GetTypeName());
        }

        private string GetTypeName()
        {
            return nameSpace.Name + "." + typeDeclaration.GetClassName();
        }

        private object ThrowCompileFailedException()
        {
            throw new Exception("Creating instance failed!");
        }

        #endregion
    }
}
