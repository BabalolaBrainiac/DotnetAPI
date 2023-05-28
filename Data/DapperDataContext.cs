using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace DotnetAPI.Data
{
    public class DapperDataContext
    {
        private readonly IConfiguration _config;

       public DapperDataContext(IConfiguration config) {
            _config = config;
        }


        public IEnumerable<T> LoadData<T>(string sql) 
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return dbConnection.Query<T>(sql);
        }

         public T LoadSingle<T>(string sql) 
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSqlWithParameters(string query, List<SqlParameter> queryParameters)
        {
            SqlCommand sqlCommand = new SqlCommand(query);

            foreach (SqlParameter parameter in queryParameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }

            SqlConnection databaseConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            databaseConnection.Open();

            sqlCommand.Connection = databaseConnection;
            int rowsAltered = sqlCommand.ExecuteNonQuery();

            databaseConnection.Close();

            return rowsAltered > 0;

        }


        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }
    }
}