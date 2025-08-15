using System;

namespace Students.Exceptions
{
    public sealed class DuplicateEmailException : Exception
    {
        public string Email { get; }
        public int? ExistingId { get; }

        public DuplicateEmailException(string email, int? existingId = null)
            : base($"Email '{email}' is already in use.")
        {
            Email = email;
            ExistingId = existingId;
        }
    }
}
