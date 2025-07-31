using System.Collections.Generic;

namespace CinemaPOS.Models
{
    public class Cinema
    {
        public string Name { get; set; }

        public Dictionary<string, Screen> Screens { get; set; } = new(); // A, B, C, etc.

        public List<Concession> Concessions { get; set; } = new();
        public List<Staff> Staff { get; set; } = new();

        public int StandardTicketPrice { get; set; }
        public int PremiumTicketPrice { get; set; }

        // Correct type: holds screenings
        public List<Screening> Screenings { get; set; } = new();
    }
}
