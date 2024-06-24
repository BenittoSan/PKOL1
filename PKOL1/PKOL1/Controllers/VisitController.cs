using Microsoft.AspNetCore.Mvc;
using PKOL1.Repositories;

namespace PKOL1.Controllers;

[Route("api/visits")]
[ApiController]
public class VisitController : ControllerBase
{
    private IVisitRepository _visitRepository;

    public VisitController(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    [HttpPost("{IdPatient:int},{IdDoctor:int},{date:datetime}")]
    public async Task<IActionResult> CreateVisit(int IdPatient, int IdDoctor, DateTime date)
    {
        var idNewVist = await _visitRepository.CreateVisitAsyn(IdPatient, IdDoctor, date);

        if (idNewVist == null)
        {
            return BadRequest($"Sorry Wrong Data ");
        }

        return Ok(idNewVist);
    }

}