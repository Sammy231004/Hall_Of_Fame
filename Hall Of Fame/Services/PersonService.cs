using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.Interface;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// теперь возвращает DTO объекты, вместо ActionResult
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

        public async Task<PersonResponse> CreatePerson(PersonRequest personRequest)
        {
            var person = new Person()
            {
                Name = personRequest.Name,
                DisplayName = personRequest.DisplayName,
                Skills = personRequest.Skills.Select(x => new Skills() { Name = x.Name, Level = x.Level }).ToList(),
            };
            var createdPerson = await _personRepository.CreatePerson(person);
            return new PersonResponse
            {
                Name = createdPerson.Name,
                DisplayName = createdPerson.DisplayName,
                Skills = createdPerson.Skills.Select(x => new SkillResponse { Name = x.Name, Level = x.Level }).ToList(),
            };
        }


        public async Task<IActionResult> GetPersonById(long id)
        {
            var person = await _personRepository.GetPersonById(id);
            if (person == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(new PersonResponse
            {
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillResponse { Name = skill.Name, Level = skill.Level }).ToList()
            });
        }

        public async Task<IActionResult> GetPeople()
        {
            var persons = await _personRepository.GetPeople();
            var personResponses = persons.Select(person => new PersonResponse
            {
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillResponse
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToList()
            }).ToList();

            return new OkObjectResult(personResponses);
        }

        public async Task<IActionResult> UpdatePerson(long id, PersonRequest personRequest)
        {
            var updatedPerson = await _personRepository.UpdatePerson(id, personRequest);
            if (updatedPerson == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(new PersonResponse
            {
                Name = updatedPerson.Name,
                DisplayName = updatedPerson.DisplayName,
                Skills = updatedPerson.Skills.Select(skill => new SkillResponse
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToList()
            });
        }

        public async Task<IActionResult> DeletePersonById(long id)
        {
            await _personRepository.DeletePersonById(id);
            return new NoContentResult();
        }

    }
}
