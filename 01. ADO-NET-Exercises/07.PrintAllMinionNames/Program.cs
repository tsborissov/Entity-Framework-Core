using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _07.PrintAllMinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            var minionsNames = new List<string>();
            var minionsNamesQuery = @"SELECT Name FROM Minions";

            var dbConnectionString = @"Server=(localdb)\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";
            var dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            
            using(dbConnection)
            {
                var minionsNamesCommand = new SqlCommand(minionsNamesQuery, dbConnection);
                var dbReader = minionsNamesCommand.ExecuteReader();

                using (dbReader)
                {
                    while(dbReader.Read())
                    {
                        minionsNames.Add((string)dbReader["Name"]);
                    }
                }
            }

            for (int i = 0; i < minionsNames.Count / 2; i++)
            {
                Console.WriteLine(minionsNames[i]);
                Console.WriteLine(minionsNames[(minionsNames.Count - 1) - i]);
            }

            if (minionsNames.Count % 2 != 0)
            {
                Console.WriteLine(minionsNames[minionsNames.Count / 2]);
            }
        }
    }
}
