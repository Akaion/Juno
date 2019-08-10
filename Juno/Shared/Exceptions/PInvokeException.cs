using System;
using System.Runtime.InteropServices;

namespace Juno.Shared.Exceptions
{
    internal class PInvokeException : Exception
    {
        internal PInvokeException(string message) : base($"{message} with error code {Marshal.GetLastWin32Error()}") { }
    }
}