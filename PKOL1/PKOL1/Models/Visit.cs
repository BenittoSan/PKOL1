namespace PKOL1.Models;

public class Visit
{
    public int IdVisit { get; set; }
    public DateTime Date { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    public double Price { get; set; }
}