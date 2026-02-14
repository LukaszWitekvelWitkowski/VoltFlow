namespace VoltFlow.Service.Core.Models.Auth
{
    public class Result
    {
        public bool Success { get;}

        public Result(bool success)
        {
            Success = success;
        }   
    }
}
