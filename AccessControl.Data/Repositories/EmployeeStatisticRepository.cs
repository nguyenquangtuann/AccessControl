using AccessControl.Data.Infrastructure;
using AccessControl.Model.MapModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IEmployeeStatisticRepository : IRepository<EmployeeStatisticMapping>
    {
        Task<IEnumerable<EmployeeStatisticMapping>> GetAllPaging(string sql, int page, int pageSize);
        Task<int> Count(string sql);
        Task<IEnumerable<EmployeeStatisticMapping>> GetAll(string sql);
    }
    public class EmployeeStatisticRepository : RepositoryBase<EmployeeStatisticMapping>, IEmployeeStatisticRepository
    {
        private readonly ACSDBContext _context;
        public EmployeeStatisticRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeStatisticMapping>> GetAllPaging(string sql, int page, int pageSize)
        {
            var parameters = new SqlParameter[]
            {
                    new SqlParameter("@sql",SqlDbType.NVarChar){Value = sql ?? ""},
                    new SqlParameter("@page", SqlDbType.Int) { Value = page },
                    new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize }
            };
            var reponse = await _context.EmployeeStatisticMappings.FromSqlRaw("[dbo].[TKNhanVien_Paging] @sql, @page, @pageSize", parameters).ToListAsync();
            return reponse;
        }

        public async Task<int> Count(string sql)
        {
            var parameters = new SqlParameter[]
            {
                    new SqlParameter("@sql",SqlDbType.NVarChar){Value = sql ?? ""}
            };
            var result = await _context.CountResults.FromSqlRaw("[dbo].[TKNhanVien_Count] @sql", parameters).ToListAsync();
            return result.FirstOrDefault()?.Total ?? 0;
        }

        public async Task<IEnumerable<EmployeeStatisticMapping>> GetAll(string sql)
        {
            var parameters = new SqlParameter[]
            {
                    new SqlParameter("@sql",SqlDbType.NVarChar){Value = sql ?? ""}
            };
            var reponse = await _context.EmployeeStatisticMappings.FromSqlRaw("[dbo].[TKNhanVien] @sql", parameters).ToListAsync();
            return reponse;
        }
    }
}
