using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ViewModels
{
    public class DepartmentViewModel
    {
        public int DepId { get; set; }
        public string? DepName { get; set; }
        public string? DepDescription { get; set; }
        public bool? DepStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? DeleteBy { get; set; }
    }
}
