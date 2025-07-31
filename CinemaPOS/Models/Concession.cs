namespace CinemaPOS.Models
{
    public class Concession
    {
        public string Name { get; set; }    // Name of the concession (e.g., "Popcorn")
        public int Price { get; set; }      // Price in pennies (e.g., 250 = £2.50)

        public override string ToString()
        {
            return $"{Name} - £{Price / 100.0:F2}";
        }
    }
}
