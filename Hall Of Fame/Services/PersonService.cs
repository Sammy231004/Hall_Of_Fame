using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hall_Of_Fame.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<PersonResponseDto> CreatePerson(CreatePersonRequestDto request)
        {
            var person = new Person()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Skills = request.Skills.Select(x => new Skills() { Name = x.Name, Level = x.Level }).ToList(),
            };
            var createdPerson = await _personRepository.CreatePerson(person);
            return new PersonResponseDto
            {
            
                Name = createdPerson.Name,
                DisplayName = createdPerson.DisplayName,
                Skills = createdPerson.Skills.Select(x => new SkillResponseDto { Name = x.Name, Level = x.Level }).ToList(),
            };
        }




        public async Task<PersonResponseDto> GetPersonById(long id)
        {
            var person = await _personRepository.GetPersonById(id);
            if (person == null)
            {
                return null; 
            }

            return new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillResponseDto { Name = skill.Name, Level = skill.Level }).ToList()
            };
        }

        public async Task<IEnumerable<PersonResponseDto>> GetPeople()
        {
            var persons = await _personRepository.GetPeople();
            var personResponses = persons.Select(person => new PersonResponseDto
            {
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillResponseDto
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToList()
            });

            return personResponses;
        }

        public async Task<PersonResponseDto> UpdatePerson(long id, UpdatePersonRequestDto personRequest)
        {
            var existingPerson = await _personRepository.GetPersonById(id);

            if (existingPerson == null)
            {
                return null;
            }

            existingPerson.Name = personRequest.Name;
            existingPerson.DisplayName = personRequest.DisplayName;

           
            existingPerson.Skills = personRequest.Skills.Select(x => new Skills { Name = x.Name, Level = x.Level }).ToList();

            var updatedPerson = await _personRepository.UpdatePerson(id, existingPerson);

            return new PersonResponseDto
            {
                Id = updatedPerson.Id,
                Name = updatedPerson.Name,
                DisplayName = updatedPerson.DisplayName,
                Skills = updatedPerson.Skills.Select(skill => new SkillResponseDto { Name = skill.Name, Level = skill.Level }).ToList()
            };
        }


        public async Task DeletePersonById(long id)
        {
            await _personRepository.DeletePersonById(id);
        }
    }
}
