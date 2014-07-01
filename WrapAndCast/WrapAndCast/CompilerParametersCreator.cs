using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace WrapAndCast
{
    internal sealed class CompilerParametersCreator
    {
        private readonly string[] defaultAssemblies = {
                                                        "System.dll",
                                                        "System.Core.dll",
                                                        "System.Data.dll",
                                                        "System.Data.DataSetExtensions.dll",
                                                        "System.Xml.dll",
                                                        "System.Xml.Linq.dll",
                                                        "Microsoft.CSharp.dll"
                                                      };
        private readonly List<string> additionalAssemblies = new List<string>();
        private IEnumerable<Type> types { get; set; }
        private Factory factory { get; set; }


        public CompilerParametersCreator(Factory factory, IEnumerable<Type> types)
        {
            this.factory = factory;
            this.types = types;
        }

        public CompilerParameters CreateCompilerParameters()
        {
            AddAssembliesByTypes();
            var cp = factory.CreateDefaultCompilerParameters();
            cp.ReferencedAssemblies.AddRange(defaultAssemblies);
            cp.ReferencedAssemblies.AddRange(additionalAssemblies.ToArray());
            return cp;
        }

        #region Private methods

        private void AddAssembliesByTypes()
        {
            additionalAssemblies.AddRange(types.Select(GetTypeAssemblyLocation).ToArray());
        }

        private string GetTypeAssemblyLocation(Type type)
        {
            return new Uri(type.Assembly.CodeBase).LocalPath;
        }

        #endregion
    }
}
