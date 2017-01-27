using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Com.Ericmas001.Windows;

namespace VirtualTaluva.Demo
{
    public class BoardTile : BaseViewModel
    {
        public static double XOffset { get; set; } = MainViewModel.NB_TILES / 2.0;
        public static double YOffset { get; set; } = MainViewModel.NB_TILES / 2.0;

        private readonly int m_X;
        private readonly int m_Y;
        private readonly IBoard m_Board;

        public Thickness Margin =>  new Thickness((m_X - XOffset) * MainViewModel.TILE_WIDTH - (m_Y % 2 * (MainViewModel.TILE_WIDTH / 2)), (m_Y - YOffset) * MainViewModel.TILE_HEIGHT, 0, 0);

        public FastObservableCollection<PlayingTile> PlayingTiles { get; } = new FastObservableCollection<PlayingTile>();
        
        public BoardTile(IBoard board, int x, int y)
        {
            m_X = x;
            m_Y = y;
            m_Board = board;
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public void RefreshMargin() => RaisePropertyChanged(nameof(Margin));
    }
}