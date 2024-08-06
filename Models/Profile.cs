using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string Bio { get; set; }

        public string Preferences { get; set; }

        public byte[] ProfilePicture { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31.")]
        public int Day { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required]
        [Display(Name = "Interested In")]
        public Gender InterestedIn { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}