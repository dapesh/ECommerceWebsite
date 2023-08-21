using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class ExcelDataModel
    {
        [Key]
        public string Id { get; set; } =Guid.NewGuid().ToString();
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3{ get; set; }
        public string Column4 { get; set; }

    }
}
