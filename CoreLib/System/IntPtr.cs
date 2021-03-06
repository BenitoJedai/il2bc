////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Apache License 2.0 (Apache)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace System
{

    using System;

    [Serializable]
    public struct IntPtr
    {
        unsafe private void* _value;

        public static readonly IntPtr Zero;

        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        public unsafe IntPtr(int value)
        {
            _value = (void*)value;
        }

        public unsafe void* ToPointer()
        {
            return _value;
        }
    }
}


