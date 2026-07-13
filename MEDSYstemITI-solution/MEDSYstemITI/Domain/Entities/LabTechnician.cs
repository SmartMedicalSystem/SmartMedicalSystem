using Domain.Common;

namespace Domain.Entities
{
    public class LabTechnician : BaseEntity
    {
        public string Name { get;  set; } = null!;

        public string Contact { get;  set; } = null!;

        private LabTechnician() { }

        public LabTechnician(string name, string contact)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Contact = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
        }

        public void UpdateProfile(string name, string contact)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Contact = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
        }
    }
}
