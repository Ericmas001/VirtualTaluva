using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Com.Ericmas001.Windows;
using GalaSoft.MvvmLight.CommandWpf;

namespace VirtualTaluva.Demo
{
    public class MainViewModel : BaseViewModel
    {
        public const int NB_TILES = 200;
        public const double TILE_WIDTH = 69.282032;
        public const double TILE_HEIGHT = 60;

        public PlayingTile CurrentTile { get; private set; }

        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        private Thickness m_Bounds = new Thickness(NB_TILES/2, NB_TILES / 2, NB_TILES / 2, NB_TILES / 2);

        private readonly BoardTile[,] m_Board = new BoardTile[NB_TILES, NB_TILES];

        public FastObservableCollection<BoardTile> Board { get; } = new FastObservableCollection<BoardTile>();
        public FastObservableCollection<PlayingTile> PlayingTiles { get; } = new FastObservableCollection<PlayingTile>();

        private double m_Scale = 1;
       
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public double Scale
        {
            get { return m_Scale; }
            set
            {
                Set(ref m_Scale, value);
                RaisePropertyChanged(nameof(ZoomValue));
            }
        }

        public string ZoomValue => $"{m_Scale*100:0}%";

        private RelayCommand m_ZoomInCommand;
        public RelayCommand ZoomInCommand => m_ZoomInCommand ?? (m_ZoomInCommand = new RelayCommand(() => Scale *= 1.25, () => Scale < 5.96));

        private RelayCommand m_ZoomOutCommand;
        public RelayCommand ZoomOutCommand => m_ZoomOutCommand ?? (m_ZoomOutCommand = new RelayCommand(() => Scale /= 1.25, () => Scale > 0.33));

        private RelayCommand m_RotateCommand;
        public RelayCommand RotateCommand => m_RotateCommand ?? (m_RotateCommand = new RelayCommand(() => CurrentTile.RotateClockwise(m_Bounds)));

        private RelayCommand m_AntiRotateCommand;
        public RelayCommand AntiRotateCommand => m_AntiRotateCommand ?? (m_AntiRotateCommand = new RelayCommand(() => CurrentTile.RotateCounterClockwise(m_Bounds)));

        private RelayCommand m_LeftCommand;
        public RelayCommand LeftCommand => m_LeftCommand ?? (m_LeftCommand = new RelayCommand(() => CurrentTile.GoLeft(m_Bounds), () =>CurrentTile.CurrentPositionX > m_Bounds.Left + 1));

        private RelayCommand m_RightCommand;
        public RelayCommand RightCommand => m_RightCommand ?? (m_RightCommand = new RelayCommand(() => CurrentTile.GoRight(m_Bounds), () => CurrentTile.CurrentPositionX < m_Bounds.Right - 1));

        private RelayCommand m_UpCommand;
        public RelayCommand UpCommand => m_UpCommand ?? (m_UpCommand = new RelayCommand(() => CurrentTile.GoUp(m_Bounds), () => CurrentTile.CurrentPositionY > m_Bounds.Top + 2));

        private RelayCommand m_DownCommand;
        public RelayCommand DownCommand => m_DownCommand ?? (m_DownCommand = new RelayCommand(() => CurrentTile.GoDown(m_Bounds), () => CurrentTile.CurrentPositionY < m_Bounds.Bottom));

        private RelayCommand m_MoreLeftCommand;
        public RelayCommand MoreLeftCommand => m_MoreLeftCommand ?? (m_MoreLeftCommand = new RelayCommand(() => AddBoardTileColumn(AddBoardTileLeft), () => m_Bounds.Left > 0));

        private RelayCommand m_MoreRightCommand;
        public RelayCommand MoreRightCommand => m_MoreRightCommand ?? (m_MoreRightCommand = new RelayCommand(() => AddBoardTileColumn(AddBoardTileRight), () => m_Bounds.Right < NB_TILES));

        private RelayCommand m_MoreUpCommand;
        public RelayCommand MoreUpCommand => m_MoreUpCommand ?? (m_MoreUpCommand = new RelayCommand(() => AddBoardTileRow((int)m_Bounds.Top), () => m_Bounds.Top > 0));

        private RelayCommand m_MoreDownCommand;
        public RelayCommand MoreDownCommand => m_MoreDownCommand ?? (m_MoreDownCommand = new RelayCommand(() => AddBoardTileRow((int)m_Bounds.Bottom + 1), () => m_Bounds.Bottom < NB_TILES));

        private RelayCommand m_AcceptCommand;
        public RelayCommand AcceptCommand => m_AcceptCommand ?? (m_AcceptCommand = new RelayCommand(Accept, () => CurrentTile.State == PlayingTileStateEnum.ActiveCorrect));
        
        public MainViewModel()
        {
            for (int i = 99; i < 101; ++i)
                for (int k = 0; k < 1; ++k)
                {
                    AddBoardTileLeft(i);
                    AddBoardTileRight(i);
                }
            PlayingTiles.Add(CurrentTile = new PlayingTile());
            RefreshBoard();
        }

        private void AddBoardTileLeft(int row)
        {
            int i = 100;

            for (; i >= 0 && m_Board[i, row] != null; --i)
            {
                //Do nothing, just navigate thru the board !
            }

            if (i >= 0)
                AddBoardTile(row, i);
        }

        private void AddBoardTileRight(int row)
        {
            int i = 100;

            for (; i < 200 && m_Board[i, row] != null; ++i)
            {
                //Do nothing, just navigate thru the board !
            }

            if (i < 200)
                AddBoardTile(row, i);
        }

        private void AddBoardTile(int row, int i)
        {
            if (i <= m_Bounds.Left)
            {
                m_Bounds.Left = i - 1;
                BoardTile.XOffset = m_Bounds.Left;
            }

            if (i > m_Bounds.Right)
                m_Bounds.Right = i;

            if (row <= m_Bounds.Top)
            {
                m_Bounds.Top = row - 1;
                BoardTile.YOffset = m_Bounds.Top;
            }

            if (row > m_Bounds.Bottom)
                m_Bounds.Bottom = row;

            var bt = new BoardTile(i, row);
            Board.Add(bt);
            m_Board[i, row] = bt;
        }

        private void AddBoardTileRow(int row)
        {
            for (int i = 100; i > m_Bounds.Left; i--)
                AddBoardTileLeft(row);

            for (int i = 100; i < m_Bounds.Right; i++)
                AddBoardTileRight(row);

            RefreshBoard();
        }

        private void AddBoardTileColumn(Action<int> func)
        {
            for (int i = (int)m_Bounds.Top + 1; i <= m_Bounds.Bottom; i++)
                func(i);

            RefreshBoard();
        }

        private void RefreshBoard()
        {
            foreach (var boardTile in Board)
                boardTile.RefreshMargin();

            CurrentTile.RecalculateMargin(m_Bounds);
        }
        private void Accept()
        {
            CurrentTile.State = PlayingTileStateEnum.Passive;
            PlayingTiles.Add(CurrentTile = new PlayingTile());
            RefreshBoard();
        }
    }
}
