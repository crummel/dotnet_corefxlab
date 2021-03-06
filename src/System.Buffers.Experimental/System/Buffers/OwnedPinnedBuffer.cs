// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    // This is to support secnarios today covered by Buffer<T> in corefxlab
    public class OwnedPinnedBuffer<T> : ReferenceCountedBuffer<T>
    {
        public unsafe OwnedPinnedBuffer(T[] array, void* pointer, GCHandle handle = default(GCHandle))
        {
            var computedPointer = new IntPtr(Unsafe.AsPointer(ref array[0]));
            if (computedPointer != new IntPtr(pointer))
            {
                throw new InvalidOperationException();
            }
            _handle = handle;
            _pointer = new IntPtr(pointer);
            _array = array;
        }

        public unsafe OwnedPinnedBuffer(T[] array) : this(array, GCHandle.Alloc(array, GCHandleType.Pinned))
        { }

        private unsafe OwnedPinnedBuffer(T[] array, GCHandle handle) : this(array, handle.AddrOfPinnedObject().ToPointer(), handle)
        { }

        public static implicit operator OwnedPinnedBuffer<T>(T[] array) => new OwnedPinnedBuffer<T>(array);

        public unsafe static implicit operator IntPtr(OwnedPinnedBuffer<T> owner) => new IntPtr(owner.Pointer);

        public static implicit operator T[] (OwnedPinnedBuffer<T> owner) => owner.Array;

        public override int Length => _array.Length;

        public override Span<T> Span
        {
            get
            {
                if (IsDisposed) BuffersExperimentalThrowHelper.ThrowObjectDisposedException(nameof(OwnedPinnedBuffer<T>));
                return _array;
            }
        }

        public unsafe byte* Pointer => (byte*)_pointer.ToPointer();

        public T[] Array => _array;

        protected override void Dispose(bool disposing)
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
            _array = null;
            _pointer = IntPtr.Zero;
            base.Dispose(disposing);
        }

        public unsafe override BufferHandle Pin(int index = 0)
        {
            return new BufferHandle(this, Add(_pointer.ToPointer(), index));
        }

        protected override bool TryGetArrayInternal(out ArraySegment<T> buffer)
        {
            if (IsDisposed) BuffersExperimentalThrowHelper.ThrowObjectDisposedException(nameof(OwnedPinnedBuffer<T>));
            buffer = new ArraySegment<T>(_array);
            return true;
        }

        protected override unsafe bool TryGetPointerAt(int index, out void* pointer)
        {
            if (IsDisposed) BuffersExperimentalThrowHelper.ThrowObjectDisposedException(nameof(OwnedPinnedBuffer<T>));
            pointer = Add(_pointer.ToPointer(), index);
            return true;
        }

        private GCHandle _handle;
        IntPtr _pointer;
        T[] _array;
    }
}

