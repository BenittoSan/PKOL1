using Microsoft.AspNetCore.Mvc;
using PKOL1.Repositories;

namespace PKOL1.Controllers;

[Route("api/patients")]
[ApiController]
public class PatientController : ControllerBase
{

    private readonly IPatientRepository _patientRepository;

    public PatientController(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPatientInfo(int id)
    {
        var patientInfo = await _patientRepository.getPatientVisitInfo(id);

        if (patientInfo == null)
        {
            return NotFound($"Patient with id {id} do not exists");
        }

        return Ok(patientInfo);
    }

}