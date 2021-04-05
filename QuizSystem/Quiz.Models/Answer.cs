﻿using System.Collections.Generic;

namespace QuizSystem.Models
{
    public class Answer
    {
        public Answer()
        {
            this.UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCorrect { get; set; }
        public int Points { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}