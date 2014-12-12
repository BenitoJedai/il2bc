﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataMethodAdapter.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace PEAssemblyReader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Symbols;
    using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
    using System.Reflection.Metadata.Ecma335;

    /// <summary>
    /// </summary>
    [DebuggerDisplay("Name = {ExplicitName}")]
    public class MetadataMethodAdapter : IMethod
    {
        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyExplicitName;

        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyFullName;

        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyMetadataFullName;

        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyMetadataName;

        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyName;

        /// <summary>
        /// </summary>
        private readonly Lazy<string> lazyNamespace;

        /// <summary>
        /// </summary>
        private readonly MethodSymbol methodDef;

        /// <summary>
        /// </summary>
        private bool? isVirtual;
        
        /// <summary>
        /// </summary>
        /// <param name="methodDef">
        /// </param>
        internal MetadataMethodAdapter(MethodSymbol methodDef)
        {
            Debug.Assert(methodDef != null);
            this.methodDef = methodDef;
            this.lazyName = new Lazy<string>(this.CalculateName);
            this.lazyExplicitName = new Lazy<string>(this.CalculateExplicitName);
            this.lazyFullName = new Lazy<string>(this.CalculateFullName);
            this.lazyMetadataName = new Lazy<string>(this.CalculateMetadataName);
            this.lazyMetadataFullName = new Lazy<string>(this.CalculateMetadataFullName);
            this.lazyNamespace = new Lazy<string>(this.CalculateNamespace);
        }

        /// <summary>
        /// </summary>
        /// <param name="methodDef">
        /// </param>
        /// <param name="genericContext">
        /// </param>
        internal MetadataMethodAdapter(MethodSymbol methodDef, IGenericContext genericContext)
            : this(methodDef)
        {
            this.GenericContext = genericContext;

            var peMethodSymbol = methodDef as PEMethodSymbol;
            if (peMethodSymbol != null)
            {
                this.Token = MetadataTokens.GetToken(peMethodSymbol.Handle);
            }
        }

        public int? Token
        {
            get;
            private set;
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string AssemblyQualifiedName
        {
            get
            {
                return this.methodDef.ContainingType.ContainingAssembly.Identity.Name;
            }
        }

        /// <summary>
        /// </summary>
        public CallingConventions CallingConvention
        {
            get
            {
                var callConv = CallingConventions.Standard;
                if (this.methodDef.CallingConvention.HasFlag(Microsoft.Cci.CallingConvention.ExtraArguments))
                {
                    callConv |= CallingConventions.VarArgs;
                }

                if (this.methodDef.CallingConvention.HasFlag(Microsoft.Cci.CallingConvention.HasThis))
                {
                    callConv |= CallingConventions.HasThis;
                }

                if (this.methodDef.CallingConvention.HasFlag(Microsoft.Cci.CallingConvention.ExplicitThis))
                {
                    callConv |= CallingConventions.ExplicitThis;
                }

                return callConv;
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool ContainsGenericParameters
        {
            get
            {
                return this.methodDef.TypeArguments.Any();
            }
        }

        /// <summary>
        /// </summary>
        public IType DeclaringType
        {
            get
            {
                if (this.methodDef.ContainingType.SpecialType == SpecialType.System_Array)
                {
                    var typeSymbol = this.methodDef.AssociatedSymbol as TypeSymbol;
                    if (typeSymbol != null)
                    {
                        return typeSymbol.ResolveGeneric(this.GenericContext);
                    }
                }

                return this.methodDef.ContainingType.ResolveGeneric(this.GenericContext);
            }
        }

        /// <summary>
        /// </summary>
        public DllImportData DllImportData
        {
            get
            {
                return this.methodDef.GetDllImportData();
            }
        }

        /// <summary>
        /// </summary>
        public string ExplicitName
        {
            get
            {
                return this.lazyExplicitName.Value;
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string FullName
        {
            get
            {
                return this.lazyFullName.Value;
            }
        }

        /// <summary>
        /// </summary>
        public IGenericContext GenericContext { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsAbstract
        {
            get
            {
                return this.methodDef.IsAbstract;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// </summary>
        public bool IsUnmanagedDllImport
        {
            get
            {
                // TODO: temporary HACK to find that this is function required dllimport attribute
                return !this.IsUnmanaged && this.methodDef.GetDllImportData() != null && string.IsNullOrWhiteSpace(this.methodDef.GetDllImportData().ModuleName);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsExplicitInterfaceImplementation
        {
            get
            {
                return this.methodDef.IsExplicitInterfaceImplementation;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsExternal
        {
            get
            {
                return this.methodDef.IsExternal;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsGenericMethod
        {
            get
            {
                return this.methodDef.TypeParameters.Any() && !this.GetGenericArguments().Any(tp => tp.IsGenericParameter || tp.IsGenericTypeDefinition);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsGenericMethodDefinition
        {
            get
            {
                return this.methodDef.TypeParameters.Any() && this.GetGenericArguments().Any(tp => tp.IsGenericParameter || tp.IsGenericTypeDefinition);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsOverride
        {
            get
            {
                return this.methodDef.IsOverride;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this.methodDef.IsStatic;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsUnmanaged
        {
            get
            {
                return this.methodDef.ImplementationAttributes.HasFlag(MethodImplAttributes.Unmanaged);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsUnmanagedMethodReference
        {
            get
            {
                return this.methodDef.ImplementationAttributes.HasFlag(MethodImplAttributes.ForwardRef);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsVirtual
        {
            get
            {
                return this.isVirtual.HasValue ? this.isVirtual.Value : (isVirtual = CalculateIsVirtual()).Value;
            }
        }

        public bool IsAnonymousDelegate
        {
            get
            {
                if (!this.methodDef.Name.StartsWith("<"))
                {
                    return false;
                }

                if (!this.methodDef.Parameters.Any())
                {
                    return false;
                }

                var parameterSymbol = this.methodDef.Parameters.First();
                return parameterSymbol.Type.SpecialType == SpecialType.System_Object && parameterSymbol.Name == "value";
            }
        }

        /// <summary>
        /// </summary>
        public string MetadataFullName
        {
            get
            {
                return this.lazyMetadataFullName.Value;
            }
        }

        /// <summary>
        /// </summary>
        public string MetadataName
        {
            get
            {
                return this.lazyMetadataName.Value;
            }
        }

        /// <summary>
        /// </summary>
        public IModule Module
        {
            get
            {
                return new MetadataModuleAdapter(this.methodDef.ContainingModule);
            }
        }

        /// <summary>
        /// </summary>
        public string Name
        {
            get
            {
                return this.lazyName.Value;
            }
        }

        /// <summary>
        /// </summary>
        public string Namespace
        {
            get
            {
                return this.lazyNamespace.Value;
            }
        }

        /// <summary>
        /// </summary>
        public IType ReturnType
        {
            get
            {
                return this.methodDef.ReturnType.ResolveGeneric(this.GenericContext);
            }
        }

        /// <summary>
        /// </summary>
        internal MethodSymbol MethodDef
        {
            get
            {
                return this.methodDef;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsInline
        {
            get;
            protected set;
        }

        /// <summary>
        /// </summary>
        public bool HasProceduralBody
        {
            get;
            protected set;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public int CompareTo(object obj)
        {
            var name = obj as IName;
            if (name == null)
            {
                return 1;
            }

            return this.MetadataFullName.CompareTo(name.MetadataFullName);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public override bool Equals(object obj)
        {
            var type = obj as IName;
            if (type != null)
            {
                return this.CompareTo(type) == 0;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<IType> GetGenericArguments()
        {
            return this.CalculateGenericArguments();
        }

        private IEnumerable<IType> CalculateGenericArguments()
        {
            if (this.methodDef.TypeArguments.Length == 0)
            {
                return new MetadataTypeAdapter[0];
            }

            return this.methodDef.TypeArguments.Select(a => a.ResolveGeneric(this.GenericContext));
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<IType> GetGenericParameters()
        {
            return this.CalculateGenericParameters();
        }

        private IEnumerable<MetadataTypeAdapter> CalculateGenericParameters()
        {
            if (this.methodDef.TypeParameters.Length == 0)
            {
                return new MetadataTypeAdapter[0];
            }

            return this.methodDef.TypeParameters.Select(a => new MetadataTypeAdapter(a));
        }

        private bool CalculateIsVirtual()
        {
            if (this.FullName == "System.Object.Finalize"
                || this.FullName == "System.Object.GetType")
            {
                return true;
            }

            return methodDef.IsVirtual;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="genericContext">
        /// </param>
        /// <returns>
        /// </returns>
        public IMethodBody GetMethodBody(IGenericContext genericContext = null)
        {
            return new MetadataMethodBodyAdapter(this.methodDef, genericContext ?? this.GenericContext);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IMethod GetMethodDefinition()
        {
            return new MetadataMethodAdapter(this.methodDef.ConstructedFrom);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<IParameter> GetParameters()
        {
            return this.methodDef.Parameters.Select(p => new MetadataParameterAdapter(p, this.GenericContext));
        }

        /// <summary>
        /// </summary>
        /// <param name="typeParameter">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// </exception>
        public IType ResolveTypeParameter(IType typeParameter)
        {
            var typeParameters = this.GetGenericParameters().ToList();
            var typeArguments = this.GetGenericArguments().ToList();

            for (var index = 0; index < typeArguments.Count; index++)
            {
                if (typeParameters[index].TypeEquals(typeParameter))
                {
                    return typeArguments[index];
                }
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// </summary>
        /// <param name="genericContext">
        /// </param>
        /// <returns>
        /// </returns>
        public IMethod ToSpecialization(IGenericContext genericContext)
        {
            return this.methodDef.ResolveGeneric(genericContext);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            var result = new StringBuilder();

            // write return type
            result.Append(this.ReturnType);
            result.Append(' ');

            // write Full Name
            result.Append(this.FullName);

            // write Parameter Types
            result.Append('(');
            var index = 0;
            foreach (var parameterType in this.GetParameters())
            {
                if (index != 0)
                {
                    result.Append(", ");
                }

                result.Append(parameterType);
                index++;
            }

            if (this.CallingConvention.HasFlag(CallingConventions.VarArgs))
            {
                {
                    result.Append(", ");
                }

                result.Append("__arglist");
            }

            result.Append(')');

            return result.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="ownerOfExplicitInterface">
        /// </param>
        /// <returns>
        /// </returns>
        public string ToString(IType ownerOfExplicitInterface)
        {
            var result = new StringBuilder();

            // write return type
            result.Append(this.ReturnType);
            result.Append(' ');

            // write Full Name
            result.Append(ownerOfExplicitInterface.FullName);
            result.Append('.');
            result.Append(this.FullName);

            // write Parameter Types
            result.Append('(');
            var index = 0;
            foreach (var parameterType in this.GetParameters())
            {
                if (index != 0)
                {
                    result.Append(", ");
                }

                result.Append(parameterType);
                index++;
            }

            result.Append(')');

            return result.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateExplicitName()
        {
            var result = new StringBuilder();

            if (this.methodDef.ContainingType != null && !string.IsNullOrWhiteSpace(this.methodDef.ContainingType.Name))
            {
                if (this.methodDef.ContainingType.IsNestedType())
                {
                    result.Append(this.methodDef.ContainingType.ContainingType.ResolveGeneric(this.GenericContext).Name);
                    ////result.Append('+');
                    // Metadata explicitname should contains +
                    result.Append('.');
                }

                result.Append(this.methodDef.ContainingType.ResolveGeneric(this.GenericContext).Name);
                result.Append('.');
            }

            result.Append(this.Name);

            return result.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateFullName()
        {
            var result = new StringBuilder();
            this.methodDef.AppendFullNamespace(result, this.Namespace, this.DeclaringType, false, '.');
            result.Append(this.Name);

            return result.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateMetadataFullName()
        {
            var sb = new StringBuilder();
            this.methodDef.AppendFullNamespace(sb, this.Namespace, this.DeclaringType, true, '.');
            sb.Append(this.MetadataName);

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateMetadataName()
        {
            var sb = new StringBuilder();

            sb.Append(this.methodDef.Name);

            if (this.IsGenericMethod || this.IsGenericMethodDefinition)
            {
                sb.Append('`');
                sb.Append(this.methodDef.GetArity());
            }

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateName()
        {
            var sb = new StringBuilder();

            if (this.methodDef.ContainingType.IsGenericType && this.methodDef.IsExplicitInterfaceImplementation)
            {
                var implMethodSymbol = this.methodDef.ExplicitInterfaceImplementations.First();
                var resolveType = implMethodSymbol.ResolveGeneric(this.GenericContext);
                sb.Append(resolveType.FullName);
            }
            else
            {
                sb.Append(this.methodDef.Name);
            }

            if (this.IsGenericMethod || this.IsGenericMethodDefinition)
            {
                sb.Append('<');

                var index = 0;
                foreach (var genArg in this.GetGenericArguments())
                {
                    if (index++ > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(genArg.FullName);
                }

                sb.Append('>');
            }

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string CalculateNamespace()
        {
            return this.methodDef.CalculateNamespace();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private MetadataGenericContext GetDefaultGenericMetadataContext()
        {
            if (this.IsGenericMethodDefinition)
            {
                return new MetadataGenericContext(this);
            }

            if (this.DeclaringType.IsGenericType)
            {
                return new MetadataGenericContext(this.DeclaringType);
            }

            return null;
        }
    }
}