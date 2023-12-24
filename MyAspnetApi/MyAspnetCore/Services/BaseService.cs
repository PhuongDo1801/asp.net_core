using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyAspnetCore.Exceptions;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Services
{
    public abstract class BaseService<TEntity ,TEntityDto, TEntityCreateDto, TEntityUpdateDto> 
        : IBaseService<TEntityDto, TEntityCreateDto, TEntityUpdateDto>
    {
        #region property
        protected readonly IBaseRepository<TEntity> _baseRepository;
        protected readonly IMapper _mapper;
        private string _tableName = typeof(TEntity).Name;
        #endregion  

        #region
        protected BaseService(IBaseRepository<TEntity> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        #endregion

        public async Task<IEnumerable<TEntityDto>> GetAllAsync()
        {
            var entitys = await _baseRepository.GetAllAsync();

            List<TEntityDto> entityDtoList = new List<TEntityDto>();

            if (entitys.Count > 0)
            {
                entitys.ForEach(item =>
                {
                    var entityDto = _mapper.Map<TEntityDto>(item);
                    entityDtoList.Add(entityDto);
                });
            }

            return entityDtoList;
        }

        public async Task<TEntityDto> GetByIdAsync(Guid Id)
        {
            var entity = await _baseRepository.GetByIdAsync(Id);

            if (entity == null)
            {
                throw new NotFoundException(new List<string> { ResourceVN.Err_NotFound });
            }

            var entityDto = _mapper.Map<TEntityDto>(entity);

            return entityDto;
        }

        public virtual async Task<int> InsertAsync(TEntityCreateDto entityCreateDto)
        {
            var entity = _mapper.Map<TEntity>(entityCreateDto);
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                if (name == $"{_tableName}Id")
                {
                    property.SetValue(entity, Guid.NewGuid());
                }
                else if (name == $"CreatedDate")
                {
                    property.SetValue(entity, DateTime.Now);
                }
                else if (name == $"CreatedBy")
                {
                    property.SetValue(entity, null);
                }
            }

            var result = await _baseRepository.InsertAsync(entity);

            //if (result == 0)
            //{
            //    throw new NotFoundException();
            //}

            return result;
        }

        public virtual async Task<int> UpdateAsync(TEntityUpdateDto entityeUpdateDto, Guid Id)
        {
            var check = await _baseRepository.GetByIdAsync(Id);

            if (check == null)
            {
                throw new NotFoundException(new List<string> { ResourceVN.Err_NotFound });
            }

            var entity = _mapper.Map<TEntity>(entityeUpdateDto);
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;

                if (name == $"UpdatedDate")
                {
                    property.SetValue(entity, DateTime.Now);
                }
                else if (name == $"UpdatedBy")
                {
                    property.SetValue(entity, null);
                }
            }
            var result = await _baseRepository.UpdateAsync(entity, Id);

            return result;
        }
        public async Task<int> DeleteByIdAsync(Guid Id)
        {
            var check = await _baseRepository.GetByIdAsync(Id);

            if (check == null)
            {
                throw new NotFoundException(new List<string> { ResourceVN.Err_NotFound });
            }

            var result = await _baseRepository.DeleteAsync(Id);

            return result;
        }

    }
}
