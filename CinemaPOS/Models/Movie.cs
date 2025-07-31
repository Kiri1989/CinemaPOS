namespace CinemaPOS.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public int LengthMinutes { get; set; }
        public string Genre { get; set; }
        public string AgeRating { get; set; }

        
        public Movie(string title, string genre, int lengthMinutes, string ageRating)
        {
            Title = title;
            Genre = genre;
            LengthMinutes = lengthMinutes;
            AgeRating = ageRating;
        }

        
        public Movie() { }

        public override string ToString()
        {
            return $"{Title} ({AgeRating}) - {LengthMinutes} mins";
        }
    }
}
