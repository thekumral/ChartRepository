using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace ChartProject.Api.Repositories
{
    public class ChartRepository : IChartRepository
    {
        private ConnectionInfoDto _connectionInfo;

        public void SetConnectionInfo(ConnectionInfoDto connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<IEnumerable<ChartData>> GetChartDataFromStoredProcedureAsync(string storedProcedureName, string connectionString, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var chartData = new List<ChartData>();
                        while (await reader.ReadAsync())
                        {
                            chartData.Add(new ChartData
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetDecimal(reader.GetOrdinal("Value")), // `decimal` olarak okuma
                                Category = reader.GetString(reader.GetOrdinal("Category"))
                            });
                        }
                        return chartData;
                    }
                }
            }
        }


        public async Task<IEnumerable<ChartData>> GetChartDataFromFunctionAsync(string functionName, string connectionString, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = $"SELECT * FROM dbo.{functionName}(@MinValue)";
                var sqlParameters = new DynamicParameters();
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        sqlParameters.Add($"@{param.Key}", param.Value, DbType.Single);
                    }
                }
                return await connection.QueryAsync<ChartData>(query, sqlParameters);
            }
        }

        public async Task<IEnumerable<ChartData>> GetChartDataFromViewAsync(string viewName, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = $"SELECT * FROM dbo.{viewName}";
                return await connection.QueryAsync<ChartData>(query);
            }
        }

        public async Task AddChartDataAsync(ChartData chartData)
        {
            var connectionInfo = GlobalConnectionInfo.ConnectionInfo;
            if (connectionInfo == null)
            {
                throw new InvalidOperationException("Connection info is not set.");
            }

            using (var connection = new SqlConnection($"Server={connectionInfo.ServerName};Database={connectionInfo.DatabaseName};Integrated Security=True;TrustServerCertificate=True;"))
            {
                var query = "INSERT INTO dbo.ChartData (Label, Value, Category) VALUES (@Label, @Value, @Category)";
                await connection.ExecuteAsync(query, new { chartData.Label, chartData.Value, chartData.Category });
            }
        }
        public async Task<dynamic> GetDataSourcesAsync(ConnectionInfoDto connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new InvalidOperationException("Connection info is not set.");
            }

            var connectionString = $"Server={connectionInfo.ServerName};Database={connectionInfo.DatabaseName};Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var viewsTask = connection.QueryAsync<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS");
                var functionsTask = connection.QueryAsync<string>("SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION'");
                var storedProceduresTask = connection.QueryAsync<string>("SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'");

                await Task.WhenAll(viewsTask, functionsTask, storedProceduresTask);

                return new
                {
                    Views = await viewsTask,
                    Functions = await functionsTask,
                    StoredProcedures = await storedProceduresTask
                };
            }
        }
    }
}
