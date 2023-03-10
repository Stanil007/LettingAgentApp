using System.ComponentModel.DataAnnotations;

namespace LettingAgentApp.Core.DtoModels.House
{
    public class HouseServiceModel
    {
        public int Id { get; init; }

        public string Title { get; init; }

        public string Address { get; init; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; init; }

        [Display(Name = "Price per month")]
        public decimal PricePerMonth { get; init; }

        [Display(Name = "Is Rented")]
        public bool IsRented { get; init; }
    }
}
