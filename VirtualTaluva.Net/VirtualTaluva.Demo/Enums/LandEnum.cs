using System.Windows.Media;
using Com.Ericmas001.Common;
using Com.Ericmas001.Common.Attributes;

namespace VirtualTaluva.Demo.Enums
{
    public enum LandEnum
    {
        [Color("OrangeRed")]
        Volcano,

        [Color("DarkGreen")]
        Jungle,

        [Color("Chartreuse")]
        Grass,

        [Color("Moccasin")]
        Desert,

        [Color("Gray")]
        Quarry,

        [Color("DodgerBlue")]
        Lagoon
    }
    public static class LandEnumExtension
    {
        public static Color WindowsColor(this LandEnum e)
        {
            var convertFromString = ColorConverter.ConvertFromString(e.Color());
            if (convertFromString != null)
                return (Color)convertFromString;
            return Colors.Black;
        }
        public static Brush WindowsBrush(this LandEnum e)
        {
            return new SolidColorBrush(e.WindowsColor());
        }
    }
}
