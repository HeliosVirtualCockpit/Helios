using AvalonDock.Converters;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GadrocsWorkshop.Helios.Effects
{
    public class ColorAdjustEffect : ShaderEffect
    {
        private bool _enabled = true;
        private string _shaderUri;
        private static PixelShader _shader;

        public ColorAdjustEffect()
        {

            PixelShader = _shader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(RedAdjustProperty);
            UpdateShaderValue(GreenAdjustProperty);
            UpdateShaderValue(BlueAdjustProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
            UpdateShaderValue(GammaProperty);
            UpdateShaderValue(ShadowStrengthProperty);
            UpdateShaderValue(HighlightStrengthProperty);
            UpdateShaderValue(MidtoneBalanceProperty);
            UpdateShaderValue(EnableCurveProperty);
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
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        // Contrast
        public static readonly DependencyProperty ContrastProperty =
            DependencyProperty.Register("Contrast", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(4)));

        public double Contrast
        {
            get => (double)GetValue(ContrastProperty);
            set => SetValue(ContrastProperty, value);
        }

        // Gamma
        public static readonly DependencyProperty GammaProperty =
            DependencyProperty.Register("Gamma", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(5)));

        public double Gamma
        {
            get => (double)GetValue(GammaProperty);
            set => SetValue(GammaProperty, value);
        }

        // RedAdjust
        public static readonly DependencyProperty RedAdjustProperty =
            DependencyProperty.Register("RedAdjust", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

        public double RedAdjust
        {
            get => (double)GetValue(RedAdjustProperty);
            set => SetValue(RedAdjustProperty, value);
        }

        // GreenAdjust
        public static readonly DependencyProperty GreenAdjustProperty =
            DependencyProperty.Register("GreenAdjust", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public double GreenAdjust
        {
            get => (double)GetValue(GreenAdjustProperty);
            set => SetValue(GreenAdjustProperty, value);
        }

        // BlueAdjust
        public static readonly DependencyProperty BlueAdjustProperty =
            DependencyProperty.Register("BlueAdjust", typeof(double),
                typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(2)));

        public double BlueAdjust
        {
            get => (double)GetValue(BlueAdjustProperty);
            set => SetValue(BlueAdjustProperty, value);
        }
        // Shadows / Highlights / Midtone
        public static readonly DependencyProperty ShadowStrengthProperty =
            DependencyProperty.Register("ShadowStrength", typeof(double), typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(6)));

        public double ShadowStrength
        {
            get => (double)GetValue(ShadowStrengthProperty);
            set => SetValue(ShadowStrengthProperty, value);
        }

        public static readonly DependencyProperty HighlightStrengthProperty =
            DependencyProperty.Register("HighlightStrength", typeof(double), typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(7)));

        public double HighlightStrength
        {
            get => (double)GetValue(HighlightStrengthProperty);
            set => SetValue(HighlightStrengthProperty, value);
        }

        public static readonly DependencyProperty MidtoneBalanceProperty =
            DependencyProperty.Register("MidtoneBalance", typeof(double), typeof(ColorAdjustEffect),
                new UIPropertyMetadata(0.5, PixelShaderConstantCallback(8)));

        public double MidtoneBalance
        {
            get => (double)GetValue(MidtoneBalanceProperty);
            set => SetValue(MidtoneBalanceProperty, value);
        }

        // Enable curve (bool → float)
        public static readonly DependencyProperty EnableCurveProperty =
            DependencyProperty.Register("EnableCurve", typeof(bool), typeof(ColorAdjustEffect),
                new UIPropertyMetadata(true, OnEnableCurveChanged));


        public static void OnEnableCurveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var effect = (ColorAdjustEffect)d;
            double floatValue = ((bool)e.NewValue) ? 1.0 : 0.0;
            effect.SetValue(EnableCurveFloatProperty, floatValue);
        }

        public static readonly DependencyProperty EnableCurveFloatProperty =
            DependencyProperty.Register("EnableCurveFloat", typeof(double), typeof(ColorAdjustEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(9)));

        public bool EnableCurve
        {
            get => (bool)GetValue(EnableCurveProperty);
            set => SetValue(EnableCurveProperty, value);
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                }
            }
        }
        public String ShaderUri
        {
            get => _shaderUri;
            set
            {
                if (_shaderUri != value)
                {
                    _shaderUri = value;
                    _shader = new PixelShader { UriSource = new Uri(_shaderUri) };
                    PixelShader = _shader;
                }
            }
        }
    }
}
