using System;

namespace DicomReader2.Helpers
{
    public class Result
    {
        protected Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        protected Result(Exception exception)
        {
            Error = exception ?? throw new InvalidOperationException();
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public bool IsException => Error != null;
        public Exception Error { get; }

        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
            {
                action?.Invoke();
            }

            return this;
        }

        public Result OnFailure(Action action)
        {
            if (IsFailure)
            {
                action?.Invoke();
            }

            return this;
        }

        public Result OnException(Action<Exception> action)
        {
            if (IsException)
            {
                action?.Invoke(Error);
            }

            return this;
        }

        public static implicit operator Result(bool isSuccess) => new Result(isSuccess);

        public static implicit operator Result(Exception exception) => Exception(exception);

        public static Result Fail() => new Result(false);

        public static Result Exception(Exception exception) => new Result(exception);

        public static Result Success() => new Result(true);
    }

    public class Result<T> : Result
    {
        private Result(bool isSuccess, T value) : base(isSuccess)
        {
            if (isSuccess && value == null)
            {
                throw new InvalidOperationException();
            }

            Value = value;
        }

        private Result(Exception exception) : base(exception)
        {
        }

        public T Value { get; }

        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess)
            {
                action?.Invoke(Value);
            }

            return this;
        }

        public new Result<T> OnFailure(Action action)
        {
            if (IsFailure)
            {
                action?.Invoke();
            }

            return this;
        }

        public new Result<T> OnException(Action<Exception> action)
        {
            if (IsException)
            {
                action?.Invoke(Error);
            }

            return this;
        }

        public static implicit operator Result<T>(T value) => Success(value);

        public static implicit operator Result<T>(Exception exception) => Exception(exception);

        public new static Result<T> Fail() => new Result<T>(false, default);

        public new static Result<T> Exception(Exception exception) => new Result<T>(exception);

        public static Result<T> Success(T value) => new Result<T>(true, value);
    }
}
