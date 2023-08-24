using ECommerceWebsite.Models;

namespace ECommerceWebsite.Repositories
{
    public interface IRepository
    {
        bool insertExcelFileSP(List<ExcelDataModel> excelData);
    }
}
