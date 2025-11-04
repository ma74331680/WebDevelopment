using System.ComponentModel.DataAnnotations;

namespace WebDevelopment.ViewModels
{
    public class ArtworkCreateVm
    {
        [Required, MaxLength(120)]
        public string Title { get; set; } = default!;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }

}
