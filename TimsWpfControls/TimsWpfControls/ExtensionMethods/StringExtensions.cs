using System;
using System.Runtime.Serialization;

namespace TimsWpfControls.ExtensionMethods
{
    public static class StringExtensions
    {
        internal static string GetStringToTheRight(this string input, int CaretIndex, char StopCharacter)
        {
            var StartIndex = input.LastIndexOf(StopCharacter, CaretIndex - 1);
            if (StartIndex < 0) StartIndex = 0;
            return input[StartIndex..CaretIndex].Trim(StopCharacter).TrimStart();
        }

        internal static string GetStringToTheRight(this string input, int CaretIndex, char[] StopCharacters)
        {
            int StartIndex = -1;

            for (int i = 0; i < StopCharacters.Length; i++)
            {
                var StartIndexTemp = input.LastIndexOf(StopCharacters[i], CaretIndex - 1);
                if (StartIndexTemp > StartIndex) StartIndex = StartIndexTemp;
            }

            if (StartIndex < 0) StartIndex = 0;
            return input[StartIndex..CaretIndex].Trim(StopCharacters).TrimStart();
        }

        internal static string GetStringToTheRight(this string input, int CaretIndex, string[] StopCharacters)
        {
            int StartIndex = -1;

            for (int i = 0; i < StopCharacters.Length; i++)
            {
                var StartIndexTemp = input.LastIndexOf(StopCharacters[i], CaretIndex - 1, StringComparison.Ordinal);
                if (StartIndexTemp > StartIndex) StartIndex = StartIndexTemp;
            }

            if (StartIndex < 0) StartIndex = 0;
            var result = input[StartIndex..CaretIndex].TrimStart();

            foreach (var str in StopCharacters)
            {
                if (result.StartsWith(str, StringComparison.Ordinal))
                {
                    result = result.Remove(0, str.Length);
                }
            }
            return result;
        }

        internal static string GetStringToTheRight(this string input, int CaretIndex, object StopCharacters)
        {
            return StopCharacters switch
            {
                char[] charArray => input.GetStringToTheRight(CaretIndex, charArray),
                string[] stringArray => input.GetStringToTheRight(CaretIndex, stringArray),
                string str => input.GetStringToTheRight(CaretIndex, str.ToCharArray()),
                _ => throw new ArgumentException("StopCharacters must either be char[], string[] or string")
            };
        }

        public static string ReplaceFirst(this string text, string search, string replace, StringComparison stringComparison = StringComparison.Ordinal)
        {
            int pos = text.IndexOf(search, stringComparison);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text[(pos + search.Length)..];
        }
    }
}