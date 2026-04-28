using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using AccessControl.Model.Models;
using AccessControl.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IRegencyRepository : IRepository<Regency>
    {
        Task<PagingResult<RegencyViewModel>> GetListPaging(int page, int pageSize, string keyword);
    }
    public class RegencyRepository : RepositoryBase<Regency>, IRegencyRepository
    {
        private readonly ACSDBContext _context;
        private readonly DbContextOptions<ACSDBContext> _dbContextOptions;
        public RegencyRepository(ACSDBContext context, DbContextOptions<ACSDBContext> dbContextOptions) : base(context)
        {
            _context = context;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<PagingResult<RegencyViewModel>> GetListPaging(int page, int pageSize, string keyword)
        {
            using ACSDBContext context = new(_dbContextOptions);
            using ACSDBContext context2 = new(_dbContextOptions);
            Task<List<RegencyViewModel>> lstRegTask;
            Task<int> countTask;
            if (string.IsNullOrEmpty(keyword))
            {
                lstRegTask = (from reg in context.Regencies
                              where reg.RegStatus == true
                              select new RegencyViewModel
                              {
                                  RegId = reg.RegId,
                                  RegName = reg.RegName,
                                  RegDescription = reg.RegDescription,
                                  RegStatus = reg.RegStatus,
                                  CreatedBy = reg.CreatedBy,
                                  CreatedDate = reg.CreatedDate,
                                  UpdatedBy = reg.UpdatedBy,
                                  UpdatedDate = reg.UpdatedDate,
                                  DeleteBy = reg.DeleteBy,
                                  DeleteDate = reg.DeleteDate
                              }).OrderByDescending(x => x.RegId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from reg in context2.Regencies
                             where reg.RegStatus == true
                             select reg).CountAsync();
            }
            else
            {
                lstRegTask = (from reg in context.Regencies
                              where reg.RegName.ToLower().Contains(keyword.ToLower())
                              select new RegencyViewModel
                              {
                                  RegId = reg.RegId,
                                  RegName = reg.RegName,
                                  RegDescription = reg.RegDescription,
                                  RegStatus = reg.RegStatus,
                                  CreatedBy = reg.CreatedBy,
                                  CreatedDate = reg.CreatedDate,
                                  UpdatedBy = reg.UpdatedBy,
                                  UpdatedDate = reg.UpdatedDate,
                                  DeleteBy = reg.DeleteBy,
                                  DeleteDate = reg.DeleteDate
                              }).OrderByDescending(x => x.RegId).Skip(page * pageSize).Take(pageSize).ToListAsync();

                countTask = (from reg in context2.Regencies
                             where reg.RegName.ToLower().Contains(keyword.ToLower())
                             select reg).CountAsync();
            }
            await Task.WhenAll(lstRegTask, countTask);
            return new PagingResult<RegencyViewModel>
            {
                Items = await lstRegTask,
                Count = await countTask
            };
        }
    }
}
