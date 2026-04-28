using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Model.ExportExModel
{
    public class ExportExcelEmployeeStatistic
    {
        public int STT { get; set; }
        [DisplayName("Họ và tên")]
        public string EmName { get; set; }
        [DisplayName("Mã nhân viên")]
        public string EmCode { get; set; }
        [DisplayName("Giới tính")]
        public string EmGender { get; set; }
        [DisplayName("Ngày sinh")]
        public string EmBirthdate { get; set; }
        [DisplayName("Địa chỉ")]
        public string EmAddress { get; set; }
        [DisplayName("SĐT")]
        public string EmPhone { get; set; }
        [DisplayName("Phòng ban")]
        public string DepName { get; set; }
        [DisplayName("Chức vụ")]
        public string RegName { get; set; }
        [DisplayName("Trạng thái")]
        public string EmStatus { get; set; }
    }
}
