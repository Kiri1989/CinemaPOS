using System;
using System.Linq;
using CinemaPOS.Models;
using CinemaPOS.Utilities;

namespace CinemaPOS.Managers
{
    public static class StaffManager
    {
        public static void ManageStaff(Cinema cinema, string cinemaPath)
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== Staff Management ===");
                Console.WriteLine("1. View Staff");
                Console.WriteLine("2. Add Staff");
                Console.WriteLine("3. Remove Staff");
                Console.WriteLine("4. Back");

                Console.Write("Choose option: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("\nCurrent Staff:");
                        foreach (var s in cinema.Staff)
                            Console.WriteLine($"{s.ID} - {s.FullName} ({s.Level})");
                        break;

                    case "2":
                        AddStaff(cinema, cinemaPath);
                        break;

                    case "3":
                        RemoveStaff(cinema, cinemaPath);
                        break;

                    case "4":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        public static void AddStaff(Cinema cinema, string cinemaPath)
        {
            Console.Write("Enter Staff ID: ");
            string id = Console.ReadLine();

            if (cinema.Staff.Any(s => s.ID == id))
            {
                Console.WriteLine("Staff ID already exists.");
                return;
            }

            Console.Write("First Name: ");
            string first = Console.ReadLine();

            Console.Write("Last Name: ");
            string last = Console.ReadLine();

            Console.Write("Role (Manager/General): ");
            string roleInput = Console.ReadLine();

            if (!Enum.TryParse<StaffLevel>(roleInput, out var level))
            {
                Console.WriteLine("Invalid role.");
                return;
            }

            Staff newStaff = new Staff
            {
                ID = id,
                FirstName = first,
                LastName = last,
                Level = level
            };

            cinema.Staff.Add(newStaff);
            CinemaLoader.SaveCinema(cinema, cinemaPath);  // Save to file
            Console.WriteLine("✅ Staff added and saved.");
        }

        public static void RemoveStaff(Cinema cinema, string cinemaPath)
        {
            Console.Write("Enter Staff ID to remove: ");
            string id = Console.ReadLine();

            var staff = cinema.Staff.FirstOrDefault(s => s.ID == id);
            if (staff == null)
            {
                Console.WriteLine("Staff not found.");
                return;
            }

            cinema.Staff.Remove(staff);
            CinemaLoader.SaveCinema(cinema, cinemaPath);  // Save to file
            Console.WriteLine("✅ Staff removed and saved.");
        }
    }
}
