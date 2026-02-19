using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Core.Abstractions.Tools
{
    public interface IJWTProvider
    {
        string Generate(User user);
    }
}
