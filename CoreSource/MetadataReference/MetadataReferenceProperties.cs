﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Information about a metadata reference.
    /// </summary>
    public struct MetadataReferenceProperties : IEquatable<MetadataReferenceProperties>
    {
        private readonly MetadataImageKind kind;
        private readonly ImmutableArray<string> aliases;
        private readonly bool embedInteropTypes;

        /// <summary>
        /// Default properties for a module reference.
        /// </summary>
        public static readonly MetadataReferenceProperties Module = new MetadataReferenceProperties(MetadataImageKind.Module);

        /// <summary>
        /// Default properties for an assembly reference.
        /// </summary>
        public static readonly MetadataReferenceProperties Assembly = new MetadataReferenceProperties(MetadataImageKind.Assembly);

        /// <summary>
        /// Initializes reference properties.
        /// </summary>
        /// <param name="kind">The image kind - assembly or module.</param>
        /// <param name="aliases">Assembly aliases. Can't be set for a module.</param>
        /// <param name="embedInteropTypes">True to embed interop types from the referenced assembly to the referencing compilation. Must be false for a module.</param>
        public MetadataReferenceProperties(MetadataImageKind kind = MetadataImageKind.Assembly, ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
        {
            if (!kind.IsValid())
            {
                throw new ArgumentOutOfRangeException("kind");
            }

            if (kind == MetadataImageKind.Module)
            {
                if (embedInteropTypes)
                {
                    throw new ArgumentException(CodeAnalysisResources.CannotEmbedInteropTypesFromModule, "embedInteropTypes");
                }

                if (!aliases.IsDefaultOrEmpty)
                {
                    throw new ArgumentException(CodeAnalysisResources.CannotAliasModule, "aliases");
                }
            }

            if (!aliases.IsDefaultOrEmpty)
            {
                foreach (var alias in aliases)
                {
                    if (!alias.IsValidClrTypeName())
                    {
                        throw new ArgumentException(CodeAnalysisResources.InvalidAlias, "aliases");
                    }
                }
            }

            this.kind = kind;
            this.aliases = aliases;
            this.embedInteropTypes = embedInteropTypes;
        }

        /// <summary>
        /// Returns <see cref="MetadataReferenceProperties"/> with specified aliases.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <see cref="Kind"/> is <see cref="MetadataImageKind.Module"/>, as modules can't be aliased.
        /// </exception>
        public MetadataReferenceProperties WithAliases(IEnumerable<string> aliases)
        {
            return WithAliases(aliases.AsImmutableOrEmpty());
        }

        /// <summary>
        /// Returns <see cref="MetadataReferenceProperties"/> with specified aliases.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <see cref="Kind"/> is <see cref="MetadataImageKind.Module"/>, as modules can't be aliased.
        /// </exception>
        public MetadataReferenceProperties WithAliases(ImmutableArray<string> aliases)
        {
            return new MetadataReferenceProperties(this.kind, aliases, this.EmbedInteropTypes);
        }

        /// <summary>
        /// Returns <see cref="MetadataReferenceProperties"/> with <see cref="EmbedInteropTypes"/> set to specified value.
        /// </summary>
        /// <exception cref="ArgumentException"><see cref="Kind"/> is <see cref="MetadataImageKind.Module"/>, as interop types can't be embedded from modules.</exception>
        public MetadataReferenceProperties WithEmbedInteropTypes(bool embedInteropTypes)
        {
            return new MetadataReferenceProperties(this.kind, this.aliases, embedInteropTypes);
        }

        /// <summary>
        /// The image kind (assembly or module) the reference refers to.
        /// </summary>
        public MetadataImageKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        /// Alias that represents a global declaration space.
        /// </summary>
        /// <remarks>
        /// Namespaces in references whose <see cref="Aliases"/> contain <see cref="GlobalAlias"/> are available in global declaration space.
        /// </remarks>
        public static readonly string GlobalAlias = "global";

        /// <summary>
        /// Aliases for the metadata reference, or default(<see cref="ImmutableArray"/>) if no aliases were specified.
        /// </summary>
        /// <remarks>
        /// In C# these aliases can be used in "extern alias" syntax to disambiguate type names. 
        /// </remarks>
        public ImmutableArray<string> Aliases
        {
            get { return aliases; }
        }

        /// <summary>
        /// True if interop types defined in the referenced metadata should be embedded into the compilation referencing the metadata.
        /// </summary>
        public bool EmbedInteropTypes
        {
            get { return embedInteropTypes; }
        }

        public override bool Equals(object obj)
        {
            return obj is MetadataReferenceProperties && Equals((MetadataReferenceProperties)obj);
        }

        public bool Equals(MetadataReferenceProperties other)
        {
            return this.aliases.NullToEmpty().SequenceEqual(other.aliases.NullToEmpty())
                && this.embedInteropTypes == other.embedInteropTypes
                && this.kind == other.kind;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Hash.CombineValues(this.aliases), Hash.Combine(this.embedInteropTypes, this.kind.GetHashCode()));
        }

        public static bool operator ==(MetadataReferenceProperties left, MetadataReferenceProperties right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MetadataReferenceProperties left, MetadataReferenceProperties right)
        {
            return !left.Equals(right);
        }
    }
}
