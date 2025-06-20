using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Rest_Api_A3.Repositories 
{ 
    public class BaseRepository<T> where T : class 
    {
        private readonly string _connectionString;
        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Executes a SQL query, returns a sequence of entities.    
        public async Task<IEnumerable<T>> GetAsync(string query)
        {
            await using var connection = new SqlConnection(_connectionString); 
            return await connection.QueryAsync<T>(query);
        }

        // Executes a SQL query, returns a single entity (or null).
        public async Task<T> GetAsync(string query, object parameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }





        // Executes a SQL query and maps to a sequence of U.
        public async Task<IEnumerable<U>> QueryAsync<U>(string sql, object parameters = null)
        {
            await using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<U>(sql, parameters);
        }

        // Executes a SQL query and maps to a single U (or null).
        public async Task<U> QuerySingleOrDefaultAsync<U>(string sql, object parameters = null)
        {
            await using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<U>(sql, parameters);
        }





        // Executes an INSERT/UPDATE/DELETE command, returns number of rows affected.
        public async Task<int> ExecuteAsync(string query, object parameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(query, parameters);
        }

        // Executes an INSERT (which have OUTPUT INSERTED.Id), returns the new identity.
        public async Task<int> CreateAsync(string sqlWithOutput, object parameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sqlWithOutput, parameters);
        }

        // Executes a stored procedure, returns number of rows affected.
        public async Task<int> ExecuteProcAsync(string procName, object parameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(
                procName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Executes a stored procedure and returns value of a specified OUTPUT parameter (e.g. newly created record’s ID).
        public async Task<int> ExecuteProcWithOutputAsync(string procName, DynamicParameters parameters, string outputParamName)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                procName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>(outputParamName);
        }
    }
}

