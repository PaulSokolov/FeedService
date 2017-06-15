using System.Collections.Generic;

namespace FeedService.DbModels
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Collection> Collections{ get; set; }

        public User()
        {
            Collections = new HashSet<Collection>();
        }
    }
}
