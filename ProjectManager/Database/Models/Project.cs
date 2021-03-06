using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Database.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public float HourValue { get; set; }
        public User Creator { get; set; }
        
        [InverseProperty("Project")]
        public List<UserProject> Members { get; set; }
        
        [InverseProperty("Project")]
        public List<Issue> Issues { get; set; }
    }
}