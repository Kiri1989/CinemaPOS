using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CinemaPOS.Models;
using CinemaPOS.Utilities; 

namespace CinemaPOS.Managers
{
    public static class ConcessionManager
    {
        // Entry point for managing concessions (manager-level)
        public static void EditConcessions(Cinema cinema, string cinemaFilePath)
        {
            bool done = false;

            while (!done)
            {
                Console.WriteLine("\n=== Concession Menu ===");
                for (int i = 0; i < cinema.Concessions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {cinema.Concessions[i]}");
                }

                Console.WriteLine("a. Add new concession");
                Console.WriteLine("r. Remove concession");
                Console.WriteLine("q. Quit concession editing");

                Console.Write("Select option: ");
                string input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case "a":
                        AddConcession(cinema);
                        SaveConcessions(cinema, cinemaFilePath);
                        break;
                    case "r":
                        RemoveConcession(cinema);
                        SaveConcessions(cinema, cinemaFilePath);
                        break;
                    case "q":
                        done = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // Add new concession with validation
        private static void AddConcession(Cinema cinema)
        {
            Console.Write("Enter concession name (letters/numbers/spaces only): ");
            string name = Console.ReadLine();

            if (!name.IsValidConcessionName())
            {
                Console.WriteLine("Invalid name format. Only letters, numbers, and spaces are allowed.");
                return;
            }

            if (cinema.Concessions.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("That concession already exists.");
                return;
            }

            Console.Write("Enter price in pennies (e.g., 250 = £2.50): ");
            if (!int.TryParse(Console.ReadLine(), out int price) || price <= 0)
            {
                Console.WriteLine("Invalid price. Must be a positive whole number.");
                return;
            }

            cinema.Concessions.Add(new Concession { Name = name, Price = price });
            Console.WriteLine("✅ Concession added.");
        }

        //  Remove existing concession by number ===
        private static void RemoveConcession(Cinema cinema)
        {
            Console.Write("Enter the number of the concession to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > cinema.Concessions.Count)
            {
                Console.WriteLine("Invalid number.");
                return;
            }

            string removed = cinema.Concessions[index - 1].Name;
            cinema.Concessions.RemoveAt(index - 1);
            Console.WriteLine($"✅ Removed {removed}.");
        }

        //  Save updated concessions to the cinema file ===
        private static void SaveConcessions(Cinema cinema, string cinemaFilePath)
        {
            if (!File.Exists(cinemaFilePath))
            {
                Console.WriteLine("Cinema file not found.");
                return;
            }

            // Read all lines and remove previous Concession entries
            var lines = File.ReadAllLines(cinemaFilePath).ToList();
            lines = lines.Where(line => !line.StartsWith("[Concession:")).ToList();

            // Append new concessions
            foreach (var c in cinema.Concessions)
            {
                lines.Add($"[Concession:{c.Name}%Price:{c.Price}]");
            }

            File.WriteAllLines(cinemaFilePath, lines);
            Console.WriteLine("✅ Concession list saved to cinema file.");
        }
    }
}
