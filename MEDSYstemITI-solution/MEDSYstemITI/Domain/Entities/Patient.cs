using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Patient : BaseEntity
    {
        // Now exposed with public getters (readable by mapping/services) while
        // keeping setters private so the only way to change them is through the
        // validated constructor / UpdateProfile method below.
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public int Age { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        private Patient() { }

        public Patient(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Age = CalculateAge(DateOfBirth);
        }

        public void UpdateProfile(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Age = CalculateAge(DateOfBirth);
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
