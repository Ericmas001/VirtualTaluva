using System.Windows;

namespace VirtualTaluva.Demo
{
    public interface IBoard
    {
        Thickness Bounds { get; }
        BoardTile[,] BoardMatrix { get; }
        int NbPlayingTiles { get; }
    }
}
