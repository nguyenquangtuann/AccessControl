using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControl.Model.Abtracts
{
    public abstract class Auditable : IAuditable
    {
        public int Id { get; set; }
        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(450)]
        public string? DeletedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }
    }
}