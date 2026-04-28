using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class Device
    {
        public int DevId { get; set; }
        public string? DevName { get; set; }
        public string? DevIp { get; set; }
        public int? DevPort { get; set; }
        public string? DevSerialnumber { get; set; }
        public string? DevMacaddress { get; set; }
        public bool? DevStatus { get; set; }
        public DateTime? DevLastTime { get; set; }
        public DateTime? OnlineTime { get; set; }
        public bool? Online { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
