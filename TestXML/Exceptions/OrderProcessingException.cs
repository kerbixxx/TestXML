namespace TestXML.Exceptions
{
    public class OrderProcessingException : Exception
    {
        public OrderProcessingException() : base() { }
        public OrderProcessingException(string message) : base(message) { }
        public OrderProcessingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
