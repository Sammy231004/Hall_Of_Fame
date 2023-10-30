using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hall_Of_Fame.DTO
{
    public class CreatePersonRequestDto
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите ваше отображаемое имя")]
        public string DisplayName { get; set; }

        public ICollection<SkillRequestDto> Skills { get; set; }
    }
}
