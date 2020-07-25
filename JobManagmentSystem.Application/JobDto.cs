using System.ComponentModel.DataAnnotations;

namespace JobManagmentSystem.Application
{
    public class JobDto
    {
        public string Key { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int IntervalType { get; set; }
        [Required] public double Interval { get; set; }
        [Required] public string TimeStart { get; set; }
        public object TaskParameters { get; set; }
    }
}