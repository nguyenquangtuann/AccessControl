using AccessControl.Data.Repositories;
using AccessControl.Model.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IEmployeeStatisticService
    {
        Task<PagedResultStatistic<EmployeeStatisticMapping>> GetAllPaging(List<int> depId, List<int> regId, int status, int page, int pageSize);
        Task<List<EmployeeStatisticMapping>> GetAll(List<int> depId, List<int> regId, int status);
    }
    public class EmployeeStatisticService : IEmployeeStatisticService
    {
        private readonly IEmployeeStatisticRepository _employeeStatisticRepository;
        public EmployeeStatisticService(IEmployeeStatisticRepository employeeStatisticRepository)
        {
            _employeeStatisticRepository = employeeStatisticRepository;
        }

        public async Task<List<EmployeeStatisticMapping>> GetAll(List<int> depId, List<int> regId, int status)
        {
            var sql = new StringBuilder();

            if (depId != null && depId.Count > 0)
            {
                sql.Append(" and dep.DEP_ID in (" + string.Join(",", depId) + ")");
            }

            if (regId != null && regId.Count > 0)
            {
                sql.Append(" and reg.REG_ID in (" + string.Join(",", regId) + ")");
            }

            if (status != 2)
            {
                sql.Append(" and em.EM_STATUS = " + status);
            }
            var data = await _employeeStatisticRepository.GetAll(sql.ToString());
            return data.ToList();
        }

        public async Task<PagedResultStatistic<EmployeeStatisticMapping>> GetAllPaging(List<int> depId, List<int> regId, int status, int page, int pageSize)
        {
            var sql = new StringBuilder();

            if (depId != null && depId.Count > 0)
            {
                sql.Append(" and dep.DEP_ID in (" + string.Join(",", depId) + ")");
            }

            if (regId != null && regId.Count > 0)
            {
                sql.Append(" and reg.REG_ID in (" + string.Join(",", regId) + ")");
            }

            if (status != 2)
            {
                sql.Append(" and em.EM_STATUS = " + status);
            }

            var data = await _employeeStatisticRepository.GetAllPaging(sql.ToString(), page, pageSize);

            var total = await _employeeStatisticRepository.Count(sql.ToString());

            return new PagedResultStatistic<EmployeeStatisticMapping>
            {
                Data = data.ToList(),
                Total = total,
                PageIndex = page,
                PageSize = pageSize
            };
        }
    }
}
