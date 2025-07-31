using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CinemaPOS.Models;
using CinemaPOS.Utilities; 

namespace CinemaPOS.Managers
{
    public static class LoyaltyManager
    {
        private static readonly string memberFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CinemaOS", "members.txt");

        // Load members from file
        public static List<Member> LoadMembers()
        {
            var members = new List<Member>();

            if (File.Exists(memberFile))
            {
                foreach (var line in File.ReadAllLines(memberFile))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        members.Add(new Member
                        {
                            ID = parts[0],
                            FirstName = parts[1],
                            LastName = parts[2],
                            Email = parts[3],
                            VisitCount = parts.Length > 4 ? int.Parse(parts[4]) : 0,
                            GoldExpiry = parts.Length > 5 && DateTime.TryParse(parts[5], out var expiry) ? expiry : (DateTime?)null
                        });
                    }
                }
            }

            return members;
        }

        // Add a new member
        public static void AddMember(List<Member> members)
        {
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();
            if (!firstName.IsValidName())
            {
                Console.WriteLine("First name must start with uppercase and contain only letters.");
                return;
            }

            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();
            if (!lastName.IsValidName())
            {
                Console.WriteLine("Last name must start with uppercase and contain only letters.");
                return;
            }

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            if (!email.IsValidEmail())
            {
                Console.WriteLine("Invalid email format.");
                return;
            }

            if (members.Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Member with that email already exists.");
                return;
            }

            var newMember = new Member
            {
                ID = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                VisitCount = 0
            };

            members.Add(newMember);
            SaveMembers(members);
            Console.WriteLine("Member registered successfully.");
        }

        // Save to file
        public static void SaveMembers(List<Member> members)
        {
            var lines = members.Select(m => $"{m.ID}|{m.FirstName}|{m.LastName}|{m.Email}|{m.VisitCount}|{m.GoldExpiry?.ToString("yyyy-MM-dd")}");
            File.WriteAllLines(memberFile, lines);
        }
    }
}
