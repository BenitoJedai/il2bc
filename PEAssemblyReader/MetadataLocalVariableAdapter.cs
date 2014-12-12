﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataLocalVariableAdapter.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace PEAssemblyReader
{
    using System;
    using System.Diagnostics;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;

    /// <summary>
    /// </summary>
    [DebuggerDisplay("Index = {LocalIndex}, Type = {LocalType.FullName}")]
    public class MetadataLocalVariableAdapter : ILocalVariable
    {
        /// <summary>
        /// </summary>
        private MetadataDecoder<TypeSymbol, MethodSymbol, FieldSymbol, AssemblySymbol, Symbol>.LocalInfo localInfo;

        // we are using it to replace Pinner IntPtr& with Void* to work with it as value
        /// <summary>
        /// </summary>
        private IType localTypeReplaced;

        /// <summary>
        /// </summary>
        /// <param name="localInfo">
        /// </param>
        /// <param name="index">
        /// </param>
        internal MetadataLocalVariableAdapter(MetadataDecoder<TypeSymbol, MethodSymbol, FieldSymbol, AssemblySymbol, Symbol>.LocalInfo localInfo, int index)
        {
            Debug.Assert(localInfo.Type.TypeKind != TypeKind.Error);
            this.localInfo = localInfo;
            this.LocalIndex = index;
        }

        /// <summary>
        /// </summary>
        /// <param name="localInfo">
        /// </param>
        /// <param name="index">
        /// </param>
        /// <param name="genericContext">
        /// </param>
        internal MetadataLocalVariableAdapter(
            MetadataDecoder<TypeSymbol, MethodSymbol, FieldSymbol, AssemblySymbol, Symbol>.LocalInfo localInfo, int index, IGenericContext genericContext)
            : this(localInfo, index)
        {
            this.GenericContext = genericContext;
        }

        /// <summary>
        /// </summary>
        public IGenericContext GenericContext { get; set; }

        /// <summary>
        /// </summary>
        public int LocalIndex { get; protected set; }

        /// <summary>
        /// </summary>
        public IType LocalType
        {
            get
            {
                if (this.localTypeReplaced != null)
                {
                    return this.localTypeReplaced;
                }

                var localType = this.localInfo.Type.ResolveGeneric(this.GenericContext, this.localInfo.IsByRef, this.localInfo.IsPinned);
                return localType;
            }

            set
            {
                this.localTypeReplaced = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}