﻿using System;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Extensions
{
    public static class ResultExtensions
    {
        public static Result OnSuccess(this Result result, Action action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsSuccess)
            {
                action();
            }

            return result;
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        public static Result OnFailureThrow(this Result result, Exception exception)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure)
            {
                throw exception;
            }

            return result;
        }

        public static Result OnException(this Result result, Action<Exception> action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsException)
            {
                action(result.Error);
            }

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        public static Result<T> OnFailureThrow<T>(this Result<T> result, Exception exception)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure)
            {
                throw exception;
            }

            return result;
        }

        public static Result<T> OnException<T>(this Result<T> result, Action<Exception> action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsException)
            {
                action(result.Error);
            }

            return result;
        }

        public static Result OnFailureOrException(this Result result, Action action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure || result.IsException)
            {
                action();
            }

            return result;
        }

        public static Result<T> OnFailureOrException<T>(this Result<T> result, Action action)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.IsFailure || result.IsException)
            {
                action();
            }

            return result;
        }

        public static Result<TTo> Select<TFrom, TTo>(this Result<TFrom> result, Func<TFrom, Result<TTo>> selector)
        {
            if (!result.IsSuccess)
            {
                return Result<TTo>.Fail();
            }

            try
            {
                return selector(result.Value);
            }
            catch (Exception exception)
            {
                return Result<TTo>.Exception(exception);
            }
        }

        public static Result<T> Select<T>(this Result result, T value) => !result.IsSuccess ? Result<T>.Fail() : Result<T>.Success(value);

        public static Result Select<T>(this Result<T> result, Func<T, Result> selector)
        {
            if (!result.IsSuccess)
            {
                return Result.Fail();
            }

            try
            {
                return selector(result.Value);
            }
            catch (Exception exception)
            {
                return Result.Exception(exception);
            }
        }
    }
}
