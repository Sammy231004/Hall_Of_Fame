using System.ComponentModel.DataAnnotations;

namespace Hall_Of_Fame.DTO
{
    public class SkillResponseDto
    {
        [Required(ErrorMessage = "Ввдетие название навыка")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите уровень навыка")]

        [Range(1, 25, ErrorMessage = "Уровень навыка может быть от 1 до 25")]
        public byte Level { get; set; }
    }
}