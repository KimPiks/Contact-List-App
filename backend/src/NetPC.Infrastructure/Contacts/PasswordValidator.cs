using System.Text.RegularExpressions;

namespace NetPC.Infrastructure.Contacts;

public static class PasswordValidator
{
    private static Regex UppercaseRegex() => new Regex(@"[A-Z]");
    private static Regex LowercaseRegex() => new Regex(@"[a-z]");
    private static Regex DigitRegex() => new Regex(@"\d");
    private static Regex SpecialCharRegex() => new Regex(@"[!@#$%^&*]");
    
    public static void ValidatePasswordComplexity(string password)
    {
        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.");
        if (!UppercaseRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        if (!LowercaseRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        if (!DigitRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one digit.");
        if (!SpecialCharRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one special character.");
    }
}