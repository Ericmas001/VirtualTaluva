using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using Com.Ericmas001.Windows;
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

        private PlayingTileStateEnum m_State = PlayingTileStateEnum.ActiveCorrect;
        private int m_CurrentPositionX = MainViewModel.NB_TILES / 2;
        private int m_CurrentPositionY = MainViewModel.NB_TILES / 2;
        private double m_Angle;
        private Thickness m_CurrentMargin = new Thickness(m_BaseMargin.Left, m_BaseMargin.Top, m_BaseMargin.Right, m_BaseMargin.Bottom);


        public FastObservableCollection<AbstractStuffOnTile> StuffOnTile { get; } = new FastObservableCollection<AbstractStuffOnTile>
        {
            new LevelIndicator(LevelIndicator.TOP_MARGIN),
            new LevelIndicator(LevelIndicator.LEFT_MARGIN),
            new LevelIndicator(LevelIndicator.RIGHT_MARGIN)
        };
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


        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        private void RefreshState()
        {
            RaisePropertyChanged(nameof(StrokeColor));
            RaisePropertyChanged(nameof(StrokeThickness));
        }

        public void RecalculateMargin(Thickness bounds)
        {
            var rotationModifier = m_RotationMarginModifier.ContainsKey(RotateAngle) ? m_RotationMarginModifier[RotateAngle] : new Thickness(0);
            var rowOffset = CurrentPositionY % 2 * (MainViewModel.TILE_WIDTH / 2);
            var displayX = CurrentPositionX - bounds.Left;
            var displayY = CurrentPositionY - bounds.Top;
            if ((int)(RotateAngle / 60) % 2 == 0)
                rowOffset = 0 - rowOffset;
            CurrentMargin = new Thickness(m_BaseMargin.Left + rotationModifier.Left + rowOffset + (displayX * MainViewModel.TILE_WIDTH), m_BaseMargin.Top + rotationModifier.Top + (displayY * MainViewModel.TILE_HEIGHT), m_BaseMargin.Right + rotationModifier.Right, m_BaseMargin.Bottom + rotationModifier.Bottom);

            foreach (var stuff in StuffOnTile)
                stuff.RecalculateMargin(RotateAngle);

        }

        public void GoUp(Thickness bounds)
        {
            CurrentPositionY--;
            RecalculateMargin(bounds);
        }
        public void GoDown(Thickness bounds)
        {
            CurrentPositionY++;
            RecalculateMargin(bounds);
        }
        public void GoRight(Thickness bounds)
        {
            CurrentPositionX++;
            RecalculateMargin(bounds);
        }
        public void GoLeft(Thickness bounds)
        {
            CurrentPositionX--;
            RecalculateMargin(bounds);
        }
        public void RotateClockwise(Thickness bounds)
        {
            RotateAngle = (RotateAngle + 60) % 360;
            RecalculateMargin(bounds);
        }
        public void RotateCounterClockwise(Thickness bounds)
        {
            RotateAngle = (RotateAngle + 300) % 360;
            RecalculateMargin(bounds);
        }
    }
}
