using System.ComponentModel.DataAnnotations;

namespace MedicalCareForm.Data.Models
{
    public class DictionaryBaseType : BaseEntity
    {
        [Display(Name = "Код")]
        public int Code { get; set; }

        [Display(Name = "Начало")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Окончание")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;

        public DictionaryBaseType()
        {
        }
    }
}