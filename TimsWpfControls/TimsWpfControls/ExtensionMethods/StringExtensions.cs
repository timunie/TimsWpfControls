namespace TimsWpfControls.ExtensionMethods
{
    public static class StringExtensions
    {
        internal static string GetStringToTheRight(this string input, int CaretIndex, char StopCharacter)
        {
            var StartIndex = input.LastIndexOf(StopCharacter, CaretIndex - 1);
            if (StartIndex < 0) StartIndex = 0;
            return input.Substring(StartIndex, CaretIndex - StartIndex).Trim(StopCharacter);
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
            return input.Substring(StartIndex, CaretIndex - StartIndex).Trim();
        }
    }
}