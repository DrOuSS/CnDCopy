using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnDCopy.Kernel
{
    public enum ReplaceMode
    {
        UserAsking = 0,
        Replace = 1,
        ReplaceIfNewer = 2,
        ReplaceIfDifferentSize = 3,
        ReplaceIfNewerOrDifferentSize = 4,
        Resume = 5,
        Rename = 6,
        Ignore =7
    }
}
