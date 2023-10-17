using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hall_Of_Fame.DTO;
using Hall_Of_Fame.ControllerAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class PersonsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Hall_Of_Fame.Startup>>
{
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTests(WebApplicationFactory<Hall_Of_Fame.Startup> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPersons_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/persons");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreatePerson_ReturnsSuccessStatusCode()
    {
        var personRequest = new PersonRequest
        {
            Name = "John Doe",
            DisplayName = "John",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "C#", Level = 5 },
                new SkillRequest { Name = "ASP.NET Core", Level = 4 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/persons", personRequest);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdatePerson_ReturnsSuccessStatusCode()
    {
        // Создайте персону и получите ее идентификатор

        var personRequest = new PersonRequest
        {
            Name = "Alice Smith",
            DisplayName = "Alice",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "Java", Level = 4 },
                new SkillRequest { Name = "Spring Boot", Level = 3 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/persons", personRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdPerson = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        // Теперь обновите персону

        personRequest.Name = "Updated Name";
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/persons/{createdPerson.Id}", personRequest);
        updateResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetPersonById_ReturnsSuccessStatusCode()
    {
        // Создайте персону и получите ее идентификатор

        var personRequest = new PersonRequest
        {
            Name = "Jane Johnson",
            DisplayName = "Jane",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "Python", Level = 5 },
                new SkillRequest { Name = "Django", Level = 4 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/persons", personRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdPerson = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        // Теперь получите персону по идентификатору

        var getResponse = await _client.GetAsync($"/api/v1/persons/{createdPerson.Id}");
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeletePerson_ReturnsSuccessStatusCode()
    {
        // Создайте персону и получите ее идентификатор

        var personRequest = new PersonRequest
        {
            Name = "Bob Brown",
            DisplayName = "Bob",
            Skills = new List<SkillRequest>
            {
                new SkillRequest { Name = "Ruby", Level = 3 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/persons", personRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdPerson = await createResponse.Content.ReadFromJsonAsync<PersonResponse>();

        // Теперь удалите персону

        var deleteResponse = await _client.DeleteAsync($"/api/v1/persons/{createdPerson.Id}");
        deleteResponse.EnsureSuccessStatusCode();
    }
}
