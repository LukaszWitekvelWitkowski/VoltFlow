using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Core.Abstractions
{
    public interface IJWTProvider
    {
        string Generate(User user);
    }
}
