using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.Players = new HashSet<Player>();
            this.HomeGames = new HashSet<Game>();
            this.AwayGames = new HashSet<Game>();
        }
        
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Initials { get; set; }
        public decimal Budget { get; set; }

        // [ForeignKey(nameof(Color))]
        public int PrimaryKitColorId { get; set; }
        public virtual Color PrimaryKitColor { get; set; }

        // [ForeignKey(nameof(Color))]
        public int SecondaryKitColorId { get; set; }
        public virtual Color SecondaryKitColor { get; set; }
        public int TownId { get; set; }
        public virtual Town Town { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        //[InverseProperty("HomeTeam")]
        public virtual ICollection<Game> HomeGames { get; set; }

        //[InverseProperty("AwayTeam")]
        public virtual ICollection<Game> AwayGames { get; set; }

    }
}
