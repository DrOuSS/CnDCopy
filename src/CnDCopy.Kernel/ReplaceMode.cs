using System;

namespace CnDCopy.Kernel
{
    [Flags]
    public enum ReplaceMode
    {
        UserAsking = 0,
        Replace = 1,
        ReplaceIfNewer = 2,
        ReplaceIfDifferentSize = 4,
        Resume = 8,
        Rename = 16,
        Ignore = 32
    }
}
