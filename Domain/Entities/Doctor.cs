using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace Domain.Entities
{
    public class Doctor:BaseEntity
    {
        private Doctor() { }
        public Doctor(string name,string specialization,string contact,int deptId)
        {
            SetName(name);
            Setcontact(contact);
            SetDeptId(deptId);
            SetSpecialization(specialization);

        }

        public void Update(string name,string specialization,string contact,int deptId)
        {
            SetName(name);
            Setcontact(contact);
            SetDeptId(deptId);
            SetSpecialization(specialization);
            UpdatedAt = DateTime.UtcNow;

        }

        public void SetName(string _name)
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new ArgumentException("name is required", nameof(Name));
            }
            Name = _name.Trim();

        }
        public void SetSpecialization(string _specialization)
        {
            if (string.IsNullOrWhiteSpace(_specialization))
            {
                throw new ArgumentException("Specialization is required", nameof(Specialization));

            }
            Specialization = _specialization.Trim();

        }
        public void Setcontact(string _contact)
        {
            if (string.IsNullOrWhiteSpace(_contact))
            {
                throw new ArgumentException("contact is required", nameof(Contact));

            }

            if (_contact.Length != 11)
            {
                throw new ArgumentException("Phone number must be exactly 11 digits", nameof(Contact));
            }
            if (!_contact.All(char.IsDigit))
            {
                throw new ArgumentException("Phone number must contain only digits", nameof(Contact));
            }
            if (!_contact.StartsWith("010") &&
                !_contact.StartsWith("011") &&
                !_contact.StartsWith("012") &&
                !_contact.StartsWith("015"))
            {
                throw new ArgumentException("Invalid phone number", nameof(Contact));
            }
            Contact = _contact.Trim();

        }
        public void SetDeptId(int _depdId)
        {
            if (_depdId <= 0)
            {
                throw new ArgumentException("Department Id can not be zero or less", nameof(DepartmentId));
            }
           
            DepartmentId = _depdId;
        }
        public void Delete()
        {
            IsDeleted = true;
        }
        public string Name { get; private set; }

        public string Specialization { get; private set; }

        public string Contact { get; private set; } 

        public int DepartmentId { get; private set; }            //FK ====>Departmrnt

        // Navigation Properties

       // public Department Department { get; set; }

        //public ICollection<Session> Sessions { get; set; }
        //    = new List<Session>();

        //public ICollection<AIReport> AIReports { get; set; }
        //    = new List<AIReport>();

    }
}
