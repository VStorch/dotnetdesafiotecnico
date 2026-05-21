namespace TaskManager.Domain.Exceptions
{
    public class InvalidUserClaimException : DomainException
    {
        public InvalidUserClaimException()
            : base("User ID claim is missing or invalid.")
        {
        }
    }
}