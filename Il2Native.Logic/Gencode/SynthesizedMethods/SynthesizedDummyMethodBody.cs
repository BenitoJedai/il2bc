﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynthesizedDummyMethodBody.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Il2Native.Logic.Gencode.SynthesizedMethods
{
    using System.Collections.Generic;

    using PEAssemblyReader;

    /// <summary>
    /// </summary>
    public class SynthesizedDummyMethodBody : IMethodBody
    {
        /// <summary>
        /// </summary>
        public IEnumerable<IExceptionHandlingClause> ExceptionHandlingClauses
        {
            get
            {
                return new IExceptionHandlingClause[0];
            }
        }

        /// <summary>
        /// </summary>
        public bool HasBody
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// </summary>
        public IEnumerable<ILocalVariable> LocalVariables
        {
            get
            {
                return new ILocalVariable[0];
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public byte[] GetILAsByteArray()
        {
            return new byte[0];
        }
    }
}