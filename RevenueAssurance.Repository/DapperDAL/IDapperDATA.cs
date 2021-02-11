using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace RevAssuranceApi.RevenueAssurance.Repository.DapperDAL
{
    
    public interface IDapperDATA<T> where T : class
    {
          Task<IEnumerable<T>> LoadData(string procName, DynamicParameters param,  IDbConnection db);
          Task<T> ResponseObj(string procName, DynamicParameters param,  IDbConnection db);
          Task<int> ResponseValueInt(string procName, DynamicParameters param,  IDbConnection db);

          Task<string> ResponseValueStr(string procName, DynamicParameters param,  IDbConnection db);
    }
}