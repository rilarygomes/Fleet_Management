namespace FleetManagement.Application.Shared
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public T Value { get; }

        private Result(bool success, T value, string error)
        {
            IsSuccess = success;
            Value = value;
            Error = error;
        }

        public static Result<T> Ok(T value) => new Result<T>(true, value, null);
        public static Result<T> Fail(string error) => new Result<T>(false, default, error);
    }
}
