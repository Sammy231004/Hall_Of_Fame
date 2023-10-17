using Xunit;
using Moq;
using Hall_Of_Fame.Services;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PersonServiceTests
{
    [Fact]
    public async Task CreatePerson_ReturnsPersonResponse()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        var personRequest = new PersonRequest
        { 
            Name = "Vitalik",
            DisplayName = "Savinih",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "Огненный шар", Level = 5 },
                new SkillRequest { Name = "Расширение территории", Level = 4 }
            }
        };
        var personEntity = new Person
        {
            Name = personRequest.Name,
            DisplayName = personRequest.DisplayName,
            Skills = personRequest.Skills.Select(s => new Skills { Name = s.Name, Level = s.Level }).ToList()
        };

        personRepositoryMock.Setup(repo => repo.CreatePerson(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);


        var result = await personService.CreatePerson(personRequest) as PersonResponse;

    
        Assert.NotNull(result);
        Assert.Equal(personEntity.Name, result.Name);
        Assert.Equal(personEntity.DisplayName, result.DisplayName);
        Assert.Equal(personEntity.Skills.Count, result.Skills.Count);
    }

    [Fact]
    public async Task DeletePersonById_ReturnsNoContentResult()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        long personId = 1;


        var result = await personService.DeletePersonById(personId) as NoContentResult;

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPeople_ReturnsOkObjectResult()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);

        var persons = new List<Person>
        {
            new Person
            {
                Name = "Mark",
                DisplayName = "Karlec",
                Skills = new List<Skills>
                {
                    new Skills { Name = "Лидерство", Level = 5 },
                    new Skills { Name = "Коммуникабельность", Level = 4 }
                }
            }

        };

        personRepositoryMock.Setup(repo => repo.GetPeople())
            .ReturnsAsync(persons);


        var result = await personService.GetPeople() as OkObjectResult;

        Assert.NotNull(result);
        var personResponses = result.Value as List<PersonResponse>;
        Assert.NotNull(personResponses);
    
    }

    [Fact]
    public async Task GetPersonById_ReturnsOkObjectResult()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        long personId = 1;

        var person = new Person
        {
            Name = "Alex",
            DisplayName = "Alex",
            Skills = new List<Skills>
            {
                new Skills { Name = "Alex", Level = 5 },
                new Skills { Name = "Alex", Level = 4 }
            }
        };

        personRepositoryMock.Setup(repo => repo.GetPersonById(personId))
            .ReturnsAsync(person);

        var result = await personService.GetPersonById(personId) as OkObjectResult;


        Assert.NotNull(result);
        var personResponse = result.Value as PersonResponse;
        Assert.NotNull(personResponse);

    }

    [Fact]
    public async Task UpdatePerson_ReturnsOkObjectResult()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        long personId = 1;
        var personRequest = new PersonRequest
        {
            Name = "Update Vitalik",
            DisplayName = "Updated Vitalik Savinih",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "Updated Читодори", Level = 4 },
                new SkillRequest { Name = "Updated Магическая битва", Level = 3 }
            }
        };

        var updatedPerson = new Person
        {
            Name = personRequest.Name,
            DisplayName = personRequest.DisplayName,
            Skills = personRequest.Skills.Select(s => new Skills { Name = s.Name, Level = s.Level }).ToList()
        };

        personRepositoryMock.Setup(repo => repo.UpdatePerson(personId, personRequest))
            .ReturnsAsync(updatedPerson);


        var result = await personService.UpdatePerson(personId, personRequest) as OkObjectResult;


        Assert.NotNull(result);
        var personResponse = result.Value as PersonResponse;
        Assert.NotNull(personResponse);
        
    }
}
