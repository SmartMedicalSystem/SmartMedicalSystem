using Domain.Entities.Baseperson;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPersonGenericRepo
    {
        Task<BasePerson?> FindBySSN(string ssn);
        Task AddPerson(string ssn, BasePerson person);
        Task UpdateSSNAsync(BasePerson person, string ssn);


    }
}