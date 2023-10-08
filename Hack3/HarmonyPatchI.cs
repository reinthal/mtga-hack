using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack3
{
    internal interface HarmonyPatchI
    {
        void DoPatching();
        void UndoPatching();
    }
}
