using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTaluva.Demo.Enums;

namespace VirtualTaluva.Demo.Models
{
    public class Land
    {
        public LandEnum LandType { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public PlayingTile ParentTile { get; }

        public Land(PlayingTile parent, LandEnum landType, int x, int y)
        {
            LandType = landType;
            X = x;
            Y = y;
            ParentTile = parent;
        }
    }
}
