// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class CSharpSyntaxTree
    {
        internal sealed class DummySyntaxTree : CSharpSyntaxTree
        {
            private readonly CompilationUnitSyntax node;

            public DummySyntaxTree()
            {
                node = this.CloneNodeAsRoot(SyntaxFactory.ParseCompilationUnit(string.Empty));
            }

            public override string ToString()
            {
                return string.Empty;
            }

            public override SourceText GetText(CancellationToken cancellationToken)
            {
                return SourceText.From(string.Empty, Encoding.UTF8);
            }

            public override bool TryGetText(out SourceText text)
            {
                text = SourceText.From(string.Empty, Encoding.UTF8);
                return true;
            }

            public override int Length
            {
                get { return 0; }
            }

            public override CSharpParseOptions Options
            {
                get { return CSharpParseOptions.Default; }
            }

            public override string FilePath
            {
                get { return string.Empty; }
            }

            public override SyntaxReference GetReference(SyntaxNode node)
            {
                return new SimpleSyntaxReference(node);
            }

            public override CSharpSyntaxNode GetRoot(CancellationToken cancellationToken)
            {
                return node;
            }

            public override bool TryGetRoot(out CSharpSyntaxNode root)
            {
                root = node;
                return true;
            }

            public override bool HasCompilationUnitRoot
            {
                get { return true; }
            }

            public override FileLinePositionSpan GetLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
            {
                return default(FileLinePositionSpan);
            }

            public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
            {
                return SyntaxFactory.SyntaxTree(root, options: options, path: FilePath, encoding: null);
            }

            public override SyntaxTree WithFilePath(string path)
            {
                return SyntaxFactory.SyntaxTree(this.node, options: this.Options, path: path, encoding: null);
            }
        }
    }
}
