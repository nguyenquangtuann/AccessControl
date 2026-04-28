using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.MapModels
{
    public class PagingResult<T>
    {
        public int Count { get; set; }
        public List<T> Items { get; set; }
    }
}
