using System.ComponentModel.DataAnnotations;

namespace Hall_Of_Fame.DTO
{
    public class PersonResponse
    {
       
        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите ваше отображаемое имя")]
        public string DisplayName { get; set; }
        public virtual ICollection<SkillResponse> Skills { get; set; }
    }
}