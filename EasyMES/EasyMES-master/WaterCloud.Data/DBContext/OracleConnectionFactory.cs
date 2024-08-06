using Chloe.Infrastructure;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace WaterCloud.DataBase
{
    public class OracleConnectionFactory : IDbConnectionFactory
    {
        string _connString = null;
        public OracleConnectionFactory(string connString)
        {
            this._connString = connString;
        }
        public IDbConnection CreateConnection()
        {
            OracleConnection oracleConnection = new OracleConnection(this._connString);
            OracleConnectionDecorator conn = new OracleConnectionDecorator(oracleConnection);
            return conn;
        }
    }
}