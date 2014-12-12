﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynthesizedMethodTypeBase.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Il2Native.Logic.Gencode.SynthesizedMethods
{
    using PEAssemblyReader;

    /// <summary>
    /// </summary>
    public class SynthesizedMethodTypeBase : SynthesizedMethodBase
    {
        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="methodName">
        /// </param>
        public SynthesizedMethodTypeBase(IType type, string methodName)
        {
            this.Type = type.ToNormal();
            this.MethodName = methodName;
        }

        /// <summary>
        /// </summary>
        public override IType DeclaringType
        {
            get
            {
                return this.Type.Clone();
            }
        }

        /// <summary>
        /// </summary>
        public override string ExplicitName
        {
            get
            {
                return string.Concat(this.Type.Name, ".", this.MethodName);
            }
        }

        /// <summary>
        /// </summary>
        public override string FullName
        {
            get
            {
                return string.Concat(this.Type.FullName, ".", this.MethodName);
            }
        }

        /// <summary>
        /// </summary>
        public override string Name
        {
            get
            {
                return this.MethodName;
            }
        }

        /// <summary>
        /// </summary>
        public override IType ReturnType
        {
            get
            {
                return this.Type;
            }
        }

        /// <summary>
        /// </summary>
        protected string MethodName { get; set; }

        /// <summary>
        /// </summary>
        protected IType Type { get; set; }
    }
}