using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.MapModels
{
    public class EmployeeStatisticMapping
    {
        [Key]
        public int EmId { get; set; }
        public int? RegId { get; set; }
        public string RegName { get; set; }
        public int? DepId { get; set; }
        public string DepName { get; set; }
        public string EmCode { get; set; }
        public string EmName { get; set; }
        public string EmGender { get; set; }
        public DateTime? EmBirthdate { get; set; }
        public string EmAddress { get; set; }
        public string EmPhone { get; set; }
        public bool EmStatus { get; set; }
    }
}
