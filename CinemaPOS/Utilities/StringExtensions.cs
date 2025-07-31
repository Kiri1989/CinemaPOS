using System.Text.RegularExpressions;

namespace CinemaPOS.Utilities
{
    public static class StringExtensions
    {
        // Validates names (First/Last)
        // Must start with a capital letter and only contain letters
        public static bool IsValidName(this string input)
        {
            return Regex.IsMatch(input, @"^[A-Z][a-zA-Z]*$");
        }

        // Validates email address format
        // Must contain one @, can use letters, numbers, and must not start or end with invalid characters
        public static bool IsValidEmail(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // Must contain exactly one @
            int atCount = input.Count(c => c == '@');
            if (atCount != 1) return false;

            // Must not start or end with . or @
            if (input.StartsWith(".") || input.EndsWith(".") || input.StartsWith("@") || input.EndsWith("@"))
                return false;

            // Must not contain ".." or ".@" or "@."
            if (input.Contains("..") || input.Contains(".@") || input.Contains("@."))
                return false;

            // General structure check
            return Regex.IsMatch(input, @"^[a-zA-Z0-9_.@]+$");
        }

        // Validates screen letter (must be single uppercase letter A-Z)
        public static bool IsValidScreenLetter(this string input)
        {
            return Regex.IsMatch(input, @"^[A-Z]$");
        }

        // Validates concession names (letters, numbers, and spaces)
        public static bool IsValidConcessionName(this string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9 ]+$");
        }

        // Optional: Validates staff full name (letters only, at least 1 char)
        public static bool IsValidFullName(this string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
        }
    }
}
