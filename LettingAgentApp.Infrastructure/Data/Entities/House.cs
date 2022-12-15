using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LettingAgentApp.Infrastructure.Data.Entities
{
    public class House
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; }

        [Required]
        public decimal PricePerMonth { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Required]
        public int AgentId { get; set; }

        [ForeignKey(nameof(AgentId))]
        public Agent Agent { get; set; }

        public string? RenterId { get; set; }

        [ForeignKey(nameof(RenterId))]
        public IdentityUser? Renter { get; set; }
    }
}
