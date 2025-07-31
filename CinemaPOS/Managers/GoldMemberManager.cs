using System;
using System.Collections.Generic;
using CinemaPOS.Models;
using CinemaPOS.Managers;

namespace CinemaPOS.Managers
{
    public static class GoldMemberManager
    {
        // Automatically downgrades expired gold members and saves updated data
        public static void ValidateGoldMemberships(List<Member> members)
        {
            foreach (var member in members)
            {
                member.DowngradeIfExpired(); // If expired, convert to regular loyalty
            }

            // Save changes back to file
            LoyaltyManager.SaveMembers(members);
        }

        // Sell a gold membership to an existing loyalty member
        public static void SellGoldMembership(List<Member> members)
        {
            Console.Write("Enter Member Email: ");
            string email = Console.ReadLine();

            var member = members.Find(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (member == null)
            {
                Console.WriteLine("❌ Member not found.");
                return;
            }

            // Check if already active gold
            if (member.IsGold && member.GoldExpiry.HasValue && member.GoldExpiry > DateTime.Today)
            {
                Console.WriteLine("⚠️ This member already has an active gold membership.");
                return;
            }

            // Upgrade to gold
            member.IsGold = true;
            member.GoldExpiry = DateTime.Today.AddYears(1); // 1 year from today

            Console.WriteLine($"✅ Gold membership activated. Expires: {member.GoldExpiry:yyyy-MM-dd}");

            LoyaltyManager.SaveMembers(members); // Save updated list
        }
    }
}
