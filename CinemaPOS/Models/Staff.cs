namespace CinemaPOS.Models
{
    public enum StaffLevel
    {
        General,
        Manager
    }

    public class Staff
    {
        public string ID { get; set; }               // Unique staff ID
        public string FirstName { get; set; }        // First name
        public string LastName { get; set; }         // Last name
        public StaffLevel Level { get; set; }        // Manager or General

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} ({Level}) - ID: {ID}";
        }
    }
}
