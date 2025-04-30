using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Resources.Scripts.GUI
{
    [Flags]
    public enum ControlType
    {
        Container = 1 << 1,
        Button = 1 << 2
    }
}
