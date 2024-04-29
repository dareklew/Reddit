using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reddit.Contracts.Entities
{
    public class Subreddit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string SubredditId { get; set; }        
        public string? Name { get; set; }
        public DateTime? CreatedWhen { get; set; }

    }
}
