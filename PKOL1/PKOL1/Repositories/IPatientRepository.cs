using PKOL1.Dtos.Response;

namespace PKOL1.Repositories;

public interface IPatientRepository
{
    public Task<PatienVisitDTO?> getPatientVisitInfo(int id);
}