using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels
{
    public class ViewAllDeptEmp
    {
        public List<Department> AllDepartments { get; set; }
        public List<Employee> AllEmployees { get; set; }
    }
}
