namespace VoltFlow.Service.Core.Exceptions
{
    public class ValidationEntityException : BusinessException { public ValidationEntityException(string msg) : base(msg, 400) { } }
}
