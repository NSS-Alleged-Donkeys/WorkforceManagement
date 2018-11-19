// David Taylor
// Creates variables to represent data in models

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentDetailViewModel
    {
        public IEnumerable<Employee> Employees;
        public Department Departments;
    }
}
