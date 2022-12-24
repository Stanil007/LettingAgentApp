using System.ComponentModel.DataAnnotations;

namespace LettingAgentApp.Core.DtoModels.Agent
{
    public class BecomeAgentFormModel
    {
        [Required]
        [StringLength(15, MinimumLength = 7)]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = null!;
    }
}
