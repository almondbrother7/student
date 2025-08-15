using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Students.Models.Validation
{
    public sealed class PastDateAttribute : ValidationAttribute
    {
        public PastDateAttribute() : base("Date must be before today.") {}

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // [Required] handles null
            if (value is DateTime dt)
            {
                return dt.Date < DateTime.Today;
            }
            return false;
        }
    }

    public sealed class GradeAttribute : ValidationAttribute
    {
        public GradeAttribute() : base("Grade must be \"K\" or a number between 1 and 12.") {}

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // [Required] handles null
            if (value is string s)
            {
                s = s.Trim();
                if (string.Equals(s, "K", StringComparison.OrdinalIgnoreCase)) return true;
                if (int.TryParse(s, out var n)) return n >= 1 && n <= 12;
            }
            return false;
        }
    }

    public sealed class UsPhoneAttribute : ValidationAttribute
    {
        private static readonly Regex Rx = new Regex(
            @"^(?:\+1[-.\s]?)?(?:\(?\d{3}\)?[-.\s]?)\d{3}[-.\s]?\d{4}$",
            RegexOptions.Compiled);

        public UsPhoneAttribute() : base("Phone must be a valid US number.") {}

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // optional
            if (value is string s)
            {
                s = s.Trim();
                if (s.Length == 0) return true;
                return Rx.IsMatch(s);
            }
            return false;
        }
    }
}
