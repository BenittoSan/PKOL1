using Microsoft.Data.SqlClient;
using PKOL1.Dtos.Response;
using PKOL1.Models;

namespace PKOL1.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IConfiguration _configuration;

    public PatientRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<PatienVisitDTO?> getPatientVisitInfo(int id)
    {
        Patient patient = new Patient();
        List<VisitDTO?> visitDtos = new List<VisitDTO?>();
        int numberOfVisit = 0;
        double amoutPrice = 0.0;
        

        using (var con = new SqlConnection(_configuration.GetConnectionString("KOL1")))
        {
            await con.OpenAsync();

            using (var transaction =  con.BeginTransaction())
            {

                try
                {
                    using (var cmd = new SqlCommand($"SELECT COUNT(*) from Patient where IdPatient = @id ",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int a = await cmd.ExecuteNonQueryAsync();

                        if (a == 0)
                        {
                            return null;
                        }

                    }
                    
                    
                    
                    using (var cmd = new SqlCommand($"SELECT IdPatient,FirstName,LastName,Birthdate FROM Patient WHERE IdPatient = @id",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                patient.IdPatient = dr.GetInt32(0);
                                patient.FirstName = dr.GetString(1);
                                patient.LastName = dr.GetString(2);
                                patient.Birthdate = dr.GetDateTime(3);
                            }
                            
                        }
                        
                    }

                    using (var cmd = new SqlCommand($"SELECT COUNT(*) FROM Visit WHERE IdPatient = @id",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                numberOfVisit = dr.GetInt32(0);
                            }
                            else
                            {
                                numberOfVisit = 0;
                            }
                        }
                        
                    }
                    using (var cmd = new SqlCommand($"SELECT SUM(Price) FROM Visit WHERE IdPatient = @id",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                amoutPrice = dr.GetDouble(0);
                            }
                            else
                            {
                                amoutPrice = 0.0;
                            }
                        }
                        
                    }
                    using (var cmd = new SqlCommand($"SELECT v.IdVisit,d.FirstName,d.LastName,v.Date,v.Price FROM Visit v JOIN Doctor d ON v.IdDoctor=d.IdDoctor WHERE IdPatient = @id",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                var visit = new VisitDTO
                                {
                                    IdVisit = dr.GetInt32(0),
                                    Doctor = dr.GetString(1) + " " + dr.GetString(2),
                                    Date = dr.GetDateTime(3),
                                    Price = dr.GetDouble(4)
                                };
                                visitDtos.Add(visit);
                            }
                        }
                        
                    }

                    PatienVisitDTO patientInfo = new PatienVisitDTO
                    {
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        Birthdate = patient.Birthdate,
                        TotalAmountMoneySpent = amoutPrice,
                        numberOfVisits = numberOfVisit,
                        Visits = visitDtos
                    };

                    await transaction.CommitAsync();
                    return patientInfo;





                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
                
                
                
            }

        }
        
    }
}