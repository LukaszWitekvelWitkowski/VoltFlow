namespace VoltFlow.Service.Core.Models.Auth
{
    public class Result
    {
        public bool Success { get; }

        protected Result(bool success)
        {
            Success = success;
        }

        public static Result isSucces() => new Result(true);

        public Result isFailure() => new Result(false);

    }
}
