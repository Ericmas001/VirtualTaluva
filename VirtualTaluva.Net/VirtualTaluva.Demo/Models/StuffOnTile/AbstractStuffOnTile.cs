﻿using Com.Ericmas001.Windows;

namespace VirtualTaluva.Demo.Models.StuffOnTile
{
    public abstract class AbstractStuffOnTile : BaseViewModel
    {
        public abstract void RecalculateMargin(double rotateAngle);
    }
}
