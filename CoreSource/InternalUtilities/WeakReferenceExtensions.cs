﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Roslyn.Utilities
{
    // Helpers that are missing from Dev11 implementation:
    internal static class WeakReferenceExtensions
    {
        public static T GetTarget<T>(this WeakReference<T> reference) where T : class
        {
            T target;
            reference.TryGetTarget(out target);
            return target;
        }

        public static bool IsNull<T>(this WeakReference<T> reference) where T : class
        {
            T target;
            return !reference.TryGetTarget(out target);
        }
    }
}
