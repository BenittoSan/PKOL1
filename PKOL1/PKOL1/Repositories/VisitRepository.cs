using Microsoft.Data.SqlClient;
using PKOL1.Dtos.Response;

namespace PKOL1.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly IConfiguration _configuration;

    public VisitRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<int?> CreateVisitAsyn(int IdPatient, int IdDoctor, DateTime date)
    {

        using (var con = new SqlConnection(_configuration.GetConnectionString("KOL1")))
        {
            await con.OpenAsync();

            using (var transaction =  con.BeginTransaction())
            {

                try
                {
                    using (var cmd = new SqlCommand($"SELECT COUNT(*) from Patient where IdPatient = @IdPatient",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@IdPatient", IdPatient);
                        int isExistPatient = await cmd.ExecuteNonQueryAsync();
                        if (isExistPatient == 0)
                        {
                            throw new Exception("Patient do not exist");
                        }
                        
                    }

                    using (var cmd = new SqlCommand($"SELECT COUNT(*) from Doctor where IdDoctor = @IdDoctor",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@IdDoctor", IdDoctor);
                        int isDoctorExist = await cmd.ExecuteNonQueryAsync();
                        if (isDoctorExist == 0)
                        {
                            throw new Exception("Doctor do not exist");
                        }
                        
                    }

                    using (var cmd = new SqlCommand($"SELECT COUNT(*) FROM Visit v JOIN Patient p ON v.IdPatient = p.IdPatient" +
                                                    $" WHERE v.Date > @CurentDate AND p.IdPatient = @IdPatient", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CurentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@IdPatient", IdPatient);

                        int checkDate = await cmd.ExecuteNonQueryAsync();
                        if (checkDate >= 0)
                        {
                            throw new Exception("Patien allready have a visit");
                        }

                    }

                    using (var cmd = new SqlCommand($"SELECT COUNT(*) FROM Schedule WHERE IdDoctor = @IdDoctor AND DateFrom < @data AND DateTo > @data"))
                    {
                        cmd.Parameters.AddWithValue("@IdDoctor", IdDoctor);
                        cmd.Parameters.AddWithValue("@data", date);

                        int isDoctorWorking = await cmd.ExecuteNonQueryAsync();
                        if (isDoctorWorking == 0)
                        {
                            throw new Exception($"Doctor with id {IdDoctor} not working in:  {date}");
                        }
                    }

                    using (var cmd = new SqlCommand($"INSERT INTO Visit (Date, IdPatient, IdDoctor, Price) VALUES (@date,@IdPatient,@IdDoctor,200);" +
                                                    $"SELECT SCOPE_IDENTITY();",con,transaction))
                    {
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@IdPatient", IdPatient);
                        cmd.Parameters.AddWithValue("@IdDoctor", IdDoctor);
                        
                    }
                    int justCreatedId = 0;
                    using (var cmd = new SqlCommand($"SELECT MAX(IdVisit) FROM  Visit",con,transaction))
                    {
                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            justCreatedId = dr.GetInt32(0);
                        }
                        
                    }

                    

                    await transaction.CommitAsync();
                    return justCreatedId;



                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
                
            }
            
        }

        return null;
    }
}