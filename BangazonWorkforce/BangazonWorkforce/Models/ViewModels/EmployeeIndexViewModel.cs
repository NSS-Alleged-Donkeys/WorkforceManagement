// David Taylor
// Created view model containing IEnumerable List of Employee Model to allow for optimization of the Index View down the road

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeIndexViewModel
    {
        public IEnumerable<Employee> Employees;
    }
}
