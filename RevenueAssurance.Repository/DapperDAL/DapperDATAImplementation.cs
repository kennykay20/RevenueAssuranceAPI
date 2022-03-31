

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Repository.DapperDAL
{

    public class DapperDATAImplementation<T> : IDapperDATA<T> where T : class
    {


        public async Task<IEnumerable<T>> LoadData(string procName, DynamicParameters param, IDbConnection db)
        {
            var result = (await db.QueryAsync<T>(sql: procName,
               commandType: CommandType.StoredProcedure
               , param: param)).ToList();

            return result;
        }

        public async Task<List<T>> LoadListNoParam(string script, IDbConnection db)
        {
            var record = (await db.QueryAsync<T>(sql: script,
               commandType: CommandType.Text
               )).ToList();

            return record;
        }

        public async Task<T> LoadParam(string script, IDbConnection db)
        {
            var record = (await db.QueryFirstOrDefaultAsync<T>(sql: script,
               commandType: CommandType.Text
               ));

            return record;
        }






        public async Task<T> ResponseObj(string procName, DynamicParameters param, IDbConnection db)
        {
            var result = (await db.QueryAsync<T>(sql: procName,
               commandType: CommandType.StoredProcedure
               , param: param)).FirstOrDefault();

            return result;
        }


        public async Task<T> LoadSingle(string procName, IDbConnection db)
        {
            var result = (await db.QueryAsync<T>(sql: procName,
               commandType: CommandType.Text
               )).FirstOrDefault();

            return result;
        }

        public async Task<int> ResponseValueInt(string procName, DynamicParameters param, IDbConnection db)
        {
            var result = (await db.QueryAsync<int>(sql: procName,
             commandType: CommandType.StoredProcedure
             , param: param)).FirstOrDefault();

            return result;
        }

        public async Task<string> ResponseValueStr(string procName, DynamicParameters param, IDbConnection db)
        {
            var result = (await db.QueryAsync<string>(sql: procName,
             commandType: CommandType.StoredProcedure
             , param: param)).FirstOrDefault();

            return result;
        }




    }


}