using System;
using System.Diagnostics.CodeAnalysis;
using Com.Ericmas001.Windows;
using GalaSoft.MvvmLight.CommandWpf;

namespace VirtualTaluva.Demo
{
    public class MainViewModel : BaseViewModel
    {
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
        public RelayCommand ZoomInCommand => m_ZoomInCommand ?? (m_ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn));

        private RelayCommand m_ZoomOutCommand;
        public RelayCommand ZoomOutCommand => m_ZoomOutCommand ?? (m_ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut));
        

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
    }
}
