using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reddit.Contracts.Entities;

namespace Reddit.Contracts.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? PostId { get; set; }
        public int Ups { get; set; }
        public string? UserId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Subreddit { get; set; }

        //public long SubredditId { get; set; }
        //public Subreddit? Subreddit { get; set; }
        public DateTime? CreatedWhen { get; set; }
    }
}
