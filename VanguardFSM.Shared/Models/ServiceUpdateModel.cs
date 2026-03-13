using System.ComponentModel.DataAnnotations;

namespace VanguardFSM.Shared.Models
{
    public class ServiceUpdateModel
    {
        [Required(ErrorMessage = "Notes are required.")]
        public string Notes { get; set; } = string.Empty;
        public string? PartsUsed { get; set; }
    }
}
