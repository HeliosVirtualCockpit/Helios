using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GadrocsWorkshop.Helios.Effects
{
    public class ColorAdjustEffect : ShaderEffect
    {
        private static readonly PixelShader _shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Helios;component/Resources/ColorAdjust.psc")
        };

        public ColorAdjustEffect()
        {
            PixelShader = _shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
            UpdateShaderValue(GammaProperty);
            UpdateShaderValue(RedFactorProperty);
            UpdateShaderValue(GreenFactorProperty);
            UpdateShaderValue(BlueFactorProperty);
        }

        // Input brush
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorAdjustEffect), 0);

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        // Brightness
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        // Contrast
        public static readonly DependencyProperty ContrastProperty =
            DependencyProperty.Register("Contrast", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public double Contrast
        {
            get => (double)GetValue(ContrastProperty);
            set => SetValue(ContrastProperty, value);
        }

        // Gamma
        public static readonly DependencyProperty GammaProperty =
            DependencyProperty.Register("Gamma", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(2)));

        public double Gamma
        {
            get => (double)GetValue(GammaProperty);
            set => SetValue(GammaProperty, value);
        }

        // RedFactor
        public static readonly DependencyProperty RedFactorProperty =
            DependencyProperty.Register("RedFactor", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(3)));

        public double RedFactor
        {
            get => (double)GetValue(RedFactorProperty);
            set => SetValue(RedFactorProperty, value);
        }

        // GreenFactor
        public static readonly DependencyProperty GreenFactorProperty =
            DependencyProperty.Register("GreenFactor", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(4)));

        public double GreenFactor
        {
            get => (double)GetValue(GreenFactorProperty);
            set => SetValue(GreenFactorProperty, value);
        }

        // BlueFactor
        public static readonly DependencyProperty BlueFactorProperty =
            DependencyProperty.Register("BlueFactor", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(5)));

        public double BlueFactor
        {
            get => (double)GetValue(BlueFactorProperty);
            set => SetValue(BlueFactorProperty, value);
        }
    }
}
