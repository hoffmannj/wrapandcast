using System.CodeDom;
using System.Linq;
using System.Reflection;

namespace WrapAndCast
{
    internal sealed class MethodBuilder
    {
        private const string voidTypeName = "System.Void";

        private Factory factory { get; set; }

        private MethodInfo methodInfo { get; set; }
        private CodeMemberMethod method { get; set; }
        private bool hasReturn { get; set; }
        private CodeMethodInvokeExpression invoke { get; set; }

        public MethodBuilder(Factory factory, MethodInfo methodInfo)
        {
            this.factory = factory;
            this.methodInfo = methodInfo;
        }

        public CodeMemberMethod Build()
        {
            InitializeMethod();
            InitializeGenericParameters();
            InitializeHasReturn();
            InitializeInvoke();
            InitializeStatements();
            return method;
        }

        #region Private methods

        private void InitializeMethod()
        {
            method = factory.CreateMemberMethod(methodInfo);
        }

        private void InitializeGenericParameters()
        {
            AddTypeParametersToMethod();
        }

        private void AddTypeParametersToMethod()
        {
            if (!IsGeneric()) return;
            method.TypeParameters.AddRange(GetCodeTypeParameterArray());
        }

        private CodeTypeParameter[] GetCodeTypeParameterArray()
        {
            return factory.GetGenericParameters(methodInfo).Select(factory.TypeParameterFromTypeName).ToArray();
        }

        private bool IsGeneric()
        {
            return methodInfo.IsGenericMethod;
        }

        private void InitializeHasReturn()
        {
            hasReturn = methodInfo.ReturnType.FullName != voidTypeName;
        }

        private void InitializeInvoke()
        {
            invoke = factory.CreateMethodInvokeExpression(methodInfo);
        }

        private void InitializeStatements()
        {
            if (hasReturn)
            {
                method.Statements.Add(factory.CreateReturnStatementFromExpression(invoke));
            }
            else
            {
                method.Statements.Add(invoke);
            }
        }

        #endregion
    }
}
