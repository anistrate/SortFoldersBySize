using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public  class Result
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }
        public bool IsFailure => !IsSuccess;
        protected Result(bool success, string error)
        {
            if (!success && string.IsNullOrEmpty(error)) throw new ArgumentException();
            if (success && !string.IsNullOrEmpty(error)) throw new ArgumentException();

            IsSuccess = success;
            Error = error;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result Ok()
        {
            return new Result(true, String.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, String.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }

    }

    public class Result<T> : Result
    {
        private T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess) throw new ArgumentException();

                return _value;
            }
            [param: AllowNull]
            private set { _value = value; }
        }

        protected internal Result([AllowNull] T value, bool success, string error)
            : base(success, error)
        {
            if (value != null && !success) throw new ArgumentException();

            Value = value;
        }
    }

}
