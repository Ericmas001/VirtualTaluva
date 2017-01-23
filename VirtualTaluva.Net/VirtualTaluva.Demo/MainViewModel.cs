using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Com.Ericmas001.Windows;
using GalaSoft.MvvmLight.CommandWpf;

namespace VirtualTaluva.Demo
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly Thickness m_BaseMargin = new Thickness(103.923048, 70, 0, 0);
        private static readonly Thickness m_BaseMarginTopNumber = new Thickness(0, -75, 0, 0);
        private static readonly Thickness m_BaseMarginLeftNumber = new Thickness(-72, 58, 0, 0);
        private static readonly Thickness m_BaseMarginRightNumber = new Thickness(75, 58, 0, 0);
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
        private static readonly Dictionary<double, Thickness> m_RotationMarginTopNumberModifier = new Dictionary<double, Thickness>
        {
            {0, new Thickness(0)},
            {60, new Thickness(-5,15,0,0)},
            {120, new Thickness(0,25,0,0)},
            {180, new Thickness(20,20,0,0)},
            {240, new Thickness(20,10,0,0)},
            {300, new Thickness(10,0,0,0)},
        };

        private double m_Scale = 1;
        private int m_CurrentPositionX = 0;
        private int m_CurrentPositionY = 0;
        private double m_Angle = 0;
        private Thickness m_CurrentMargin = new Thickness(m_BaseMargin.Left, m_BaseMargin.Top, m_BaseMargin.Right, m_BaseMargin.Bottom);
        private Thickness m_CurrentMarginTopNumber = new Thickness(m_BaseMarginTopNumber.Left, m_BaseMarginTopNumber.Top, m_BaseMarginTopNumber.Right, m_BaseMarginTopNumber.Bottom);
        private Thickness m_CurrentMarginLeftNumber = new Thickness(m_BaseMarginLeftNumber.Left, m_BaseMarginLeftNumber.Top, m_BaseMarginLeftNumber.Right, m_BaseMarginLeftNumber.Bottom);
        private Thickness m_CurrentMarginRightNumber = new Thickness(m_BaseMarginRightNumber.Left, m_BaseMarginRightNumber.Top, m_BaseMarginRightNumber.Right, m_BaseMarginRightNumber.Bottom);

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
        public Thickness CurrentMarginTopNumber
        {
            get { return m_CurrentMarginTopNumber; }
            set { Set(ref m_CurrentMarginTopNumber, value); }
        }
        public Thickness CurrentMarginLeftNumber
        {
            get { return m_CurrentMarginLeftNumber; }
            set { Set(ref m_CurrentMarginLeftNumber, value); }
        }
        public Thickness CurrentMarginRightNumber
        {
            get { return m_CurrentMarginRightNumber; }
            set { Set(ref m_CurrentMarginRightNumber, value); }
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

            var rotationModifierTopNumber = m_RotationMarginTopNumberModifier.ContainsKey(RotateAngle) ? m_RotationMarginTopNumberModifier[RotateAngle] : new Thickness(0);
            CurrentMarginTopNumber = new Thickness(m_BaseMarginTopNumber.Left + rotationModifierTopNumber.Left, m_BaseMarginTopNumber.Top + rotationModifierTopNumber.Top, m_BaseMarginTopNumber.Right + rotationModifierTopNumber.Right, m_BaseMarginTopNumber.Bottom + rotationModifierTopNumber.Bottom);
            CurrentMarginLeftNumber = new Thickness(m_BaseMarginLeftNumber.Left + rotationModifierTopNumber.Left, m_BaseMarginLeftNumber.Top + rotationModifierTopNumber.Top, m_BaseMarginLeftNumber.Right + rotationModifierTopNumber.Right, m_BaseMarginLeftNumber.Bottom + rotationModifierTopNumber.Bottom);
            CurrentMarginRightNumber = new Thickness(m_BaseMarginRightNumber.Left + rotationModifierTopNumber.Left, m_BaseMarginRightNumber.Top + rotationModifierTopNumber.Top, m_BaseMarginRightNumber.Right + rotationModifierTopNumber.Right, m_BaseMarginRightNumber.Bottom + rotationModifierTopNumber.Bottom);

        }
    }
}
