﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    partial class SyntaxParser
    {
        protected struct ResetPoint
        {
            internal readonly int ResetCount;
            internal readonly LexerMode Mode;
            internal readonly int Position;
            internal readonly CSharpSyntaxNode PrevTokenTrailingTrivia;

            internal ResetPoint(int resetCount, LexerMode mode, int position, CSharpSyntaxNode prevTokenTrailingTrivia)
            {
                this.ResetCount = resetCount;
                this.Mode = mode;
                this.Position = position;
                this.PrevTokenTrailingTrivia = prevTokenTrailingTrivia;
            }
        }
    }
}