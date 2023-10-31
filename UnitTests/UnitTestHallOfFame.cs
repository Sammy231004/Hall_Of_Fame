using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class PersonServiceTests
{



    [Fact]
    public async Task CreatePerson_ReturnsPersonResponse()
    {

        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        var personRequest = new CreatePersonRequestDto
        
        {
            Name = "Vitalik",
            DisplayName = "Savinih",
            Skills = new List<SkillRequestDto>
            {
                new SkillRequestDto { Name = "Огненный шар", Level = 5 },
                new SkillRequestDto { Name = "Расширение территории", Level = 4 }
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


        var result = await personService.CreatePerson(personRequest) as PersonResponseDto;


        Assert.NotNull(result);
        Assert.Equal(personEntity.Name, result.Name);
        Assert.Equal(personEntity.DisplayName, result.DisplayName);
        Assert.Equal(personEntity.Skills.Count, result.Skills.Count);
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
            Id = 1,
            Name = "vitalik",
            DisplayName = "savimih",
            Skills = new List<Skills>
            {
                new Skills { Name = "fireball", Level = 5 },
            }
        }
            };

        personRepositoryMock.Setup(repo => repo.GetPeople())
            .ReturnsAsync(persons);

        var actionResult = await personService.GetPeople();
        var result = actionResult as OkObjectResult;

        Assert.NotNull(result);

        var personResponses = result.Value as List<PersonResponseDto>;
        Assert.NotNull(personResponses);




    }
    [Fact]
    public async Task DeletePeopleById_ReturnsPersonResponse()
    {
        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);
        long personID = 2;
    }

    [Fact]
    public async Task GetPersonById_ReturnsNotFound_WhenPersonNotFound()
    {
        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);

        var personId = 1;

        personRepositoryMock.Setup(repo => repo.GetPersonById(personId))
            .ReturnsAsync((Person)null);

        var actionResult = await personService.GetPersonById(personId);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsNotFound_WhenPersonNotFound()
    {
        var personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<PersonService>>();
        var personService = new PersonService(personRepositoryMock.Object, loggerMock.Object);

        var personId = 1;
        var updatePersonRequest = new UpdatePersonRequestDto
        {
            Id = personId,
            Name = "alex",
            DisplayName = "petrov"
        };

        personRepositoryMock.Setup(repo => repo.GetPersonById(personId))
            .ReturnsAsync((Person)null);

        var actionResult = await personService.UpdatePerson(personId, updatePersonRequest);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}

