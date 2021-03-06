using System;

namespace ProjectManager.Utils
{
    public static class Validate
    {
        public static void NotNullOrBlank(string value, string message = "Value cannot be empty.")
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message);
            }
        }

        public static void NotNegative(float value, string message = "Value cannot be negative.")
        {
            if (value < 0)
            {
                throw new ArgumentException(message);
            }
        }

        public static TResult NotNullOrElseThrow<TResult>(TResult item, Func<Exception> exceptionSupplier)
        {
            if (item == null)
            {
                throw exceptionSupplier.Invoke();
            }

            return item;
        }

        public static void ShouldThrow<TException>(Action block, string message = "An exception was expected.") where TException : Exception
        {
            try
            {
                block.Invoke();
                throw new ArgumentException(message);
            }
            catch (Exception exception)
            {
                if (!(exception is TException))
                {
                    throw;
                }
            } 
        }
    }
}