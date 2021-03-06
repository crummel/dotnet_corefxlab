// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;

namespace System.Buffers
{
    public struct DisposableReservation<T> : IDisposable
    {
        OwnedBuffer<T> _owner;

        internal DisposableReservation(OwnedBuffer<T> owner)
        {
            _owner = owner;
            _owner.Retain();
        }

        public Span<T> Span => _owner.Span;

        public void Dispose()
        {
            _owner.Release();
            _owner = null;
        }
    }
}
