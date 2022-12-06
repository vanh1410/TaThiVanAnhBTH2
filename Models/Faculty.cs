using System.ComponentModel.DataAnnotations;

namespace TaThiVanAnhBTH2.Models
{
    public class Faculty
    {
        [Key]
        public string FacultyID { get; set; }
        public string FacultyName { get; set; }
    }
}
