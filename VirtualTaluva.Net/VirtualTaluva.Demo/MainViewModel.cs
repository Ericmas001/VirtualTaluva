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
        private static readonly Thickness m_BaseMargin = new Thickness(173.20508, 70, 0, 0);

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
        private double m_Angle = 0;
        private Thickness m_CurrentMargin= new Thickness(m_BaseMargin.Left, m_BaseMargin.Top, m_BaseMargin.Right, m_BaseMargin.Bottom);

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

        public double RotateAngle
        {
            get { return m_Angle; }
            set { Set(ref m_Angle, value); }
        }

        public Thickness CurrentMargin
        {
            get { return m_CurrentMargin; }
            set { Set(ref m_CurrentMargin, value); }
        }
        public string ZoomValue => $"{m_Scale*100:0}%";

        private RelayCommand m_ZoomInCommand;
        public RelayCommand ZoomInCommand => m_ZoomInCommand ?? (m_ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn));

        private RelayCommand m_ZoomOutCommand;
        public RelayCommand ZoomOutCommand => m_ZoomOutCommand ?? (m_ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut));

        private RelayCommand m_RotateCommand;
        public RelayCommand RotateCommand => m_RotateCommand ?? (m_RotateCommand = new RelayCommand(OnRotate));

        private RelayCommand m_AntiRotateCommand;
        public RelayCommand AntiRotateCommand => m_AntiRotateCommand ?? (m_AntiRotateCommand = new RelayCommand(OnAntiRotate));

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

        private void RecalculateMargin()
        {
            var rotationModifier = m_RotationMarginModifier.ContainsKey(RotateAngle) ? m_RotationMarginModifier[RotateAngle] : new Thickness(0);
            CurrentMargin = new Thickness(m_BaseMargin.Left + rotationModifier.Left, m_BaseMargin.Top + rotationModifier.Top, m_BaseMargin.Right + rotationModifier.Right, m_BaseMargin.Bottom + rotationModifier.Bottom);
        }
    }
}
