using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
	public class Computer
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "You must provide a purchase date for this computer")]
		[Display(Name = "Purchase Date")]
		public DateTime PurchaseDate { get; set; }

		public DateTime DecomissionDate { get; set; }

		[Required(ErrorMessage = "You must specify the make of this computer")]
		[Display(Name = "Make")]
		public string Make { get; set; }

		[Required(ErrorMessage = "You must specify the manufacturer of this computer")]
		[Display(Name = "Manufacturer")]
		public string Manufacturer { get; set; }
	}
}