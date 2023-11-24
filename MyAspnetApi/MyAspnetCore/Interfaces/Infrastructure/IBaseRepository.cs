using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Infrastructure
{
    public interface IBaseRepository<TEntity>
    {
        public Task<List<TEntity>> GetAllAsync();
        public Task<TEntity> GetByIdAsync(Guid id);
        Task<int> InsertAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity, Guid id);
        Task<int> DeleteAsync(Guid id);
    }
}
