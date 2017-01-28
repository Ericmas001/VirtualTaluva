using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Com.Ericmas001.Windows;
using Com.Ericmas001.Windows.ViewModels;
using VirtualTaluva.Demo.StuffOnTile;

namespace VirtualTaluva.Demo
{
    public enum PlayingTileStateEnum
    {
        Passive,
        ActiveCorrect,
        ActiveProblem
    }
    public class PlayingTile : BaseViewModel
    {
        public event EmptyHandler PositionChanged = delegate {};
        private readonly IBoard m_Board;
        private static readonly Thickness m_BaseMargin = new Thickness(MainViewModel.TILE_WIDTH, 10, 0, 0);
        private static readonly Dictionary<double, Thickness> m_RotationMarginModifier = new Dictionary<double, Thickness>
        {
            {0, new Thickness(0)},
            {60, new Thickness(-26.5,-15,0,0)},
            {120, new Thickness(7.5,15,0,0)},
            {180, new Thickness(-35.5,-1.5,0,0)},
            {240, new Thickness(-8.75,13.5,0,0)},
            {300, new Thickness(-43,-15.5,0,0)},
        };


        private Point[] m_CurrentPositions = new Point[0];
        public int Level { get; private set; }
        private PlayingTileStateEnum m_State = PlayingTileStateEnum.ActiveCorrect;
        private int m_CurrentPositionX;
        private int m_CurrentPositionY;
        private double m_Angle;
        private Thickness m_CurrentMargin = new Thickness(m_BaseMargin.Left, m_BaseMargin.Top, m_BaseMargin.Right, m_BaseMargin.Bottom);


        public FastObservableCollection<AbstractStuffOnTile> StuffOnTile { get; } = new FastObservableCollection<AbstractStuffOnTile>();

        public PlayingTileStateEnum State
        {
            get { return m_State; }
            set
            {
                Set(ref m_State, value);
                RefreshState();
            }
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public double RotateAngle
        {
            get { return m_Angle; }
            set
            {
                Set(ref m_Angle, value);
                RaisePropertyChanged(nameof(AntiRotateAngle));
            }
        }


        public Thickness CurrentMargin
        {
            get { return m_CurrentMargin; }
            set { Set(ref m_CurrentMargin, value); }
        }

        public int CurrentPositionX
        {
            get { return m_CurrentPositionX; }
            set { Set(ref m_CurrentPositionX, value); }
        }

        public int CurrentPositionY
        {
            get { return m_CurrentPositionY; }
            set { Set(ref m_CurrentPositionY, value); }
        }

        public Brush StrokeColor
        {
            get
            {
                switch (State)
                {
                    case PlayingTileStateEnum.ActiveCorrect:
                        return Brushes.Lime;
                    case PlayingTileStateEnum.ActiveProblem:
                        return Brushes.Red;
                }
                return Brushes.Black;
            }
        }
        public double StrokeThickness => State == PlayingTileStateEnum.Passive ? 1 : 3;
        public double AntiRotateAngle => 360 - m_Angle;


        public PlayingTile(IBoard board, int x, int y)
        {
            m_Board = board;
            CurrentPositionX = x;
            CurrentPositionY = y;
            RecalculatePositions();
        }

        private void RecalculatePositions()
        {
            if (IsPointingUp)
                if (IsOnOddRow)
                    m_CurrentPositions = new[]
                    {
                        new Point(CurrentPositionX + 1, CurrentPositionY - 1),
                        new Point(CurrentPositionX, CurrentPositionY),
                        new Point(CurrentPositionX + 1, CurrentPositionY),
                    };
                else
                    m_CurrentPositions = new[]
                    {
                        new Point(CurrentPositionX, CurrentPositionY - 1),
                        new Point(CurrentPositionX, CurrentPositionY),
                        new Point(CurrentPositionX + 1, CurrentPositionY),
                    };
            else if (IsOnOddRow)
                m_CurrentPositions = new[]
                {
                    new Point(CurrentPositionX, CurrentPositionY - 1),
                    new Point(CurrentPositionX + 1, CurrentPositionY - 1),
                    new Point(CurrentPositionX, CurrentPositionY),
                };
            else
                m_CurrentPositions = new[]
                {
                    new Point(CurrentPositionX, CurrentPositionY - 1),
                    new Point(CurrentPositionX + 1, CurrentPositionY - 1),
                    new Point(CurrentPositionX + 1, CurrentPositionY),
                };
            if (State != PlayingTileStateEnum.Passive)
            {
                if (m_CurrentPositions.Any(p => m_Board.BoardMatrix[(int) p.X, (int) p.Y] == null))
                    State = PlayingTileStateEnum.ActiveProblem;
                else if (m_CurrentPositions.Select(p => m_Board.BoardMatrix[(int) p.X, (int) p.Y].PlayingTiles.Count).Distinct().Count() == 1)
                {
                    if (m_CurrentPositions.All(p => !m_Board.BoardMatrix[(int) p.X, (int) p.Y].PlayingTiles.Any()))
                    {
                        if (m_Board.NbPlayingTiles == 1)
                            State = PlayingTileStateEnum.ActiveCorrect;
                        else
                        {
                            foreach (var p in CurrentPositions)
                            {
                                var pIsOnOddRow = (int)p.Y % 2 == 0;
                                var points = new List<Point>
                                {
                                    new Point(p.X - 1, p.Y),
                                    new Point(p.X + 1, p.Y),
                                    new Point(p.X, p.Y - 1),
                                    new Point(p.X, p.Y + 1),
                                    new Point(pIsOnOddRow ? p.X + 1 : p.X - 1, p.Y + 1),
                                    new Point(pIsOnOddRow ? p.X + 1 : p.X - 1, p.Y - 1),

                                };
                                if(points.Any(q => m_Board.BoardMatrix[(int)q.X, (int)q.Y] != null && m_Board.BoardMatrix[(int)q.X, (int)q.Y].PlayingTiles.Any()))
                                {
                                    State = PlayingTileStateEnum.ActiveCorrect;
                                    return;
                                }
                            }
                            State = PlayingTileStateEnum.ActiveProblem;
                        }
                    }
                    else if (m_CurrentPositions.Select(p => m_Board.BoardMatrix[(int) p.X, (int) p.Y].PlayingTiles.Last()).Distinct().Count() == 1)
                        State = PlayingTileStateEnum.ActiveProblem;
                    else
                        State = PlayingTileStateEnum.ActiveCorrect;
                }
                else
                    State = PlayingTileStateEnum.ActiveProblem;
            }
            PositionChanged();
        }


        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        private void RefreshState()
        {
            RaisePropertyChanged(nameof(StrokeColor));
            RaisePropertyChanged(nameof(StrokeThickness));
        }

        public bool IsPointingUp => (int)(RotateAngle / 60) % 2 == 0;
        public bool IsOnOddRow => CurrentPositionY % 2 == 0;

        public Point[] CurrentPositions => m_CurrentPositions;

        public void RecalculateMargin()
        {
            var rotationModifier = m_RotationMarginModifier.ContainsKey(RotateAngle) ? m_RotationMarginModifier[RotateAngle] : new Thickness(0);
            var rowOffset = CurrentPositionY % 2 * (MainViewModel.TILE_WIDTH / 2);
            var displayX = CurrentPositionX - m_Board.Bounds.Left;
            var displayY = CurrentPositionY - m_Board.Bounds.Top;
            if (IsPointingUp)
                rowOffset = 0 - rowOffset;
            CurrentMargin = new Thickness(m_BaseMargin.Left + rotationModifier.Left + rowOffset + (displayX * MainViewModel.TILE_WIDTH), m_BaseMargin.Top + rotationModifier.Top + (displayY * MainViewModel.TILE_HEIGHT), m_BaseMargin.Right + rotationModifier.Right, m_BaseMargin.Bottom + rotationModifier.Bottom);

            foreach (var stuff in StuffOnTile)
                stuff.RecalculateMargin(RotateAngle);

            RecalculatePositions();
        }

        public void GoUp()
        {
            CurrentPositionY--;
            RecalculateMargin();
        }
        public void GoDown()
        {
            CurrentPositionY++;
            RecalculateMargin();
        }
        public void GoRight()
        {
            CurrentPositionX++;
            RecalculateMargin();
        }
        public void GoLeft()
        {
            CurrentPositionX--;
            RecalculateMargin();
        }
        public void RotateClockwise()
        {
            RotateAngle = (RotateAngle + 60) % 360;
            RecalculateMargin();
        }
        public void RotateCounterClockwise()
        {
            RotateAngle = (RotateAngle + 300) % 360;
            RecalculateMargin();
        }

        public void PlaceOnBoard()
        {
            if (State != PlayingTileStateEnum.ActiveCorrect)
                return;

            State = PlayingTileStateEnum.Passive;

            var pos = CurrentPositions.First();
            Level = m_Board.BoardMatrix[(int)pos.X, (int)pos.Y].PlayingTiles.Count + 1;

            StuffOnTile.AddItems(new List<AbstractStuffOnTile>
            {
                new LevelIndicator(LevelIndicator.TOP_MARGIN, Level),
                new LevelIndicator(LevelIndicator.LEFT_MARGIN, Level),
                new LevelIndicator(LevelIndicator.RIGHT_MARGIN, Level)
            });

            foreach (var p in m_CurrentPositions)
                m_Board.BoardMatrix[(int)p.X, (int)p.Y].PlayingTiles.Add(this);
        }
    }
}
