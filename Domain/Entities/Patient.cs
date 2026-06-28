using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Patient : BaseEntity
    {
        private Patient() { }
        public Patient
            (string ssn, string firstName, string lastName, string address, string contact, DateTime dateOfBirth)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetContact(contact);
            SetAddress(address);
            SetDateOfBirth(dateOfBirth);
            SetSSN(ssn);
        }

        public string SSN { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Contact { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Address { get; private set; }
        public virtual ICollection<Session> Sessions { get; private set; }
        public virtual ICollection<PatientResult> PatientResults { get; private set; }
        private void SetSSN(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn))
            {
                throw new ArgumentException("SSN is required.", nameof(ssn));
            }

            if (ssn.Length != 14)
            {
                throw new ArgumentException("SSN must be exactly 14 digits.", nameof(ssn));
            }

            if (!ssn.All(char.IsDigit))
            {
                throw new ArgumentException("SSN must contain only digits.", nameof(ssn));
            }

            SSN = ssn;
        }
        private void SetFirstName(string firstName)
        {
            if (String.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First Name is required", nameof(firstName));
            FirstName = firstName;
        }
        private void SetLastName(string lastName)
        {
            if (String.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last Name is required", nameof(lastName));
            LastName = lastName;
        }
        private void SetContact(string contact)
        {
            if (String.IsNullOrWhiteSpace(contact))
                throw new ArgumentException("Contact is required", nameof(contact));
            if (!contact.All(char.IsDigit))
            {
                throw new ArgumentException("Contact must contain only digits.", nameof(contact));
            }
            if (contact.Length != 11)
            {
                throw new ArgumentException(
                    "Contact must be 11 digits.",
                    nameof(contact));
            }
            if (!contact.StartsWith("010") &&
                !contact.StartsWith("011") &&
                !contact.StartsWith("012") &&
                !contact.StartsWith("015"))
            {
                throw new ArgumentException(
                    "Invalid Egyptian phone number.",
                    nameof(contact));
            }
            Contact = contact;
        }
        private void SetDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));
            }
            DateOfBirth = dateOfBirth;
        }
        private void SetAddress(string address)
        {
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentException("address is required", nameof(address));
            Address = address;
        }
        public void Update(
            string firstName,
            string lastName,
            string contact,
            string address)
        {
            SetFirstName(firstName); 
            SetLastName(lastName); 
            SetContact(contact); 
            SetAddress(address);
        }
    }
}
