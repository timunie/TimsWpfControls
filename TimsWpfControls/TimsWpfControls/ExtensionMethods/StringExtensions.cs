using System;
using System.Collections.Generic;
using System.Text;

namespace TimsWpfControls.ExtensionMethods
{
    public static class StringExtensions
    {
        internal static string GetStringToTheRight(this string input, int CaretIndex, char StopCharacter)
        {
            var StartIndex = input.LastIndexOf(StopCharacter, CaretIndex -1);
            if (StartIndex < 0) StartIndex = 0;
            return input.Substring(StartIndex, CaretIndex - StartIndex).Trim(StopCharacter);
        }
    }
}
