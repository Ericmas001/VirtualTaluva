using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Com.Ericmas001.Windows;

namespace VirtualTaluva.Demo
{
    public class BoardTile : BaseViewModel
    {
        public static double XOffset { get; set; } = 100;
        public static double YOffset { get; set; } = 100;

        private readonly int m_X;
        private readonly int m_Y;
        public Thickness Margin =>  new Thickness((m_X - XOffset) * 69.282032 - (m_Y % 2 * 34.641016), (m_Y - YOffset) * 60, 0, 0);

        public BoardTile(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public void RefreshMargin() => RaisePropertyChanged(nameof(Margin));
    }
}
