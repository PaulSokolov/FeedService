using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FeedService.DbModels
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

        public virtual ICollection<Collection> Collections{ get; set; }

        public User()
        {
            Collections = new HashSet<Collection>();
        }
    }
}
