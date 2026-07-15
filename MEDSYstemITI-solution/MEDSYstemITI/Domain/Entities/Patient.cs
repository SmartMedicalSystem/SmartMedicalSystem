using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Patient : BasePerson
    {
        // Patient-specific properties; common personal/contact fields live in BasePerson
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        // EF materialization constructor
        protected Patient() { }

        public Patient(string firstName, string lastName, DateTime dateOfBirth)
            : base(firstName, lastName, dateOfBirth)
        {
        }


     
  

  
    }
}
