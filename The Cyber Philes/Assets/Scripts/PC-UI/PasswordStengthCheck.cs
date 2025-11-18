using System;
using System.Text.RegularExpressions;

public class PasswordStrengthCheck
{
    public int CheckPasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return 0; // Empty password
        }

        // Check for strong password first
        if (IsStrongPassword(password))
        {
            return 3; // Strong password
        }

        // Check for adequate password
        if (IsNotAdequatePassword(password))
        {
            return 2; // Not adequate password
        }

        // Check for horrible password
        if (IsHorriblePassword(password))
        {
            return 1; // Horrible password
        }

        return 0; // Fallback (shouldn't be needed)
    }

    private bool IsStrongPassword(string password)
    {
        // Check if password is at least 8 characters long and contains at least one lowercase letter, one uppercase letter, one digit, and one special character.
        if (password.Length >= 12 &&
            Regex.IsMatch(password, @"[a-z]") &&
            Regex.IsMatch(password, @"[A-Z]") &&
            Regex.IsMatch(password, @"\d") &&
            Regex.IsMatch(password, @"[\W_]")) // \W matches any non-word character (e.g., @, #, $, etc.)
        {
            return true;
        }

        return false;
    }

    private bool IsNotAdequatePassword(string password)
    {
        // Check if password has at least 6 characters and contains a mix of lowercase, uppercase, and digits
        if (password.Length >= 6 &&
            Regex.IsMatch(password, @"[a-z]") &&
            Regex.IsMatch(password, @"[A-Z]") &&
            Regex.IsMatch(password, @"\d"))
        {
            return true;
        }

        return false;
    }

    private bool IsHorriblePassword(string password)
    {
        // Check for single character or repeating characters
        if (password.Length == 1 || Regex.IsMatch(password, @"^([a-zA-Z0-9])\1+$"))
        {
            return true;
        }

        // Check for common weak passwords
        string[] weakPasswords = { "password", "123456", "123456789", "qwerty", "abc123", "letmein", "welcome" };
        if (Array.Exists(weakPasswords, p => p.Equals(password, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}
