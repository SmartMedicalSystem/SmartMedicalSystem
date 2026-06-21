using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Patient
    {
        public Patient(int sSN, string firstName, string lastName, int age, DateTime dateOfBirth)
        {
            SSN = sSN;
            FirstName = firstName;
            LastName = lastName;
            if (age < 0)
            {
                throw new ArgumentException("Age cannot be negative.", nameof(age));
            }
        
            Age = age;
            DateOfBirth = dateOfBirth;
        }

        private int SSN { get; set; }
        private string FirstName { get; set; } = string.Empty;
        private string LastName { get; set; }

        private int Age { get; set; }
        public DateTime DateOfBirth { get; set; }

       
    }
}
