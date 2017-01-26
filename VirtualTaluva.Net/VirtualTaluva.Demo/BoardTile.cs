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
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        public static double XOffset { get; set; } = MainViewModel.NB_TILES / 2;

        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        public static double YOffset { get; set; } = MainViewModel.NB_TILES / 2;

        private readonly int m_X;
        private readonly int m_Y;
        public Thickness Margin =>  new Thickness((m_X - XOffset) * MainViewModel.TILE_WIDTH - (m_Y % 2 * (MainViewModel.TILE_WIDTH / 2)), (m_Y - YOffset) * MainViewModel.TILE_HEIGHT, 0, 0);

        public BoardTile(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public void RefreshMargin() => RaisePropertyChanged(nameof(Margin));
    }
}
