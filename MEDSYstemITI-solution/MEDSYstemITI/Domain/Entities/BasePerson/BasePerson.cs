
namespace Domain.Entities.Baseperson
{
    // Confirmed: BasePerson currently adds nothing on top of BaseEntity.
    // Kept as its own abstract class (instead of merging into BaseEntity) so
    // person-type entities (Doctor, and any future person entity) can be
    // extended or queried as a group later without touching BaseEntity,
    // which is also used by non-person entities (LabTest, Session, etc.).
    public abstract class BasePerson : BaseEntity
    {
    }
}
