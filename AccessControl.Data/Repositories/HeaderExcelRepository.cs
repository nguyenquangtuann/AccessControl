using AccessControl.Data.Infrastructure;
using AccessControl.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Repositories
{
    public interface IHeaderExcelRepository : IRepository<HeaderExcel>
    {
        HeaderExcel GetTop1();
    }
    public class HeaderExcelRepository : RepositoryBase<HeaderExcel>, IHeaderExcelRepository
    {
        private readonly ACSDBContext _context;

        public HeaderExcelRepository(ACSDBContext context) : base(context)
        {
            _context = context;
        }

        public HeaderExcel GetTop1()
        {
            var query = from hd in _context.HeaderExcels
                        select hd;
            return query.FirstOrDefault();
        }
    }
}
