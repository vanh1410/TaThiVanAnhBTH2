using System.ComponentModel.DataAnnotations;

namespace TaThiVanAnhBTH2.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeAge { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeeAddress { get; set; }
    }
}
