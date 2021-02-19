using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _05.ChangeTownNamesCasing
{
    class Program
    {
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            List<string> targetTownNames = new List<string>();
            int countOfTownsAffected = 0;

            string dbConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";
            SqlConnection dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using (dbConnection)
            {
                SqlCommand updateTownNamesQuery =
                            new SqlCommand
                            (@"UPDATE Towns
                              SET Name = UPPER(Name)
                            WHERE CountryCode = 
                            (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)",
                            dbConnection);

                updateTownNamesQuery.Parameters.AddWithValue("@countryName", countryName);

                countOfTownsAffected = updateTownNamesQuery.ExecuteNonQuery();

                if (countOfTownsAffected > 0)
                {
                    SqlCommand getTargetTownNamesQuery =
                            new SqlCommand
                            (@"SELECT t.Name
                             FROM Towns as t
                             JOIN Countries AS c ON c.Id = t.CountryCode
                            WHERE c.Name = @countryName", 
                            dbConnection);

                    getTargetTownNamesQuery.Parameters.AddWithValue("@countryName", countryName);

                    SqlDataReader dbReader = getTargetTownNamesQuery.ExecuteReader();


                    using (dbReader)
                    {
                        while (dbReader.Read())
                        {
                            targetTownNames.Add((string)dbReader["Name"]);
                        }
                    }

                    Console.WriteLine($"{targetTownNames.Count} town names were affected.");
                    Console.WriteLine($"[{string.Join(", ", targetTownNames)}]");
                }
                else
                {
                    Console.WriteLine("No town names were affected.");
                }
            }
        }
    }
}
