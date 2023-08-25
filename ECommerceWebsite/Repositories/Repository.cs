using Dapper;
using ECommerceWebsite.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ECommerceWebsite.Repositories
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;
        public Repository(string connectionString)
        {

            _connectionString = connectionString;

        }
        //public bool insertExcelFileSP(List<ExcelDataModel> excelData)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();
        //            var parameters = new DynamicParameters();
        //            parameters.AddDynamicParams(new ListParameter<ExcelDataModel>("@DataToAdd", excelData));
        //            var result = connection.Execute("sp_ExcelUploads", parameters, commandType: CommandType.StoredProcedure);
        //            //var result = connection.QueryFirstOrDefault<int>("sp_ExcelUploads", parameters, commandType: CommandType.StoredProcedure);
        //            return result > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    //throw new NotImplementedException();
        //}
        //public class ListParameter<T> : SqlMapper.IDynamicParameters
        //{
        //    private readonly string _name;
        //    private readonly IEnumerable<T> _values;

        //    public ListParameter(string name, IEnumerable<T> values)
        //    {
        //        _name = name;
        //        _values = values;
        //    }

        //    public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        //    {
        //        var dataTable = new DataTable();
        //        dataTable.Columns.Add("Value", typeof(T));

        //        foreach (var value in _values)
        //        {
        //            dataTable.Rows.Add(value);
        //        }

        //        var parameter = ((SqlCommand)command).Parameters.Add(_name, SqlDbType.Structured);
        //        parameter.Value = dataTable;
        //        parameter.TypeName = "dbo.ExcelDataModelTypes"; 
        //    }
        //}



        public bool insertExcelFileSP(List<ExcelDataModel> excelData)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DataToAdd", ToDataTable(excelData), DbType.Object);

                    var result = connection.Execute("sp_InsertExcelData", parameters, commandType: CommandType.StoredProcedure);

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private DataTable ToDataTable(List<ExcelDataModel> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Column1");
            dt.Columns.Add("Column2");
            dt.Columns.Add("Column3");
            dt.Columns.Add("Column4");
            dt.Columns.Add("Column5");

            foreach (var item in data)
            {
                DataRow row = dt.NewRow();
                row["Id"] = item.Id;
                row["Column1"] = item.Column1;
                row["Column2"] = item.Column2;
                row["Column3"] = item.Column3;
                row["Column4"] = item.Column4;
                row["Column5"] = item.Column5;
                dt.Rows.Add(row);
            }
            return dt;

        }
    }
}
