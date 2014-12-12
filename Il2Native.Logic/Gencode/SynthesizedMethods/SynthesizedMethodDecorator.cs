﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynthesizedMethodBase.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Il2Native.Logic.Gencode.SynthesizedMethods
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.CodeAnalysis;

    using PEAssemblyReader;
    using System.Text;

    /// <summary>
    /// </summary>
    public class SynthesizedMethodDecorator : IMethod
    {
        private readonly IMethod method;
        private readonly IMethodBody methodBody;
        private readonly IEnumerable<IParameter> parameters;
        private readonly IModule module;

        public SynthesizedMethodDecorator(IMethod method)
        {
            this.method = method;
        }

        public SynthesizedMethodDecorator(IMethod method, IMethodBody methodBody, IEnumerable<IType> parameters, IModule module) : this(method)
        {
            this.methodBody = methodBody;
            this.module = module;
            this.parameters = parameters.Select(t => new SynthesizedValueParameter(t)).ToList();
        }

        public string AssemblyQualifiedName 
        {
            get
            {
                return this.method.AssemblyQualifiedName;
            }
        }

        public IType DeclaringType
        {
            get
            {
                return this.method.DeclaringType;
            }
        }

        public string FullName
        {
            get
            {
                return this.method.FullName;
            }
        }

        public string MetadataFullName
        {
            get
            {
                return this.method.MetadataFullName;
            }
        }

        public string MetadataName
        {
            get
            {
                return this.method.MetadataName;
            }
        }

        public string Name
        {
            get
            {
                return this.method.Name;
            }
        }

        public string Namespace
        {
            get
            {
                return this.method.Namespace;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return this.method.IsAbstract;
            }
        }

        public bool IsOverride
        {
            get
            {
                return this.method.IsOverride;
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.method.IsStatic;
            }
        }

        public bool IsVirtual
        {
            get
            {
                return this.method.IsVirtual;
            }
        }

        public bool IsAnonymousDelegate
        {
            get
            {
                return this.method.IsAnonymousDelegate;
            }
        }

        public IModule Module
        {
            get
            {
                return this.module ?? this.method.Module;
            }
        }

        public int? Token
        {
            get
            {
                return this.method.Token;
            }
        }

        public CallingConventions CallingConvention
        {
            get
            {
                return this.method.CallingConvention;
            }
        }

        public DllImportData DllImportData
        {
            get
            {
                return this.method.DllImportData;
            }
        }

        public string ExplicitName
        {
            get
            {
                return this.method.ExplicitName;
            }
        }

        public bool IsConstructor
        {
            get
            {
                return this.method.IsConstructor;
            }
        }

        public bool IsUnmanagedDllImport
        {
            get
            {
                return this.method.IsUnmanagedDllImport;
            }
        }

        public bool IsExplicitInterfaceImplementation
        {
            get
            {
                return this.method.IsExplicitInterfaceImplementation;
            }
        }

        public bool IsExternal
        {
            get
            {
                return this.method.IsExternal;
            }
        }

        public bool IsGenericMethod
        {
            get
            {
                return this.method.IsGenericMethod;
            }
        }

        public bool IsGenericMethodDefinition
        {
            get
            {
                return this.method.IsGenericMethodDefinition;
            }
        }

        public bool IsUnmanaged
        {
            get
            {
                return this.method.IsUnmanaged;
            }
        }

        public bool IsUnmanagedMethodReference
        {
            get
            {
                return this.method.IsUnmanagedMethodReference;
            }
        }

        public IType ReturnType
        {
            get
            {
                return this.method.ReturnType;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsInline
        {
            get
            {
                return this.method.IsInline;
            }
        }

        /// <summary>
        /// </summary>
        public bool HasProceduralBody
        {
            get
            {
                return this.method.HasProceduralBody;
            }
        }

        public int CompareTo(object obj)
        {
            return this.method.CompareTo(obj);
        }

        public IEnumerable<IType> GetGenericArguments()
        {
            return this.method.GetGenericArguments();
        }

        public IEnumerable<IType> GetGenericParameters()
        {
            return this.method.GetGenericParameters();
        }

        public IMethodBody GetMethodBody(IGenericContext genericContext = null)
        {
            return this.methodBody ?? this.method.GetMethodBody(genericContext);
        }

        public IMethod GetMethodDefinition()
        {
            return this.method.GetMethodDefinition();
        }

        public IEnumerable<IParameter> GetParameters()
        {
            if (this.parameters != null)
            {
                return this.parameters;
            }

            return this.method.GetParameters();
        }

        public IMethod ToSpecialization(IGenericContext genericContext)
        {
            return this.method.ToSpecialization(genericContext);
        }

        public string ToString(IType ownerOfExplicitInterface)
        {
            return this.method.ToString(ownerOfExplicitInterface);
        }

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

            result.Append(')');

            return result.ToString();
        }
    }
}