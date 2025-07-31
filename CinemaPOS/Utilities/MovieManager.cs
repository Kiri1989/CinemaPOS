using System;
using System.Collections.Generic;
using System.IO;
using CinemaPOS.Models;

namespace CinemaPOS.Utilities
{
    public static class MovieManager
    {
        public static List<Movie> LoadMovies(string path)
        {
            var movies = new List<Movie>();

            if (!File.Exists(path))
            {
                Console.WriteLine("Movies file not found.");
                return movies;
            }

            var lines = File.ReadAllLines(path);

            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.StartsWith("[Movie:"))
                {
                    string title = Extract(line, "Movie", "%");
                    int length = int.Parse(Extract(line, "Length", "%"));
                    string genre = line.Contains("Genre") ? Extract(line, "Genre", "%") : "";
                    string rating = Extract(line, "Rating", "]");

                    movies.Add(new Movie { Title = title, LengthMinutes = length, Genre = genre, AgeRating = rating });

                }
            }

            Console.WriteLine($"Loaded {movies.Count} movies.");
            return movies;
        }

        private static string Extract(string line, string startTag, string endTag)
        {
            int start = line.IndexOf(startTag) + startTag.Length + 1;
            int end = line.IndexOf(endTag, start);
            return line.Substring(start, end - start);
        }
    }
}
