namespace CinemaPOS.Models
{
    public class Screen
    {
        public string Letter { get; set; }       // Screen identifier (e.g., A, B, C)
        public int StandardSeats { get; set; }   // Number of standard seats
        public int PremiumSeats { get; set; }    // Number of premium seats
    }
}
