using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace _03.MinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            string dbConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";
            SqlConnection dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using(dbConnection)
            {
                StringBuilder sb = new StringBuilder();

                bool isVillian = false;

                SqlCommand getVillainNameQuery = new SqlCommand("SELECT Name FROM Villains WHERE Id = @Id", dbConnection);

                getVillainNameQuery.Parameters.AddWithValue("@Id", villainId);

                string villainName = (string)getVillainNameQuery.ExecuteScalar();

                if (villainName != null)
                {
                    sb.AppendLine($"Villain: {villainName}");

                    isVillian = true;
                }
                else
                {
                    sb.AppendLine($"No villain with ID {villainId} exists in the database.");
                }


                if(isVillian)
                {
                    SqlCommand getMinionsQuery = new SqlCommand
                    (@"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name", dbConnection);

                    getMinionsQuery.Parameters.AddWithValue("@Id", villainId);

                    SqlDataReader dbReader = getMinionsQuery.ExecuteReader();

                    using (dbReader)
                    {
                        bool hasMinions = false;

                        while (dbReader.Read())
                        {
                            long rowNr = (long)dbReader["RowNum"];
                            string minionName = (string)dbReader["Name"];
                            int age = (int)dbReader["Age"];

                            sb.AppendLine($"{rowNr}. {minionName} {age}");

                            hasMinions = true;
                        }

                        if (!hasMinions)
                        {
                            sb.AppendLine("(no minions)");
                        }
                    }
                }

                Console.WriteLine(sb.ToString());
            }
        }
    }
}
