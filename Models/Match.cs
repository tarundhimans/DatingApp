using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [Required]
        public string MatchedUserId { get; set; }
        [ForeignKey("MatchedUserId")]
        public ApplicationUser MatchedUser { get; set; }
        public bool IsLiked { get; set; }
        public bool IsRejected { get; set; }
    }
}
