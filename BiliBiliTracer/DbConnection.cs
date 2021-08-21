using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBiliTracer
{
    public class DbConnection
    {
        private readonly IConfiguration _cfg;
        public DbConnection(IConfiguration configuration)
        {
            _cfg = configuration;
        }

        public SqlConnection New()
        {
            var conn = new SqlConnection(_cfg.GetConnectionString("Default"));
            conn.Open();
            return conn;
        }
    }
}
