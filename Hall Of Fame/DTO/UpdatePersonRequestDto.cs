using System.ComponentModel.DataAnnotations;

namespace Hall_Of_Fame.DTO
{
    public class UpdatePersonRequestDto
    {
        [Required(ErrorMessage = "Идентификатор не указан")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите ваше отображаемое имя")]
        public string DisplayName { get; set; }

        public ICollection<SkillResponseDto> Skills { get; set; }

    }
}
