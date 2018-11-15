using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class EmployeeEditViewModel
    {
        public string LastName { get; set; }

        public int DepartmentId { get; set; }

        public int ComputerId { get; set; }

        public List<TrainingProgram> AllTrainingPrograms { get; set; }

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
