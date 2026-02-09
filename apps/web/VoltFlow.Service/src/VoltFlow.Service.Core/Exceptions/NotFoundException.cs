namespace VoltFlow.Service.Core.Exceptions
{
    public class NotFoundException : BusinessException { public NotFoundException(string msg) : base(msg, 404) { } }
}
