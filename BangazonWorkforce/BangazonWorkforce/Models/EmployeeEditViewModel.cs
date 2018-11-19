using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BangazonWorkforce.Models
{
    public class EmployeeEditViewModel
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public int EmployeeId { get; set;  }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Computer")]
        public int ComputerId { get; set; }

        public List<int> PreselectedTrainingPrograms { get; set; }

        public List<TrainingProgram> AllTrainingPrograms { get; set; }

        public List<TrainingProgram> EmployeeTrainingPrograms { get; set; }

        [Display(Name = "Selected Training Programs")]
        public List<int> SelectedTrainingPrograms { get; set; }

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

        public List<SelectListItem> AllTrainingProgramOptions
        {
            get
            {
                if (AllTrainingPrograms == null)
                {
                    return null;
                }

                PreselectedTrainingPrograms = EmployeeTrainingPrograms.Select(tp => tp.Id).ToList();

                List<SelectListItem> allOptions = AllTrainingPrograms
                        .Select(tp => new SelectListItem(tp.Name, tp.Id.ToString()))
                        .ToList();
                foreach (int Id in PreselectedTrainingPrograms)
                {
                    foreach (SelectListItem sli in allOptions)
                    {
                        if (sli.Value == Id.ToString())
                        {
                            sli.Selected = true;

                        }
                    }
                }

                return allOptions;
            }
        }
    }
}
