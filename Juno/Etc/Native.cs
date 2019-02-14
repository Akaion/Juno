using System;
using System.Runtime.InteropServices;

namespace Juno.Etc
{
    internal static class Native
    {
        #region pinvoke
        
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtect(IntPtr address, uint size, uint newProtect, out uint oldProtect);
        
        #endregion
        
        #region Enumerations
        
        [Flags]
        internal enum MemoryProtection
        {
            ExecuteReadWrite = 0x040,
        }
        
        #endregion
    }
}