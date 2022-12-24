using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace LettingAgentApp.Core.DtoModels.House
{
    public class HouseFormModel
    {
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Title { get; init; }

        [Required]
        [StringLength(150, MinimumLength = 30)]
        public string Address { get; init; }

        [Required]
        [StringLength(500, MinimumLength = 50)]
        public string Description { get; init; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; init; }

        [Required]
        [Display(Name = "Price per month")]
        [Range(0.00, 2000.00, ErrorMessage = "Price per month must be a positive number and less than {2} leva")]
        public decimal PricePerMonth { get; init; }

        [Display(Name = "Category")]
        public int CategoryId { get; init; }

        public IEnumerable<HouseCategoryServiceModel> Categories { get; set; } = new List<HouseCategoryServiceModel>();


    }
}
