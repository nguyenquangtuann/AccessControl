using AccessControl.Data.Infrastructure.Extentsions;
using AccessControl.Data.Repositories;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IRegencyService
    {
        Task<IQueryable<Regency>> GetAll();
        Task<PagingResult<RegencyViewModel>> GetListPaging(int page, int pageSize, string keyword);
        Task<Regency> GetById(int id);
        Task<Regency> Create(Regency regency);
        Task<Regency> Update(Regency regency);
        Task<Regency> Delete(int id);
    }
    public class RegencyService : IRegencyService
    {
        private readonly IRegencyRepository _regencyRepository;
        public RegencyService(IRegencyRepository regencyRepository)
        {
            _regencyRepository = regencyRepository;
        }

        public async Task<Regency> Create(Regency regency)
        {
            return await _regencyRepository.CheckContainsAsync(x => x.RegName == regency.RegName) ? throw new NameDuplicatedException("Tên chức vụ đã tồn tại") : await _regencyRepository.AddASync(regency);
        }

        public async Task<Regency> Delete(int id)
        {
            return await _regencyRepository.DeleteAsync(id);
        }

        public async Task<IQueryable<Regency>> GetAll()
        {
            return await _regencyRepository.GetAllAsync(x=>x.RegStatus == true);
        }

        public async Task<Regency> GetById(int id)
        {
            return await _regencyRepository.GetByIdAsync(id);
        }

        public async Task<PagingResult<RegencyViewModel>> GetListPaging(int page, int pageSize, string keyword)
        {
            return await _regencyRepository.GetListPaging(page, pageSize, keyword);
        }

        public async Task<Regency> Update(Regency regency)
        {
            return await _regencyRepository.CheckContainsAsync(x => x.RegId != regency.RegId && x.RegName == regency.RegName) ? throw new NameDuplicatedException("Tên chức vụ đã tồn tại") : await _regencyRepository.UpdateASync(regency);
        }
    }
}
