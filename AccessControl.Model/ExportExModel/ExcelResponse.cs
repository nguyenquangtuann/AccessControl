using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ExportExModel
{
    public class ExcelResponse
    {
        public ExcelResponse(string path)
        {
            this.Result = path;
        }
        public string Result { get; set; }
    }
}
