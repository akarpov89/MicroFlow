using System.Runtime.InteropServices;

namespace MicroFlow
{
    [StructLayout(LayoutKind.Auto)]
    public struct Unit
    {
        public static readonly Unit Instance;
    }
}