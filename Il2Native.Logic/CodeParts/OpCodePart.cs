﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpCodePart.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Il2Native.Logic.CodeParts
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection.Emit;

    using Il2Native.Logic.Exceptions;

    using PEAssemblyReader;

    using OpCodesEmit = System.Reflection.Emit.OpCodes;

    /// <summary>
    /// </summary>
    [DebuggerDisplay("{OpCode.Name}, {OpCode.FlowControl}, {OpCode.StackBehaviourPop}, {OpCode.StackBehaviourPush}")]
    public class OpCodePart
    {
        /// <summary>
        /// </summary>
        private FullyDefinedReference result;

        /// <summary>
        /// </summary>
        /// <param name="opcode">
        /// </param>
        /// <param name="addressStart">
        /// </param>
        /// <param name="addressEnd">
        /// </param>
        public OpCodePart(OpCode opcode, int addressStart, int addressEnd)
        {
            this.OpCode = opcode;
            this.AddressStart = addressStart;
            this.AddressEnd = addressEnd;
        }

        /// <summary>
        /// </summary>
        protected OpCodePart()
        {
        }

        /// <summary>
        /// </summary>
        public static OpCodePart CreateNop
        {
            get
            {
                return new OpCodePart(OpCodesEmit.Nop, 0, 0);
            }
        }

        /// <summary>
        /// </summary>
        public int AddressEnd { get; private set; }

        /// <summary>
        /// </summary>
        public int AddressStart { get; private set; }

        /// <summary>
        /// </summary>
        public PhiNodes AlternativeValues { get; set; }

        /// <summary>
        /// </summary>
        public OpCodePart BranchStackValue { get; set; }

        /// <summary>
        /// </summary>
        public CatchOfFinallyClause CatchOrFinallyBegin { get; set; }

        /// <summary>
        /// </summary>
        public IList<CatchOfFinallyClause> CatchOrFinallyEnds { get; set; }

        /// <summary>
        /// </summary>
        public string CreatedLabel { get; set; }

        /// <summary>
        /// </summary>
        public IList<CatchOfFinallyClause> ExceptionHandlers { get; set; }

        /// <summary>
        /// </summary>
        public virtual int GroupAddressEnd
        {
            get
            {
                return this.AddressEnd;
            }
        }

        /// <summary>
        /// </summary>
        public virtual int GroupAddressStart
        {
            get
            {
                return this.OpCodeOperands != null && this.OpCodeOperands.Length > 0 && this.OpCodeOperands[0] != this
                           ? this.OpCodeOperands[0].GroupAddressStart
                           : this.AddressStart;
            }
        }

        /// <summary>
        /// </summary>
        public bool HasDup
        {
            get
            {
                // todo: fix for 2 bytes command
                if (this.ToCode() == Code.Dup)
                {
                    return true;
                }

                if (this.OpCodeOperands != null)
                {
                    return this.OpCodeOperands.Any(u => u.HasDup);
                }

                return false;
            }
        }

        /// <summary>
        /// </summary>
        public bool HasResult
        {
            get
            {
                return this.Result != null;
            }
        }

        /// <summary>
        /// </summary>
        public List<OpCodePart> JumpDestination { get; set; }

        /// <summary>
        /// </summary>
        public bool JumpProcessed { get; set; }

        /// <summary>
        /// </summary>
        public OpCodePart Next { get; set; }

        /// <summary>
        /// </summary>
        public OpCode OpCode { get; private set; }

        /// <summary>
        /// </summary>
        public OpCodePart[] OpCodeOperands { get; set; }

        /// <summary>
        /// </summary>
        public OpCodePart Previous { get; set; }

        /// <summary>
        /// </summary>
        public bool ReadExceptionFromStack { get; set; }

        /// <summary>
        /// </summary>
        public IType ReadExceptionFromStackType { get; set; }

        /// <summary>
        /// used to adjust operand type
        /// </summary>
        public IType RequiredIncomingType { get; set; }

        /// <summary>
        /// used to adjust result of OpCode type
        /// </summary>
        public IType RequiredOutgoingType { get; set; }

        /// <summary>
        /// </summary>
        public FullyDefinedReference Result
        {
            get
            {
                if (this.result != null)
                {
                    return this.result;
                }

                if (this.Any(Code.Dup))
                {
                    return this.OpCodeOperands[0].Result;
                }

                return this.result;
            }

            set
            {
                ////if (value != null && this.result != null && this.Next != null && this.Next.Any(Code.Dup) && this.Next.OpCodeOperands[0].Equals(this))
                ////{
                ////    this.Next.Result = this.result;
                ////}
                
                this.result = value;
            }
        }

        /// <summary>
        /// TODO: you need to get rid of it when you stop allowing rewriting result of OpCode uncontrollablly (as you do in 'newarray' and etc)
        /// so OpCode should get only one result only once and second time whem 'cast' happends
        /// </summary>
        public FullyDefinedReference ResultWithDupShift
        {
            set
            {
                if (value != null && this.result != null && this.Next != null && this.Next.Any(Code.Dup) && this.Next.OpCodeOperands[0].Equals(this))
                {
                    this.Next.Result = this.result;
                }

                this.result = value;
            }
        }

        /// <summary>
        /// </summary>
        public List<TryClause> TryBegin { get; set; }

        /// <summary>
        /// </summary>
        public TryClause TryEnd { get; set; }

        /// <summary>
        /// </summary>
        public bool UseAsConditionalExpression { get; set; }

        /// <summary>
        /// </summary>
        public bool UseAsNull { get; set; }

        /// <summary>
        /// </summary>
        public UsedByInfo UsedBy { get; set; }
    }
}