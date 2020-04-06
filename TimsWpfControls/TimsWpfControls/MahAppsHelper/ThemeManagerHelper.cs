using MahApps.Metro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using generator = XamlColorSchemeGenerator;

namespace TimsWpfControls.MahAppsHelper
{
    public static class ThemeManagerHelper
    {

        public static ResourceDictionary CreateAppStyleBy(string Theme, Color Accent, Color? Highlight = null )
        {

            var Name = "Custom" + (ThemeManager.ColorSchemes.Count(x => x.Name.StartsWith("Custom")) + 1);

            var theme = GetThemeResourceDictionary(Theme, Accent, Highlight, Name);

            ThemeManager.AddTheme(theme);
            ThemeManager.ChangeTheme(Application.Current, theme["Theme.Name"].ToString());

            return theme;
        }


        private static generator.GeneratorParameters GetGeneratorParameters()
        {
            return JsonConvert.DeserializeObject<generator.GeneratorParameters>(GetGeneratorParametersJson());
        }

        private static string GetGeneratorParametersJson()
        {
            var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("MahApps.Metro.Styles.Themes.GeneratorParameters.json");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static ResourceDictionary GetThemeResourceDictionary(string baseColorScheme, Color accentBaseColor, Color? highlightColor = null, string AccentName = null)
        {
            var generatorParameters = GetGeneratorParameters();
            var themeTemplateContent = GetThemeTemplateContent();

            var variant = generatorParameters.BaseColorSchemes.First(x => x.Name == baseColorScheme);
            var colorScheme = new generator.ColorScheme
            {
                Name = AccentName
            };
            var values = colorScheme.Values;

            var baseColor = baseColorScheme == "Dark" ? Colors.Black : Colors.White;

            values.Add("MahApps.Colors.AccentBase", accentBaseColor.ToString());
            values.Add("MahApps.Colors.Accent", accentBaseColor.AddColor(baseColor, 0.1).ToString());
            values.Add("MahApps.Colors.Accent2", accentBaseColor.AddColor(baseColor, 0.25).ToString());
            values.Add("MahApps.Colors.Accent3", accentBaseColor.AddColor(baseColor, 0.4).ToString());
            values.Add("MahApps.Colors.Accent4", accentBaseColor.AddColor(baseColor, 0.55).ToString());

            // Check if HighlightColor is set or not 
            if (highlightColor is null) highlightColor = baseColorScheme == "Dark" ? accentBaseColor.AddColor(Colors.White, 0.3) : accentBaseColor.AddColor(Colors.Black, 0.2);

            values.Add("MahApps.Colors.Highlight", highlightColor.ToString());
            values.Add("MahApps.Colors.IdealForeground", IdealTextColor(accentBaseColor).ToString());

            // Strings
            values.Add("ColorScheme", AccentName);

            var xamlContent = new generator.ColorSchemeGenerator().GenerateColorSchemeFileContent(generatorParameters, variant, colorScheme, themeTemplateContent, $"{baseColorScheme}.{AccentName}", $"{AccentName} ({baseColorScheme})");


            // Check if we have to fix something
            if (xamlContent.Contains("WithAssembly=\""))
            {

                var withAssemblyMatches = Regex.Matches(xamlContent, @"\s*xmlns:(.+?)WithAssembly=("".+?"")");

                foreach (Match withAssemblyMatch in withAssemblyMatches)
                {
                    var originalMatches = Regex.Matches(xamlContent, $@"\s*xmlns:({withAssemblyMatch.Groups[1].Value})=("".+?"")");

                    foreach (Match originalMatch in originalMatches)
                    {
                        xamlContent = xamlContent.Replace(originalMatch.Groups[2].Value, withAssemblyMatch.Groups[2].Value);
                    }
                }
            }

            var resourceDictionary = (ResourceDictionary)XamlReader.Parse(xamlContent);

            return resourceDictionary;
        }


        private static string GetThemeTemplateContent()
        {
            var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("MahApps.Metro.Styles.Themes.Theme.Template.xaml");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


        /// <summary>
        ///     Determining Ideal Text Color Based on Specified Background Color
        ///     http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name="color">The bg.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color color)
        {
            const int nThreshold = 105;
            var bgDelta = Convert.ToInt32((color.R * 0.299) + (color.G * 0.587) + (color.B * 0.114));
            var foreColor = 255 - bgDelta < nThreshold
                ? Colors.Black
                : Colors.White;
            return foreColor;
        }


        private static Color AddColor(this Color baseColor, Color ColorToAdd, double? Factor)
        {
            var firstColorAlpha = baseColor.A;
            var secondColorAlpha = Factor.HasValue ? Convert.ToByte(Factor * 255) : ColorToAdd.A;

            var alpha = CompositeAlpha(firstColorAlpha, secondColorAlpha);

            var r = CompositeColorComponent(baseColor.R, firstColorAlpha, ColorToAdd.R, secondColorAlpha, alpha);
            var g = CompositeColorComponent(baseColor.G, firstColorAlpha, ColorToAdd.G, secondColorAlpha, alpha);
            var b = CompositeColorComponent(baseColor.B, firstColorAlpha, ColorToAdd.B, secondColorAlpha, alpha);

            return Color.FromArgb(alpha, r, g, b);
        }

        /// <summary>
        /// For a single R/G/B component. a = precomputed CompositeAlpha(a1, a2)
        /// </summary>
        private static byte CompositeColorComponent(byte c1, byte a1, byte c2, byte a2, byte a)
        {
            // Handle the singular case of both layers fully transparent.
            if (a == 0)
            {
                return 0;
            }

            return System.Convert.ToByte((((255 * c2 * a2) + (c1 * a1 * (255 - a2))) / a) / 255);
        }

        private static byte CompositeAlpha(byte a1, byte a2)
        {
            return System.Convert.ToByte(255 - ((255 - a2) * (255 - a1)) / 255);
        }

    }
}
