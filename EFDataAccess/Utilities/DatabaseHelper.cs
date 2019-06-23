using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Data.SqlClient;

namespace EFDataAccess.Utilities
{
    public class DatabaseHelper
    {
        private UOW.UnitOfWork UnitOfWork;

        public DatabaseHelper()
        {
            UnitOfWork = new UOW.UnitOfWork();
        }

        public string GetConnectionString()
        {
            return UnitOfWork.DbContext.Database.Connection.ConnectionString + ";Connection Timeout=180";
        }

        public List<dynamic> ExecuteQuery(string query, ExpandoObject parameters)
        {
            var result = new List<dynamic>();
            using (SqlConnection connection = GetSQLConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    foreach (var member in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(member.Key, member.Value));
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(GetDynamicData(reader));
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }

        private dynamic GetDynamicData(SqlDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                expandoObject.Add(reader.GetName(i), reader[i]);
            }
            return expandoObject;
        }

        private SqlConnection GetSQLConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}