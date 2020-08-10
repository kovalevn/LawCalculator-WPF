using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;

namespace LawCalculator_WPF
{
    class SqliteDataAccess
    {
        public static List<Lawyer> LoadLawyers(string table)
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Lawyer>($"select * from {table}", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveLawyers(Lawyer lawyer)
        {
            if (lawyer != null && lawyer.Name != null && lawyer.Salary != null)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("insert into Lawyers (Name, Salary) values (@Name, @Salary)", lawyer);
                }
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
