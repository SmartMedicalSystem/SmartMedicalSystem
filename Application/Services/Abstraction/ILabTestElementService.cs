using Application.DTOs.LabTestElement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface ILabTestElementService
    {
        Task<LabTestElementReadDto> AddElementToTestAsync(LabTestElementCreateDto dto);
        Task<IEnumerable<LabTestElementReadDto>> GetByLabTestAsync(int labTestId);
        Task RemoveElementFromTestAsync(int labTestId, int testElementId);
    }
}
