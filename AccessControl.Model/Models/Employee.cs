using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class Employee
    {
        public int EmId { get; set; }
        public int? RegId { get; set; }
        public int? DepId { get; set; }
        public string EmCode { get; set; }
        public string EmName { get; set; }
        public string EmGender { get; set; }
        public DateTime? EmBirthdate { get; set; }
        public string EmIdentityNumber { get; set; }
        public string EmAddress { get; set; }
        public string EmPhone { get; set; }
        public string EmEmail { get; set; }
        public byte[] EmImage { get; set; }
        public bool EmStatus { get; set; }
        public bool EditStatus { get; set; }
        public string DevIdSynchronized { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string DeleteBy { get; set; }
    }
}
