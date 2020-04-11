using System;

namespace PostmanAssignment.Exceptions
{
    public class InvalidArgumentException : Exception
    {
        public string ArgumentName { get; }
        public string ArgumentValue { get; }
        public string ExpectedArgumentValue { get; }

        public InvalidArgumentException()
        {
        }

        public InvalidArgumentException(string message) : base(message)
        {
        }

        public InvalidArgumentException(string argumentName, string argumentValue, string expectedArgumentValue) : base(GetMessage(argumentName, argumentValue, expectedArgumentValue))
        {
            this.ArgumentName = argumentName;
            this.ArgumentValue = argumentValue;
            this.ExpectedArgumentValue = expectedArgumentValue;
        }

        private static string GetMessage(string argumentName, string argumentValue, string expectedArgumentValue)
        {
            return $"Invalid argument : {argumentName} supplied with value : {argumentValue}. Expected : {expectedArgumentValue}";
        }

        public InvalidArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}