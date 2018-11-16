using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BangazonWorkforce.Models
{
	public class EmployeeDetailViewModel
	{
		public int Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public int DepartmentId { get; set; }

		public int ComputerId { get; set; }

		public string DepartmentName { get; set; }

		public string ComputerMake { get; set; }

		public string ComputerManufacturer { get; set; }
	}
}

