﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Bets = new HashSet<Bet>();
            this.PlayerStatistics = new HashSet<PlayerStatistic>();
        }
        public int GameId { get; set; }

        // [ForeignKey(nameof(Team))]
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; }

        // [ForeignKey(nameof(Team))]
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public DateTime DateTime { get; set; }
        public double HomeTeamBetRate { get; set; }
        public double AwayTeamBetRate { get; set; }
        public double DrawBetRate { get; set; }
        public string Result { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }
        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }


    }
}
