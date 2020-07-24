using ControlzEx.Theming;
using MahApps.Metro.Theming;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimsWpfControls.Helper
{
    public class SolidAccentsLibaryThemeProvider : LibraryThemeProvider

    {
        public static readonly SolidAccentsLibaryThemeProvider DefaultInstance = new SolidAccentsLibaryThemeProvider();

        /// <inheritdoc cref="LibraryThemeProvider" />
        public SolidAccentsLibaryThemeProvider() : base(true)
        {
        }

        public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
        {
            if (values is null) throw new ArgumentNullException(nameof(values));
            if (colorValues is null) throw new ArgumentNullException(nameof(colorValues));

            if (colorValues.Options.UseHSL)
            {
                Color background = (Color)ColorConverter.ConvertFromString(colorValues.Options.BaseColorScheme.Values["MahApps.Colors.ThemeBackground"]);
                Color foreground = (Color)ColorConverter.ConvertFromString(colorValues.Options.BaseColorScheme.Values["MahApps.Colors.ThemeForeground"]);
                Color accent = colorValues.AccentBaseColor;
                double factor = colorValues.Options.BaseColorScheme.Name == "Dark" ? 0.1 : 0.2;

                var accentHsv = new HSVColor(accent);
                var backgroundHsv = new HSVColor(background);
                var foregroundHsv = new HSVColor(foreground);

                values.Add("MahApps.Colors.AccentBase", accent.ToString(CultureInfo.InvariantCulture));
                values.Add("MahApps.Colors.Accent", GetAccentedColor(accentHsv, backgroundHsv, factor * 1).ToString(CultureInfo.InvariantCulture));
                values.Add("MahApps.Colors.Accent2", GetAccentedColor(accentHsv, backgroundHsv, factor * 2).ToString(CultureInfo.InvariantCulture));
                values.Add("MahApps.Colors.Accent3", GetAccentedColor(accentHsv, backgroundHsv, factor * 3).ToString(CultureInfo.InvariantCulture));
                values.Add("MahApps.Colors.Accent4", GetAccentedColor(accentHsv, backgroundHsv, factor * 4).ToString(CultureInfo.InvariantCulture));

                values.Add("MahApps.Colors.Highlight", GetAccentedColor(accentHsv, foregroundHsv, factor).ToString(CultureInfo.InvariantCulture));
                values.Add("MahApps.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                values.Add("MahApps.Colors.AccentBase", colorValues.AccentBaseColor.ToString());
                values.Add("MahApps.Colors.Accent", colorValues.AccentColor80.ToString());
                values.Add("MahApps.Colors.Accent2", colorValues.AccentColor60.ToString());
                values.Add("MahApps.Colors.Accent3", colorValues.AccentColor40.ToString());
                values.Add("MahApps.Colors.Accent4", colorValues.AccentColor20.ToString());

                values.Add("MahApps.Colors.Highlight", colorValues.HighlightColor.ToString());
                values.Add("MahApps.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString());
            }

        }

        public override string GetThemeGeneratorParametersContent()
        {
            return MahAppsLibraryThemeProvider.DefaultInstance.GetThemeGeneratorParametersContent();
        }

        public override string GetThemeTemplateContent()
        {
            return MahAppsLibraryThemeProvider.DefaultInstance.GetThemeTemplateContent();
        }

        private static Color GetAccentedColor (HSVColor accentBase, HSVColor background, double percentage)
        {
            double newSaturation = accentBase.Saturation + (background.Saturation - accentBase.Saturation) * percentage;
            double newValue = accentBase.Value + (background.Value - accentBase.Value) * percentage;

            return new HSVColor(accentBase.Hue, newSaturation, newValue).ToColor();
        }


    }
}