using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.Interface;
using Hall_Of_Fame.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Hall_Of_Fame.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<IPersonService> _logger;
        public PersonService(IPersonRepository personRepository, ILogger<IPersonService> logger)
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
            var personResponse = new PersonResponse()
            {

                Name = createdPerson.Name,
                DisplayName = createdPerson.DisplayName,
                Skills = createdPerson.Skills.Select(x => new SkillResponse() { Name = x.Name, Level = x.Level }).ToList(),
            };
            return personResponse;
        }

        public async Task<ActionResult> DeletePersonById(long id)
        {
            await _personRepository.DeletePersonById(id);
            return new NoContentResult();
        }

        public async Task<ActionResult> GetPeople()
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

        public async Task<ActionResult> GetPersonById(long id)
        {
            var person = await _personRepository.GetPersonById(id);
            if (person == null)
            {
                return new NotFoundResult();
            }

            var personResponse = new PersonResponse
            {

                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillResponse
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToList()
            };

            return new OkObjectResult(personResponse);
        }

        public async Task<ActionResult> UpdatePerson(long id, PersonRequest personRequest)
        {


            var updatedPerson = await _personRepository.UpdatePerson(id, personRequest);
            if (updatedPerson == null)
            {
                return new NotFoundResult();
            }

            var personResponse = new PersonResponse
            {

                Name = updatedPerson.Name,
                DisplayName = updatedPerson.DisplayName,
                Skills = updatedPerson.Skills.Select(skill => new SkillResponse
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToList()
            };

            return new OkObjectResult(personResponse);
        }

    }

}