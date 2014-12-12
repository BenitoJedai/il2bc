﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGenericContext.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace PEAssemblyReader
{
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    public interface IGenericContext
    {
        /// <summary>
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// </summary>
        IDictionary<IType, IType> Map { get; }

        /// <summary>
        /// </summary>
        IMethod MethodDefinition { get; }

        /// <summary>
        /// </summary>
        IMethod MethodSpecialization { get; }

        /// <summary>
        /// </summary>
        IType TypeDefinition { get; }

        /// <summary>
        /// </summary>
        IType TypeSpecialization { get; }

        /// <summary>
        /// </summary>
        /// <param name="typeParameter">
        /// </param>
        /// <returns>
        /// </returns>
        IType ResolveTypeParameter(IType typeParameter, bool isByRef = false, bool isPinned = false);
    }
}