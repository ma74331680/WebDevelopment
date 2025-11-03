using System;
using System.ComponentModel.DataAnnotations;

namespace WebDevelopment.Models
{
    public class Artwork
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ArtistName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // 儲存圖片檔名或完整 URL
        public string? ImageUrl { get; set; }

        // 上傳時間
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
