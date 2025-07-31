using System;
using System.Collections.Generic;
using System.IO;
using CinemaPOS.Models;
using CinemaPOS.Managers;
using CinemaPOS.Utilities;

namespace CinemaPOS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cinema POS System";

            // Base path setup: dynamic & cross-compatible 
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string cinemaPath = Path.Combine(basePath, "CinemaOS", "Cinema.txt.txt");
            string moviesPath = Path.Combine(basePath, "CinemaOS", "Movies.txt.txt");
            string memberFilePath = Path.Combine(basePath, "CinemaOS", "Members.txt");

            // Load core data
            Cinema cinema = CinemaLoader.LoadCinema(cinemaPath);
            List<Movie> movies = MovieManager.LoadMovies(moviesPath);
            List<Member> members = LoyaltyManager.LoadMembers();

            // Gold membership validation on startup
            GoldMemberManager.ValidateGoldMemberships(members);

            // Staff Login
            Console.Clear();
            Console.WriteLine("=== Cinema POS Login ===");
            for (int i = 0; i < cinema.Staff.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cinema.Staff[i]}");
            }

            Console.Write("Select your staff number: ");
            if (!int.TryParse(Console.ReadLine(), out int staffIndex) || staffIndex < 1 || staffIndex > cinema.Staff.Count)
            {
                Console.WriteLine("Invalid login.");
                return;
            }

            Staff currentStaff = cinema.Staff[staffIndex - 1];
            Console.Clear();
            Console.WriteLine($"Welcome {currentStaff.FullName} ({currentStaff.Level})");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Start Transaction");
                Console.WriteLine("2. Register Member");
                Console.WriteLine("3. Sell Gold Membership");
                Console.WriteLine("4. Load Schedule");
                Console.WriteLine("5. Exit");

                if (currentStaff.Level == StaffLevel.Manager)
                {
                    Console.WriteLine("6. Set Schedule");
                    Console.WriteLine("7. Edit Concessions");
                    Console.WriteLine("8. Edit Staff");
                }

                Console.Write("Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        TransactionManager.StartTransaction(cinema, movies, members, memberFilePath);
                        break;
                    case "2":
                        LoyaltyManager.AddMember(members);
                        break;
                    case "3":
                        GoldMemberManager.SellGoldMembership(members);
                        break;
                    case "4":
                        Console.Write("Enter schedule date (yyyy-MM-dd): ");
                        string date = Console.ReadLine();
                        ScreeningManager.LoadSchedule(cinema, movies, date);
                        break;
                    case "5":
                        exit = true;
                        break;
                    case "6":
                        if (currentStaff.Level == StaffLevel.Manager)
                            ScreeningManager.ScheduleScreening(cinema, movies);
                        break;
                    case "7":
                        if (currentStaff.Level == StaffLevel.Manager)
                            ConcessionManager.EditConcessions(cinema, cinemaPath);
                        break;
                    case "8":
                        if (currentStaff.Level == StaffLevel.Manager)
                            StaffManager.ManageStaff(cinema, cinemaPath);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}


