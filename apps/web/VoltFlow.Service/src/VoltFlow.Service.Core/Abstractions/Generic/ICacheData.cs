
namespace VoltFlow.Service.Core.Abstractions.Generic
{
    public interface ICacheData<D> where D : class
    {
        IEnumerable<D> Items { get; set; }

        void insert(IEnumerable<D> enumerable);
    }
}
