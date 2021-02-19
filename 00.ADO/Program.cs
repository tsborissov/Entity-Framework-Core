using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _00.ADO
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees = new List<Employee>();

            string connectionString =
                "Server = (localdb)\\MSSQLLocalDB;" +
                "Database = SoftUni;" +
                "Integrated Security = true";

            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();

            using(dbConnection)
            {
                SqlCommand dbCommand = new SqlCommand(@"SELECT COUNT(*) FROM Employees", dbConnection);

                int employeesCount = (int)dbCommand.ExecuteScalar();

                Console.WriteLine($"Employees count: {employeesCount}");
            }
        }
    }
}
