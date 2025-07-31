using System;

namespace CinemaPOS.Models
{
    // This class represents a customer who is part of the loyalty scheme.
    public class Member
    {
        // Properties
        public string ID { get; set; }              // Unique ID assigned to the member
        public string FirstName { get; set; }       // Member's first name
        public string LastName { get; set; }        // Member's last name
        public string Email { get; set; }           // Member's email address
        public int VisitCount { get; set; }         // Tracks how many transactions they've made
        public bool IsGold { get; set; }            // Determines whether they are a gold member
        public DateTime? GoldExpiry { get; set; }   // Expiration date of gold membership

        // Computed Properties
        public string FullName => $"{FirstName} {LastName}"; // For display
        public bool EligibleForFreeTicket => VisitCount >= 10;

        // === Constructors ===

        // Parameterless constructor for file loading (fixes CS7036 error)
        public Member() { }

        // Use this constructor when registering a new member
        public Member(string firstName, string lastName, string email)
        {
            ID = Guid.NewGuid().ToString().Substring(0, 8);  // Creates a short, unique ID
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            VisitCount = 0;
            IsGold = false;
            GoldExpiry = null;
        }

        // Methods

        // Formats member details for display
        public override string ToString()
        {
            string type = IsGoldActive() ? "Gold Member" : "Loyalty Member";
            return $"{FullName} ({type}) - ID: {ID} | Visits: {VisitCount}";
        }

        // Checks if gold membership is active
        public bool IsGoldActive()
        {
            return IsGold && GoldExpiry.HasValue && GoldExpiry.Value > DateTime.Today;
        }

        // Downgrade this member to regular loyalty if expired
        public void DowngradeIfExpired()
        {
            if (IsGold && (!GoldExpiry.HasValue || GoldExpiry.Value <= DateTime.Today))
            {
                IsGold = false;
                GoldExpiry = null;
            }
        }
    }
}
