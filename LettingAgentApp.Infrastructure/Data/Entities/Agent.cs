using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LettingAgentApp.Infrastructure.Data.Entities
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
