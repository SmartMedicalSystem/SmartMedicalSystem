using Domain.Entities.Person;

namespace Domain.Entities;

public class AdminPerson : BasePerson
{
    public AdminPerson(
        string firstName,
        string lastName,
        DateTime dateOfBirth)
        : base(
            firstName,
            lastName,
            dateOfBirth)
    {
    }

    protected AdminPerson()
    {
    }
}