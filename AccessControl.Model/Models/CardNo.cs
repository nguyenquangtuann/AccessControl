using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class CardNo
    {
        public int CaId { get; set; }
        public int? EmId { get; set; }
        public string? CaNo { get; set; }
        public string? CaNumber { get; set; }
        public bool? CaStatus { get; set; }
        /// <summary>
        /// thẻ vẫn của người hiện tại thì using = true, thu hồi thẻ và cấp mới cho người khác thì người cũ = false, người mới = true
        /// </summary>
        public bool? Using { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Employee? Em { get; set; }
    }
}
