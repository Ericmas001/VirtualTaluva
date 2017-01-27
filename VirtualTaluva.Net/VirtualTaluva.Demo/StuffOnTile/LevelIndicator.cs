using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace VirtualTaluva.Demo.StuffOnTile
{
    public class LevelIndicator : AbstractStuffOnTile
    {
        public int Level { get; }
        public static readonly Thickness TOP_MARGIN = new Thickness(0, -75, 0, 0);
        public static readonly Thickness LEFT_MARGIN = new Thickness(-72, 58, 0, 0);
        public static readonly Thickness RIGHT_MARGIN = new Thickness(75, 58, 0, 0);
        private static readonly Dictionary<double, Thickness> m_RotationMarginModifier = new Dictionary<double, Thickness>
        {
            {0, new Thickness(0)},
            {60, new Thickness(-5,15,0,0)},
            {120, new Thickness(0,25,0,0)},
            {180, new Thickness(20,20,0,0)},
            {240, new Thickness(20,10,0,0)},
            {300, new Thickness(10,0,0,0)},
        };

        private readonly Thickness m_BaseMargin;
        private Thickness m_CurrentMargin;
        private double m_Angle;

        public Thickness CurrentMargin
        {
            get { return m_CurrentMargin; }
            set { Set(ref m_CurrentMargin, value); }
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
        public double AntiRotateAngle => 360 - m_Angle;

        public LevelIndicator(Thickness baseMargin, int level)
        {
            Level = level;
            m_BaseMargin = baseMargin;
        }

        public override void RecalculateMargin(double rotateAngle)
        {
            RotateAngle = rotateAngle;
            var rotationModifier = m_RotationMarginModifier.ContainsKey(RotateAngle) ? m_RotationMarginModifier[RotateAngle] : new Thickness(0);
            CurrentMargin = new Thickness(m_BaseMargin.Left + rotationModifier.Left, m_BaseMargin.Top + rotationModifier.Top, m_BaseMargin.Right + rotationModifier.Right, m_BaseMargin.Bottom + rotationModifier.Bottom);
        }
    }
}
