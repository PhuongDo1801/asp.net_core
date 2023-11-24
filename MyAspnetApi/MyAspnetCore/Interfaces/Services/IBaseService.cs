using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Interfaces.Services
{
    public interface IBaseService <TEntityDto, TEntityCreateDto, TEntityUpdateDto>
    {
        public Task<IEnumerable<TEntityDto>> GetAllAsync();
        public Task<TEntityDto> GetByIdAsync(Guid Id);
        public Task<int> InsertAsync(TEntityCreateDto entity);
        public Task<int> UpdateAsync(TEntityUpdateDto entity, Guid Id);
        public Task<int> DeleteByIdAsync(Guid Id);
    }
}
