namespace SmartRestaurant.Application.Exceptions;

public sealed class InvoiceConcurrencyException : Exception
{
    public InvoiceConcurrencyException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
