using System.Windows;

namespace VirtualTaluva.Demo.Models
{
    public interface IBoard
    {
        Thickness Bounds { get; }
        BoardTile[,] BoardMatrix { get; }
        int NbPlayingTiles { get; }
    }
}
