using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Database.Models
{
    public class UserProject
    {
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}