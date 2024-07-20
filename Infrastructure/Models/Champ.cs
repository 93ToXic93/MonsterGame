using System.ComponentModel.DataAnnotations;
namespace MonsterGame.Infrastructure.Models
{
    public class Champ
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.Constants.MaxNameOfTheChampLength)]
        public string ChampType { get; set; } = default!;

        [Required]
        public int Health { get; set; }

        [Required]
        public int Damage { get; set; }

        [Required]
        public int Mana { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

    }
}
