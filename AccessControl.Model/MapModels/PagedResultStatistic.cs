using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.MapModels
{
    public class PagedResultStatistic<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    }
}
