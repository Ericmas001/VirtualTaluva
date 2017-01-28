using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Com.Ericmas001.Common;
using Com.Ericmas001.Windows;
using Com.Ericmas001.Windows.ViewModels;
using VirtualTaluva.Demo.Enums;
using VirtualTaluva.Demo.Models.StuffOnTile;

namespace VirtualTaluva.Demo.Models
{
    public enum PlayingTileStateEnum
    {
        Passive,
        ActiveCorrect,
        ActiveProblem
    }
    public class PlayingTile : BaseViewModel
    {
        public event EmptyHandler PositionChanged = delegate { };
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

        public Land Volcano { get; }
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
            private set { Set(ref m_CurrentPositionX, value); }
        }

        public int CurrentPositionY
        {
            get { return m_CurrentPositionY; }
            private set { Set(ref m_CurrentPositionY, value); }
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
        public Color TopColor => ColorFromLand(0);
        public Color LeftColor => ColorFromLand(2);
        public Color RightColor => ColorFromLand(1);

        private Color ColorFromLand(int i)
        {
            var convertFromString = ColorConverter.ConvertFromString(Lands[i].LandType.Color());
            if (convertFromString != null)
                return (Color)convertFromString;
            return Colors.Black;
        }

        public PlayingTile(IBoard board, LandEnum landLeft, LandEnum landRight)
        {
            m_Board = board;
            Lands = new[]
            {
                new Land(this, LandEnum.Volcano, CurrentPositionX, CurrentPositionY - 1),
                new Land(this, landLeft, CurrentPositionX, CurrentPositionY),
                new Land(this, landRight, CurrentPositionX + 1, CurrentPositionY)
            };
            Volcano = Lands.First(l => l.LandType == LandEnum.Volcano);
        }

        private void RecalculatePositions()
        {
            if (IsPointingUp)
            {
                Land[] landsFromUpperLeftCorner =
                {
                    Lands[((int) RotateAngle / 120 + 0) % 3],
                    Lands[((int) RotateAngle / 120 + 1) % 3],
                    Lands[((int) RotateAngle / 120 + 2) % 3],
                };

                if (IsOnOddRow)
                {
                    landsFromUpperLeftCorner[0].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[0].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[1].X = CurrentPositionX;
                    landsFromUpperLeftCorner[1].Y = CurrentPositionY;

                    landsFromUpperLeftCorner[2].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[2].Y = CurrentPositionY;
                }
                else
                {
                    landsFromUpperLeftCorner[0].X = CurrentPositionX;
                    landsFromUpperLeftCorner[0].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[1].X = CurrentPositionX;
                    landsFromUpperLeftCorner[1].Y = CurrentPositionY;

                    landsFromUpperLeftCorner[2].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[2].Y = CurrentPositionY;
                }
            }
            else
            {
                Land[] landsFromUpperLeftCorner =
                {
                    Lands[((int) RotateAngle / 120 + 1) % 3],
                    Lands[((int) RotateAngle / 120 + 0) % 3],
                    Lands[((int) RotateAngle / 120 + 2) % 3],
                };

                if (IsOnOddRow)
                {
                    landsFromUpperLeftCorner[0].X = CurrentPositionX;
                    landsFromUpperLeftCorner[0].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[1].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[1].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[2].X = CurrentPositionX;
                    landsFromUpperLeftCorner[2].Y = CurrentPositionY;
                }
                else
                {
                    landsFromUpperLeftCorner[0].X = CurrentPositionX;
                    landsFromUpperLeftCorner[0].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[1].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[1].Y = CurrentPositionY - 1;

                    landsFromUpperLeftCorner[2].X = CurrentPositionX + 1;
                    landsFromUpperLeftCorner[2].Y = CurrentPositionY;
                }
            }
            if (State != PlayingTileStateEnum.Passive)
            {
                //If there is no board tile under every land, PROBLEM
                if (Lands.Any(p => m_Board.BoardMatrix[p.X, p.Y] == null))
                    State = PlayingTileStateEnum.ActiveProblem;

                //If every land would be at the same level, it's a good start
                else if (Lands.Select(p => m_Board.BoardMatrix[p.X, p.Y].Lands.Count).Distinct().Count() == 1)
                {

                    //If spaces are empty under every land, there is a chance it would be good
                    if (Lands.All(p => !m_Board.BoardMatrix[p.X, p.Y].Lands.Any()))
                    {

                        // If this is the first tile, it's clearly CORRECT
                        if (m_Board.NbPlayingTiles == 1)
                            State = PlayingTileStateEnum.ActiveCorrect;

                        // If this is not the first tile, there are some placement rules
                        else
                        {
                            //We need to find at least 1 land that is touching one already on board
                            foreach (var p in Lands)
                            {
                                var pIsOnOddRow = p.Y % 2 == 0;
                                var points = new List<Point>
                                {
                                    new Point(p.X - 1, p.Y),
                                    new Point(p.X + 1, p.Y),
                                    new Point(p.X, p.Y - 1),
                                    new Point(p.X, p.Y + 1),
                                    new Point(pIsOnOddRow ? p.X + 1 : p.X - 1, p.Y + 1),
                                    new Point(pIsOnOddRow ? p.X + 1 : p.X - 1, p.Y - 1),

                                };

                                //If there is one land already on board touching the land, CORRECT
                                if (points.Any(q => m_Board.BoardMatrix[(int)q.X, (int)q.Y] != null && m_Board.BoardMatrix[(int)q.X, (int)q.Y].Lands.Any()))
                                {
                                    State = PlayingTileStateEnum.ActiveCorrect;
                                    return;
                                }
                            }

                            //There was no land touching one already on the board, PROBLEM
                            State = PlayingTileStateEnum.ActiveProblem;
                        }
                    }

                    //There is same level land under each land, but they're all from the same tile, PROBLEM 
                    else if (Lands.Select(p => m_Board.BoardMatrix[p.X, p.Y].Lands.Last().ParentTile).Distinct().Count() == 1)
                        State = PlayingTileStateEnum.ActiveProblem;

                    //There is same level land under each land, and they're not all from the same tile, it's a good start
                    else
                    {
                        // If volcano is over another volcano, CORRECT, or else it's a PROBLEM
                        State = m_Board.BoardMatrix[Volcano.X,Volcano.Y].Lands.Last().LandType == LandEnum.Volcano ? PlayingTileStateEnum.ActiveCorrect : PlayingTileStateEnum.ActiveProblem;
                    }
                }

                //All the lands are not at the same level, PROBLEM
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

        public Land[] Lands { get; }

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

            Level = m_Board.BoardMatrix[Volcano.X, Volcano.Y].Lands.Count + 1;

            StuffOnTile.AddItems(new List<AbstractStuffOnTile>
            {
                new LevelIndicator(LevelIndicator.TOP_MARGIN, Level),
                new LevelIndicator(LevelIndicator.LEFT_MARGIN, Level),
                new LevelIndicator(LevelIndicator.RIGHT_MARGIN, Level)
            });

            foreach (var p in Lands)
                m_Board.BoardMatrix[p.X, p.Y].Lands.Add(p);
        }
        public void PlaceOnBoard(int x, int y)
        {
            CurrentPositionX = x;
            CurrentPositionY = y;
            RecalculateMargin();
        }
    }
}
