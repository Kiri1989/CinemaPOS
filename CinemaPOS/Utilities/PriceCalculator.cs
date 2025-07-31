using System;
using System.Collections.Generic;
using CinemaPOS.Models;
using CinemaPOS.Managers;

namespace CinemaPOS.Utilities
{
    public static class PriceCalculator
    {
        // === Calculate the total transaction cost ===
        public static int CalculateTotal(
            Cinema cinema,
            Screening screening,
            int standardTickets,
            int premiumTickets,
            List<Concession> concessions,
            Member member)
        {
            int total = 0;

            // Loyalty: Check if member gets a free standard ticket
            bool applyFreeStandard = member != null && member.VisitCount >= 10 && standardTickets > 0;
            int standardTicketPrice = applyFreeStandard ? 0 : cinema.StandardTicketPrice;

            // Ticket costs
            total += standardTicketPrice * standardTickets;
            total += cinema.PremiumTicketPrice * premiumTickets;

            // Concession total 
            int concessionTotal = 0;
            foreach (var item in concessions)
            {
                concessionTotal += item.Price;
            }

            //  Apply 25% gold discount if applicable 
            if (member != null && member.IsGoldActive())
            {
                concessionTotal = (int)(concessionTotal * 0.75); // 25% off
            }

            total += concessionTotal;

            return total;
        }

        //  After finalizing transaction: Update loyalty visits
        public static void UpdateMemberVisit(Member member)
        {
            if (member != null)
            {
                member.VisitCount++;

                // If the user used their 10th visit for a free ticket, reset counter
                if (member.VisitCount > 10)
                    member.VisitCount = 1; // Restart count after using free ticket
            }
        }
    }
}
