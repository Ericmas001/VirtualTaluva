using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Com.Ericmas001.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using VirtualTaluva.Demo.StuffOnTile;

namespace VirtualTaluva.Demo
{
    public class MainViewModel : BaseViewModel
    {
        public FastObservableCollection<AbstractStuffOnTile> StuffOnTile { get; } = new FastObservableCollection<AbstractStuffOnTile>
        {
            new LevelIndicator(LevelIndicator.TOP_MARGIN),
            new LevelIndicator(LevelIndicator.LEFT_MARGIN),
            new LevelIndicator(LevelIndicator.RIGHT_MARGIN)
        };

        private static readonly Thickness m_BaseMargin = new Thickness(103.923048, 70, 0, 0);
        private static readonly Size m_Step = new Size(69.282032, 60);
        private static readonly Dictionary<double, Thickness> m_RotationMarginModifier = new Dictionary<double, Thickness>
        {
            {0, new Thickness(0)},
            {60, new Thickness(-26.5,-15,0,0)},
            {120, new Thickness(7.5,15,0,0)},
            {180, new Thickness(-35.5,-1.5,0,0)},
            {240, new Thickness(-8.75,13.5,0,0)},
            {300, new Thickness(-43,-15.5,0,0)},
        };

        private double m_Scale = 1;
        private int m_CurrentPositionX = 0;
        private int m_CurrentPositionY = 0;
        private double m_Angle = 0;
        private Thickness m_CurrentMargin = new Thickness(m_BaseMargin.Left, m_BaseMargin.Top, m_BaseMargin.Right, m_BaseMargin.Bottom);
       
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

        public string ZoomValue => $"{m_Scale*100:0}%";
        public double AntiRotateAngle => 360 - m_Angle;

        private RelayCommand m_ZoomInCommand;
        public RelayCommand ZoomInCommand => m_ZoomInCommand ?? (m_ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn));

        private RelayCommand m_ZoomOutCommand;
        public RelayCommand ZoomOutCommand => m_ZoomOutCommand ?? (m_ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut));

        private RelayCommand m_RotateCommand;
        public RelayCommand RotateCommand => m_RotateCommand ?? (m_RotateCommand = new RelayCommand(OnRotate));

        private RelayCommand m_AntiRotateCommand;
        public RelayCommand AntiRotateCommand => m_AntiRotateCommand ?? (m_AntiRotateCommand = new RelayCommand(OnAntiRotate));

        private RelayCommand m_LeftCommand;
        public RelayCommand LeftCommand => m_LeftCommand ?? (m_LeftCommand = new RelayCommand(OnLeft, CanGoLeft));

        private RelayCommand m_RightCommand;
        public RelayCommand RightCommand => m_RightCommand ?? (m_RightCommand = new RelayCommand(OnRight, CanGoRight));

        private RelayCommand m_UpCommand;
        public RelayCommand UpCommand => m_UpCommand ?? (m_UpCommand = new RelayCommand(OnUp, CanGoUp));

        private RelayCommand m_DownCommand;
        public RelayCommand DownCommand => m_DownCommand ?? (m_DownCommand = new RelayCommand(OnDown, CanGoDown));


        public MainViewModel()
        {
            RecalculateMargin();
        }

        private void OnZoomIn()
        {
            Scale *= 1.25;
        }
        private bool CanZoomIn()
        {
            return Scale < 5.96;
        }
        private void OnZoomOut()
        {
            Scale /= 1.25;
        }
        private bool CanZoomOut()
        {
            return Scale > 0.33;
        }

        private void OnRotate()
        {
            RotateAngle = (RotateAngle + 60) % 360;
            RecalculateMargin();
        }
        private void OnAntiRotate()
        {
            RotateAngle = (RotateAngle + 300) % 360;
            RecalculateMargin();
        }
        private void OnLeft()
        {
            CurrentPositionX--;
            RecalculateMargin();
        }

        private bool CanGoLeft()
        {
            return CurrentPositionX > 0;
        }
        private void OnRight()
        {
            CurrentPositionX++;
            RecalculateMargin();
        }

        private bool CanGoRight()
        {
            return CurrentPositionX < 3;
        }
        private void OnUp()
        {
            CurrentPositionY--;
            RecalculateMargin();
        }

        private bool CanGoUp()
        {
            return CurrentPositionY > 0;
        }
        private void OnDown()
        {
            CurrentPositionY++;
            RecalculateMargin();
        }

        private bool CanGoDown()
        {
            return CurrentPositionY < 1;
        }

        private void RecalculateMargin()
        {
            var rotationModifier = m_RotationMarginModifier.ContainsKey(RotateAngle) ? m_RotationMarginModifier[RotateAngle] : new Thickness(0);
            var rowOffset = CurrentPositionY % 2 * (m_Step.Width / 2);
            if ((int) (RotateAngle / 60) % 2 == 0)
                rowOffset = 0 - rowOffset;
            CurrentMargin = new Thickness(m_BaseMargin.Left + rotationModifier.Left + rowOffset + (CurrentPositionX * m_Step.Width), m_BaseMargin.Top + rotationModifier.Top + (CurrentPositionY * m_Step.Height), m_BaseMargin.Right + rotationModifier.Right, m_BaseMargin.Bottom + rotationModifier.Bottom);

            foreach (var stuff in StuffOnTile)
                stuff.RecalculateMargin(RotateAngle);

        }
    }
}
