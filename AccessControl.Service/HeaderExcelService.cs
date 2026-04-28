using AccessControl.Data.Repositories;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Service
{
    public interface IHeaderExcelService
    {
        HeaderExcel GetTop1();
    }
    public class HeaderExcelService : IHeaderExcelService
    {
        private readonly IHeaderExcelRepository _headerExcelRepository;
        public HeaderExcelService(IHeaderExcelRepository headerExcelRepository)
        {
            _headerExcelRepository = headerExcelRepository;
        }

        public HeaderExcel GetTop1()
        {
            return _headerExcelRepository.GetTop1();
        }
    }
}
