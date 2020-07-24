namespace JobManagmentSystem.Application
{
    public class JobDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int IntervalType { get; set; }
        public double Interval { get; set; }
        public string TimeStart { get; set; }
        public object TaskParameters { get; set; }
        public string Status { get; set; } //TODO: 
    }
}