using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ViewModels
{
    public class RegencyViewModel
    {
        public int RegId { get; set; }
        public string? RegName { get; set; }
        public string? RegDescription { get; set; }
        public bool? RegStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? DeleteBy { get; set; }
    }
}
