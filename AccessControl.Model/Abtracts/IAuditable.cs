using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControl.Model.Abtracts
{
    public interface IAuditable
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [StringLength(128)]
        string? CreatedBy { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        [Column(TypeName = "datetime")]
        DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        [StringLength(128)]
        string? UpdatedBy { get; set; }

        /// <summary>
        /// Thời gian chỉnh sửa gần nhất
        /// </summary>
        [Column(TypeName = "datetime")]
        DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Đã xóa
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Người xóa
        /// </summary>
        [StringLength(128)]
        string? DeletedBy { get; set; }

        /// <summary>
        /// Thời gian xóa
        /// </summary>
        [Column(TypeName = "datetime")]
        DateTime? DeletedDate { get; set; }
    }
}