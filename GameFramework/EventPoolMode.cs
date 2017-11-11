using System;

namespace GameFramework
{
    [Flags]
    internal enum EventPoolMode
    {
        Default = 0,
        AllowNoHandler = 1,
        AllowMultiHandler = 2,
        AllowDuplicateHandler = 4,
    }
}