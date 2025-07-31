using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CinemaPOS.Models;

namespace CinemaPOS.Managers
{
    public static class ScreeningManager
    {
        // Folder where daily screening schedules will be stored
        private static readonly string scheduleFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CinemaOS", "Schedules");

        // Ensure folder exists when the class is first accessed
        static ScreeningManager()
        {
            if (!Directory.Exists(scheduleFolder))
                Directory.CreateDirectory(scheduleFolder);
        }

        // Schedule a new screening (Manager-only functionality)
        public static void ScheduleScreening(Cinema cinema, List<Movie> movies)
        {
            Console.Write("Enter schedule date (yyyy-MM-dd): ");
            string dateInput = Console.ReadLine();

            // Validate the input date
            if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            // List all available screens
            Console.WriteLine("Available Screens:");
            foreach (var screen in cinema.Screens.Values)
            {
                Console.WriteLine($"Screen {screen.Letter} - Std: {screen.StandardSeats}, Prem: {screen.PremiumSeats}");
            }

            Console.Write("Select screen letter: ");
            string screenLetter = Console.ReadLine();

            if (!cinema.Screens.ContainsKey(screenLetter))
            {
                Console.WriteLine("Screen not found.");
                return;
            }

            // Show movie options
            Console.WriteLine("\nAvailable Movies:");
            for (int i = 0; i < movies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {movies[i].Title} ({movies[i].AgeRating}) - {movies[i].LengthMinutes} mins");
            }

            Console.Write("Select movie number: ");
            if (!int.TryParse(Console.ReadLine(), out int movieIndex) || movieIndex < 1 || movieIndex > movies.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Movie selectedMovie = movies[movieIndex - 1];

            Console.Write("Enter start time (HH:mm): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            {
                Console.WriteLine("Invalid time.");
                return;
            }

            DateTime fullStart = date.Date.AddHours(time.Hour).AddMinutes(time.Minute);

            // Conflict checking
            foreach (var screening in cinema.Screenings)
            {
                if (screening.ScreenLetter == screenLetter &&
                    fullStart < screening.AvailableAfter &&
                    fullStart >= screening.StartTime)
                {
                    Console.WriteLine($"❌ Conflict: '{screening.Movie.Title}' is already showing in Screen {screenLetter} at {screening.StartTime:HH:mm}.");
                    Console.WriteLine($"Screen will be available after {screening.AvailableAfter:HH:mm}.");
                    return;
                }
            }

            // Create new screening
            var screenObj = cinema.Screens[screenLetter];

            var screeningToAdd = new Screening
            {
                Movie = selectedMovie,
                ScreenLetter = screenLetter,
                StartTime = fullStart,
                AvailableStandardSeats = screenObj.StandardSeats,
                AvailablePremiumSeats = screenObj.PremiumSeats
            };

            cinema.Screenings.Add(screeningToAdd);

            // Save the updated schedule for the selected date
            SaveSchedule(date, cinema.Screenings.Where(s => s.StartTime.Date == date).ToList());

            Console.WriteLine("✅ Screening scheduled successfully.");
        }

        // Save a list of screenings to file for a specific date
        public static void SaveSchedule(DateTime date, List<Screening> screenings)
        {
            string file = Path.Combine(scheduleFolder, $"{date:yyyy-MM-dd}.fs");

            var lines = screenings.Select(s =>
                $"{s.Movie.Title}|{s.Movie.Genre}|{s.Movie.LengthMinutes}|{s.Movie.AgeRating}|{s.ScreenLetter}|{s.StartTime:HH:mm}|{s.AvailableStandardSeats}|{s.AvailablePremiumSeats}");

            File.WriteAllLines(file, lines);
        }

        // Load a list of screenings from a .fs file
        public static List<Screening> LoadSchedule(List<Movie> allMovies, string dateText)
        {
            string file = Path.Combine(scheduleFolder, $"{dateText}.fs");
            var list = new List<Screening>();

            if (!File.Exists(file))
            {
                Console.WriteLine("No schedule file for that date.");
                return list;
            }

            foreach (var line in File.ReadAllLines(file))
            {
                var parts = line.Split('|');

                if (parts.Length >= 8)
                {
                    string title = parts[0];
                    string genre = parts[1];
                    int length = int.Parse(parts[2]);
                    string rating = parts[3];
                    string screenLetter = parts[4];
                    DateTime startTime = DateTime.ParseExact($"{dateText} {parts[5]}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    int stdSeats = int.Parse(parts[6]);
                    int premSeats = int.Parse(parts[7]);

                    // Try to find the movie in the existing list; if not found, reconstruct
                    Movie movie = allMovies.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                                ?? new Movie(title, genre, length, rating);

                    list.Add(new Screening
                    {
                        Movie = movie,
                        ScreenLetter = screenLetter,
                        StartTime = startTime,
                        AvailableStandardSeats = stdSeats,
                        AvailablePremiumSeats = premSeats
                    });
                }
            }

            return list;
        }

        // Assign loaded screenings to the cinema object
        public static void LoadSchedule(Cinema cinema, List<Movie> allMovies, string date)
        {
            var loadedScreenings = LoadSchedule(allMovies, date);
            cinema.Screenings = loadedScreenings;
            Console.WriteLine($"✅ Loaded {loadedScreenings.Count} screenings from schedule {date}.fs");
        }

        // Prompt the user to pick a screening from the list
        public static Screening SelectScreening(Cinema cinema)
        {
            if (cinema.Screenings.Count == 0)
            {
                Console.WriteLine("No screenings available.");
                return null;
            }

            Console.WriteLine("\n=== Select Screening ===");
            for (int i = 0; i < cinema.Screenings.Count; i++)
            {
                var s = cinema.Screenings[i];
                Console.WriteLine($"{i + 1}. {s.Movie.Title} @ {s.StartTime:HH:mm} in Screen {s.ScreenLetter}");
            }

            Console.Write("Enter screening number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 1 && choice <= cinema.Screenings.Count)
            {
                return cinema.Screenings[choice - 1];
            }

            Console.WriteLine("Invalid selection.");
            return null;
        }
    }
}
