using System;
using System.Collections.Generic;

namespace Hall_Of_Fame.DTO
{
    public class PersonResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<SkillResponseDto> Skills { get; set; }
    }
}
