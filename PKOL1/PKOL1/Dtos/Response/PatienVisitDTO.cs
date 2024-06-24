using PKOL1.Models;

namespace PKOL1.Dtos.Response;

public class PatienVisitDTO
{
    
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime Birthdate { get; set; }
    public double TotalAmountMoneySpent { get; set; }
    public int numberOfVisits { get; set; }
    public List<VisitDTO> Visits { get; set; }
}