using System;

namespace CinemaPOS.Models
{
    // Represents a scheduled showing of a movie
    public class Screening
    {
        public Movie Movie { get; set; }                   // The movie being shown
        public string ScreenLetter { get; set; }           // Which screen it's shown in (e.g., A, B, C)
        public DateTime StartTime { get; set; }            // When the screening begins

        public int AvailableStandardSeats { get; set; }    // Remaining standard seats
        public int AvailablePremiumSeats { get; set; }     // Remaining premium seats

        public int SoldStandard { get; set; } = 0;         // How many standard seats have been sold
        public int SoldPremium { get; set; } = 0;          // How many premium seats have been sold

        // Total number of seats originally available
        public int TotalStandardSeats => SoldStandard + AvailableStandardSeats;
        public int TotalPremiumSeats => SoldPremium + AvailablePremiumSeats;

        // Total runtime including trailers
        public DateTime EndTime
        {
            get
            {
                int trailers = 20; // Fixed 20 minutes for trailers
                int runtime = Movie.LengthMinutes;
                return StartTime.AddMinutes(trailers + runtime);
            }
        }

        // Turnaround time based on screen capacity
        public int GetTurnaroundTime()
        {
            int totalSeats = TotalStandardSeats + TotalPremiumSeats;

            if (totalSeats <= 50)
                return 15;
            else if (totalSeats <= 100)
                return 30;
            else
                return 45;
        }

        // When the screen is available again after this screening
        public DateTime AvailableAfter => EndTime.AddMinutes(GetTurnaroundTime());

        // Display format
        public override string ToString()
        {
            return $"{Movie.Title} ({Movie.AgeRating}) @ {StartTime:HH:mm} in Screen {ScreenLetter} " +
                   $"- Std: {AvailableStandardSeats}, Prem: {AvailablePremiumSeats}";
        }
    }
}
