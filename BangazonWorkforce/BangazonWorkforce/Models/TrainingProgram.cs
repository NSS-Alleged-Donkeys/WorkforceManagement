using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
	public class TrainingProgram
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "You must provide a name for this Training Program")]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required(ErrorMessage = "You must provide a start date for this Training Program")]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "You must provide a start date for this Training Program")]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }

		[Required(ErrorMessage = "You must specify the maximum attendees for this Training Program")]
		[Display(Name = "Maximum Attendees")]
		public int MaxAttendees { get; set; }
	}
}