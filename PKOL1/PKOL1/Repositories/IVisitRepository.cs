using PKOL1.Dtos.Response;

namespace PKOL1.Repositories;

public interface IVisitRepository
{
    public Task<int?> CreateVisitAsyn(int IdPatient, int IdDoctor, DateTime date);
}