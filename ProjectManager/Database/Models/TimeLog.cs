using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Database.Models
{
    public class TimeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Issue Issue { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public float Minutes { get; set; }
    }
}