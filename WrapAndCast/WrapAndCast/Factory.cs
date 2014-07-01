using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace WrapAndCast
{
    internal sealed class Factory
    {
        private const TypeAttributes defaultTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public;

        public string GetInstanceFieldName()
        {
            return "_____instance";
        }

        public Type BuildNewTypeFromType(Type t)
        {
            return new TypeBuilder(this, t).Build();
        }

        public CompilerParameters CreateDefaultCompilerParameters()
        {
            return new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/optimize",
            };
        }

        public CodeMemberMethod CreateMemberMethod(MethodInfo methodInfo)
        {
            var method = new CodeMemberMethod()
            {
                Name = methodInfo.Name,
                Attributes = MemberAttributes.Public,
                ReturnType = TypeToTypeReference(methodInfo.ReturnType)
            };
            method.Parameters.AddRange(GetMethodParameterArray(methodInfo));
            return method;
        }

        public CodeMemberProperty CreateMemberProperty(PropertyInfo propertyInfo)
        {
            return new CodeMemberProperty()
            {
                Name = propertyInfo.Name,
                Type = TypeToTypeReference(propertyInfo.PropertyType),
                Attributes = MemberAttributes.Public
            };
        }

        public CodeTypeParameter TypeParameterFromTypeName(Type t)
        {
            return new CodeTypeParameter(t.Name);
        }

        public CodeMethodInvokeExpression CreateMethodInvokeExpression(MethodInfo methodInfo)
        {
            return new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetInstanceFieldName()),
                                methodInfo.Name, GetTypeReferenceArray(GetGenericParameters(methodInfo))
                        ),
                        GetArgumentReferenceExpressionArray(methodInfo)
                );
        }

        public CodeStatement CreateReturnStatementFromExpression(CodeExpression expr)
        {
            return new CodeMethodReturnStatement(expr);
        }

        public Type[] GetGenericParameters(MethodInfo methodInfo)
        {
            return methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments() : new Type[0];
        }

        public CodeStatement CreateMethodReturnStatement(PropertyInfo propertyInfo)
        {
            return new CodeMethodReturnStatement(
                            new CodePropertyReferenceExpression(
                                GetInstanceFieldReferenceExpression(),
                                propertyInfo.Name));
        }

        public CodeStatement CreateAssignStatement(PropertyInfo propertyInfo)
        {
            return new CodeAssignStatement(
                            new CodePropertyReferenceExpression(
                                GetInstanceFieldReferenceExpression(),
                                propertyInfo.Name),
                            new CodePropertySetValueReferenceExpression());
        }

        public TypeCompiler CreateTypeCompiler(string namespaceName)
        {
            return new TypeCompiler(this, namespaceName);
        }

        public TypeDeclaration CreateTypeDeclaration(string className, Type baseType)
        {
            return new TypeDeclaration(this, className, baseType);
        }

        public CodeNamespace CreateNamespace(string namespaceName){
            return new CodeNamespace(namespaceName);
        }

        public CodeNamespaceImport CreateNamespaceImport(string namespaceName)
        {
            return new CodeNamespaceImport(namespaceName);
        }

        public CodeCompileUnit CreateCompileUnit()
        {
            return new CodeCompileUnit();
        }

        public CSharpCodeProvider CreateCodeProvider()
        {
            return new CSharpCodeProvider();
        }

        public CompilerParametersCreator CreateCompilerParameterCreator(TypeDeclaration typeDeclaration)
        {
            return new CompilerParametersCreator(this, typeDeclaration.GetAllTypes());
        }

        public CodeTypeDeclaration CreateDefaultTypeDeclaration(string className)
        {
            return new CodeTypeDeclaration(className)
            {
                IsClass = true,
                TypeAttributes = defaultTypeAttributes
            };
        }

        public CodeMemberField CreateMemberField()
        {
            return new CodeMemberField
            {
                Name = GetInstanceFieldName(),
                Attributes = MemberAttributes.Private,
                Type = new CodeTypeReference("dynamic")
            };
        }

        public CodeMemberProperty BuildMemberFromProperty(PropertyInfo propertyInfo)
        {
            return new PropertyBuilder(this, propertyInfo).Build();
        }

        public CodeMemberMethod BuildMemberFromMethod(MethodInfo methodInfo)
        {
            return new MethodBuilder(this, methodInfo).Build();
        }

        public CodeTypeReference[] GetTypeReferenceArray(IEnumerable<Type> types)
        {
            return types.Select(TypeToTypeReference).ToArray();
        }


        #region private methods

        private CodeTypeReference TypeToTypeReference(Type t)
        {
            return new CodeTypeReference(t);
        }

        private CodeExpression GetInstanceFieldReferenceExpression()
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetInstanceFieldName());
        }

        private ParameterInfo[] GetMethodParameters(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters();
        }

        private CodeParameterDeclarationExpression[] GetMethodParameterArray(MethodInfo methodInfo)
        {
            return GetMethodParameters(methodInfo).Select(ParameterDeclarationSelector).ToArray();
        }

        private CodeParameterDeclarationExpression ParameterDeclarationSelector(ParameterInfo p)
        {
            return new CodeParameterDeclarationExpression(p.ParameterType, p.Name);
        }

        private CodeExpression[] GetArgumentReferenceExpressionArray(MethodInfo methodInfo)
        {
            return GetMethodParameters(methodInfo).Select(ParameterNameSelector).ToArray();
        }

        private CodeArgumentReferenceExpression ParameterNameSelector(ParameterInfo p)
        {
            return new CodeArgumentReferenceExpression(p.Name);
        }

        #endregion
    }
}
