using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CinemaPOS.Models;

namespace CinemaPOS.Utilities
{
    public static class CinemaLoader
    {
        public static Cinema LoadCinema(string path)
        {
            var cinema = new Cinema();
            cinema.Screens = new Dictionary<string, Screen>();
            cinema.Concessions = new List<Concession>();
            cinema.Staff = new List<Staff>();

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (line.StartsWith("[Name:"))
                {
                    cinema.Name = line.Substring(6, line.Length - 7);
                }
                else if (line.StartsWith("[Screen:"))
                {
                    var parts = line.Substring(8, line.Length - 9).Split('%');
                    string letter = parts[0];
                    int prem = int.Parse(parts[1].Split(':')[1]);
                    int std = int.Parse(parts[2].Split(':')[1]);

                    cinema.Screens[letter] = new Screen
                    {
                        Letter = letter,
                        PremiumSeats = prem,
                        StandardSeats = std
                    };
                }
                else if (line.StartsWith("[Staff:"))
                {
                    var parts = line.Substring(7, line.Length - 8).Split('%');
                    string id = parts[0];
                    string level = parts[1].Split(':')[1];
                    string first = parts[2].Split(':')[1];
                    string last = parts[3].Split(':')[1];

                    cinema.Staff.Add(new Staff
                    {
                        ID = id,
                        Level = Enum.Parse<StaffLevel>(level),
                        FirstName = first,
                        LastName = last
                    });
                }
                else if (line.StartsWith("[Ticket:"))
                {
                    var parts = line.Substring(8, line.Length - 9).Split('%');
                    string type = parts[0];
                    int price = int.Parse(parts[1]);

                    if (type == "Standard")
                        cinema.StandardTicketPrice = price;
                    else if (type == "Premium")
                        cinema.PremiumTicketPrice = price;
                }
                else if (line.StartsWith("[Concession:"))
                {
                    var parts = line.Substring(12, line.Length - 13).Split('%');
                    string name = parts[0];
                    int price = int.Parse(parts[1].Split(':')[1]);

                    cinema.Concessions.Add(new Concession
                    {
                        Name = name,
                        Price = price
                    });
                }
            }

            return cinema;
        }

        // Save updated staff list back to the same file
        public static void SaveCinema(Cinema cinema, string path)
        {
            var lines = new List<string>
            {
                $"[Name:{cinema.Name}]"
            };

            foreach (var screen in cinema.Screens.Values)
            {
                lines.Add($"[Screen:{screen.Letter}%NumPremiumSeat:{screen.PremiumSeats}%NumStandardSeat:{screen.StandardSeats}]");
            }

            foreach (var staff in cinema.Staff)
            {
                lines.Add($"[Staff:{staff.ID}%Level:{staff.Level}%FirstName:{staff.FirstName}%LastName:{staff.LastName}]");
            }

            lines.Add($"[Ticket:Standard%{cinema.StandardTicketPrice}]");
            lines.Add($"[Ticket:Premium%{cinema.PremiumTicketPrice}]");

            foreach (var c in cinema.Concessions)
            {
                lines.Add($"[Concession:{c.Name}%Price:{c.Price}]");
            }

            File.WriteAllLines(path, lines);
        }
    }
}
