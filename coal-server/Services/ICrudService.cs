using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public interface ICrudService<T>
    {
        Task<List<T>> GetAsync();

        Task<T> GetAsync(string id);

        Task<T> CreateAsync(T obj);

        Task<List<T>> CreateManyAsync(List<T> objs);

        Task UpdateAsync(string id, T objIn);

        Task UpdateManyAsync(List<T> objsIn);

        Task RemoveAsync(T objIn);

        Task RemoveAsync(string id);

        void Refresh();
    }
}
