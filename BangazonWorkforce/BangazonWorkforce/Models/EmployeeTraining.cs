using BangazonWorkforce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
    public class EmployeeTraining
    {
        [Key]
        public int EmployeeTrainingId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        [Required]
        public int TrainingId { get; set; }
        public virtual TrainingProgram Training { get; set; }
    }
}