using System.Data;
using System.Data.SqlClient;


namespace MVC_QLGV_SV
{
    public static class SQLHelper
    {
        public  static List<Dictionary<string, object>> ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(Global.connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameters(command, parameters);

                    return ExecuteReader(command);
                }
            }
        }

        public static List<Dictionary<string, object>> ExecuteQuery(string sqlQuery, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(Global.connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sqlQuery;

                    AddParameters(command, parameters);

                    return ExecuteReader(command);
                }
            }
        }

        private static void AddParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
        }

        private static List<Dictionary<string, object>> ExecuteReader(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

                while (reader.Read())
                {
                    var result = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object columnValue = reader.GetValue(i);

                        result[columnName] = columnValue;
                    }

                    results.Add(result);
                }

                return results;
            }
        }

    }
}
