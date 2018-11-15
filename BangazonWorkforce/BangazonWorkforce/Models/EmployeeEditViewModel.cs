using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class EmployeeEditViewModel
    {
        public int EmployeeId { get; set;  }

        public string LastName { get; set; }

        public int DepartmentId { get; set; }

        public int ComputerId { get; set; }

        public List<Computer> AllComputers { get; set; }

        public List<SelectListItem> AllComputerOptions
        {
            get
            {
                if (AllComputers == null)
                {
                    return null;
                }

                return AllComputers
                        .Select(c => new SelectListItem(c.Make, c.Id.ToString()))
                        .ToList();
            }
        }

        public SelectList AllTrainingPrograms { get; set; }

        public IEnumerable<TrainingProgram> SelectedTrainingPrograms { get; set; }

        public List<Department> AllDepartments { get; set; }

        public List<SelectListItem> AllDepartmentOptions
        {
            get
            {
                if (AllDepartments == null)
                {
                    return null;
                }

                return AllDepartments
                        .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                        .ToList();
            }
        }
    }
}
