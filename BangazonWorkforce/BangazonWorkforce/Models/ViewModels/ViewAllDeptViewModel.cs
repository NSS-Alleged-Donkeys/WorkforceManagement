// 

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels
{
    public class ViewAllDeptViewModel
    {
        public List<Department> Departments { get; set; }
    }
}
