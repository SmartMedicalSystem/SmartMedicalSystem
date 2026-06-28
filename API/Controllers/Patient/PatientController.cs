using Application.DTOs.Patient;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService patientService;

        public PatientsController(IPatientService _patientService)
        {
            patientService = _patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await patientService.GetAllAsync();

            return Ok(patients);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await patientService.GetByIdAsync(id);

            return Ok(patient);
        }
        [HttpGet("ssn/{ssn}")]
        public async Task<IActionResult> GetBySsn(string ssn)
        {
            var patient = await patientService.GetBySSNAsync(ssn);
            return Ok(patient);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePatientDTO dto)
        {
            await patientService.CreateAsync(dto);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePatientDTO dto)
        {
            await patientService.UpdateAsync(dto);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await patientService.DeleteAsync(id);

            return NoContent();
        }
    }
}