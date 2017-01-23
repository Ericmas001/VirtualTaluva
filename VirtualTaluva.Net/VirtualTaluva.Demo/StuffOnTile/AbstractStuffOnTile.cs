using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Ericmas001.Windows;

namespace VirtualTaluva.Demo.StuffOnTile
{
    public abstract class AbstractStuffOnTile : BaseViewModel
    {
        public abstract void RecalculateMargin(double rotateAngle);
    }
}
