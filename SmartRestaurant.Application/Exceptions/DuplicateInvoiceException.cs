namespace SmartRestaurant.Application.Exceptions;

public sealed class DuplicateInvoiceException : Exception
{
    public DuplicateInvoiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
