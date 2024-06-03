namespace ScriptableArchitecture.Core
{
    public static class StringExtensions
    {
        public static string RemoveUnderscore(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.TrimStart('_');
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public static string RemoveAfterDot(this string input)
        {
            int dotIndex = input.IndexOf('.');
            return (dotIndex != -1) ? input.Substring(0, dotIndex) : input;
        }
    }
}