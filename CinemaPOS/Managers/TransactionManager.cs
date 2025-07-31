using System;
using System.Collections.Generic;
using System.Linq;
using CinemaPOS.Models;
using CinemaPOS.Utilities;

namespace CinemaPOS.Managers
{
    public static class TransactionManager
    {
        public static void StartTransaction(Cinema cinema, List<Movie> movies, List<Member> members, string memberFilePath)
        {
            Console.WriteLine("\n=== Start New Transaction ===");

            // Step 1: Select Screening
            var screening = ScreeningManager.SelectScreening(cinema);
            if (screening == null) return;

            // Step 2: Link a member
            Member member = null;
            Console.Write("Do you have a loyalty member ID? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                Console.Write("Enter member ID: ");
                string memberId = Console.ReadLine();
                member = members.FirstOrDefault(m => m.ID.Equals(memberId, StringComparison.OrdinalIgnoreCase));

                if (member == null)
                {
                    Console.WriteLine("Member not found.");
                }
                else
                {
                    member.DowngradeIfExpired();
                    Console.WriteLine($"Welcome back, {member.FullName} ({(member.IsGoldActive() ? "Gold" : "Loyalty")})");
                }
            }

            // Step 3: Choose ticket types
            int numStandard = GetTicketCount("standard");
            int numPremium = GetTicketCount("premium");

            // Check seat availability
            int remainingStandard = screening.TotalStandardSeats - screening.SoldStandard;
            int remainingPremium = screening.TotalPremiumSeats - screening.SoldPremium;

            if (numStandard > remainingStandard || numPremium > remainingPremium)
            {
                Console.WriteLine("Not enough seats available.");
                return;
            }

            // Loyalty scheme: free standard ticket after 10 visits
            bool oneFreeTicket = member != null && member.EligibleForFreeTicket && numStandard > 0;
            int payableStandard = oneFreeTicket ? numStandard - 1 : numStandard;

            // Step 4: Choose concessions
            var concessions = new List<Concession>();
            var selectedConcessions = new List<Concession>();

            foreach (var concession in cinema.Concessions)
            {
                Console.Write($"Add {concession.Name}? (Y/N): ");
                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    selectedConcessions.Add(concession);
                }
            }

            // Step 5: Calculate total
            int total = 0;

            // Ticket pricing
            total += payableStandard * cinema.StandardTicketPrice;
            total += numPremium * cinema.PremiumTicketPrice;

            // Concession pricing (apply 25% off if gold)
            foreach (var item in selectedConcessions)
            {
                int price = item.Price;
                if (member != null && member.IsGoldActive())
                {
                    price = (int)(price * 0.75);
                }

                total += price;
            }

            // Step 6: Display breakdown
            Console.WriteLine("\n=== Transaction Summary ===");
            Console.WriteLine($"{numStandard} Standard Tickets ({(oneFreeTicket ? "1 free!" : "")})");
            Console.WriteLine($"{numPremium} Premium Tickets");
            if (selectedConcessions.Any())
            {
                Console.WriteLine("Concessions:");
                foreach (var item in selectedConcessions)
                {
                    Console.WriteLine($" - {item.Name} {(member != null && member.IsGoldActive() ? "(25% off)" : "")}");
                }
            }
            Console.WriteLine($"TOTAL: £{total / 100.0:0.00}");

            Console.Write("Confirm purchase? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() != "Y")
            {
                Console.WriteLine("Transaction cancelled.");
                return;
            }

            // Step 7: Finalise
            screening.AvailableStandardSeats -= numStandard;
            screening.AvailablePremiumSeats -= numPremium;

            // Update visit count
            if (member != null)
            {
                if (oneFreeTicket) member.VisitCount = 0;
                else member.VisitCount++;

                LoyaltyManager.SaveMembers(members);
            }

            Console.WriteLine("✅ Transaction complete. Enjoy your film!");
        }

        // Helper method to ask for ticket quantity
        private static int GetTicketCount(string type)
        {
            Console.Write($"How many {type} tickets? ");
            return int.TryParse(Console.ReadLine(), out int count) && count >= 0 ? count : 0;
        }
    }
}
