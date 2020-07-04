using System;

namespace ProjectManager.Utils
{
    public static class Functional
    {
        public static IntermediateTry<TReturn> Try<TReturn>(Func<TReturn> block)
        {
            try
            {
                return new IntermediateTry<TReturn>(block.Invoke(), null);
            }
            catch (Exception exception)
            {
                return new IntermediateTry<TReturn>(default, exception);
            }
        }
    }

    public class IntermediateTry<TReturn>
    {
        private readonly TReturn _result;
        private readonly Exception _exception;

        public IntermediateTry(TReturn result, Exception exception)
        {
            _result = result;
            _exception = exception;
        }

        public IntermediateTry<TReturn> On<TException>(Func<Exception, TReturn> block) where TException : Exception
        {
            return _exception == null ? this : Functional.Try(() => block.Invoke(_exception));
        }

        public TReturn OrElseThrow()
        {
            return _exception == null ? _result : throw _exception;
        }
    }
}