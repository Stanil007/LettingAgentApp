using System.ComponentModel.DataAnnotations;

namespace LettingAgentApp.Infrastructure.Data.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public IEnumerable<House> Houses { get; set; } = new List<House>();
    }
}
