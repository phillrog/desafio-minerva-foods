namespace DesafioMinervaFoods.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public List<string> Errors { get; } = new();

        protected Result(bool isSuccess, List<string>? errors)
        {
            IsSuccess = isSuccess;
            if (errors != null) Errors.AddRange(errors);
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, new List<string> { error });
        public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToList());
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        protected Result(bool isSuccess, T? data, List<string>? errors)
            : base(isSuccess, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public new static Result<T> Failure(string error)
            => new(false, default, new List<string> { error });
        public new static Result<T> Failure(IEnumerable<string> errors)
            => new(false, default, errors.ToList());
    }
}
