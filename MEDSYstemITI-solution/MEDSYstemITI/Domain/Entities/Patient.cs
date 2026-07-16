using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities.Baseperson;
using Domain.Enums;

namespace Domain.Entities
{
    public class Patient : BasePerson
    {

        // Patient-specific properties; common personal/contact fields live in BasePerson
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        // EF materialization constructor
        protected Patient() { }

        // Backward-compatible constructor used by data seeders and existing code.
        public Patient(string firstName, string lastName, int nationalId, DateTime dateOfBirth, Gender gender, int mobileNumber, string address, BloodType bloodType)
            : base(firstName, lastName, dateOfBirth)
        {
            Gender = gender;
            Address = address;
            BloodType = bloodType;
            PhoneNumber = mobileNumber.ToString();
            EncryptedNationalId = nationalId.ToString();
        }

        // Domain fields expected by services/migrations
      

        public Patient(string firstName, string lastName, DateTime dateOfBirth)
            : base(firstName, lastName, dateOfBirth)
        {
        }



        // Backward-compatible constructor used by data seeders and existing code.
        public Patient(string firstName, string lastName, DateTime dateOfBirth, Gender gender, string address, BloodType bloodType)
            : base(firstName, lastName, dateOfBirth)
        {
            Gender = gender;
            Address = address;
            BloodType = bloodType;
        }

        public Enums.BloodType BloodType { get; set; }

     
  

  
    }
}
