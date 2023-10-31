using Hall_Of_Fame.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Text;
using Xunit;


namespace IntegrationsTestHallOfFame
{
    public class PersonControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
 
    {

        private readonly WebApplicationFactory<Program> _factory;           
        private readonly HttpClient _client;
        public readonly TestServer _server;



        public PersonControllerIntegrationTests(WebApplicationFactory<Program>factory)
        {

            _factory = factory;
            _client = _factory.CreateClient();

        }

        [Fact]
        public async Task GetPeople_ReturnsOkObjectResult()
        {

            var response = await _client.GetAsync("/api/v1/persons");


            response.EnsureSuccessStatusCode();


            var responseString = await response.Content.ReadAsStringAsync();


            var personResponses = JsonConvert.DeserializeObject<PersonResponseDto[]>(responseString);


            Assert.NotEmpty(personResponses);
        }

        [Fact]
        public async Task CreatePerson_ReturnsCreatedAtAction()
        {

            var personRequest = new CreatePersonRequestDto
            {
                Name = "killaaaaaaa",
                DisplayName = "Killuabbbbbbbbbb",
                Skills = new[]
                {
                new SkillRequestDto { Name = "FireBall", Level = 5 },
                new SkillRequestDto { Name = "Blast", Level = 4 }
            }
            };


            var jsonRequest = JsonConvert.SerializeObject(personRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v1/persons", content);


            response.EnsureSuccessStatusCode();


            var location = response.Headers.Location;
            Assert.NotNull(location);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsOkObjectResult()
        {

            var updatePersonRequest = new UpdatePersonRequestDto
            {
                Id = 24,
                Name = "йцукйцук",
                DisplayName = "йцук",
                Skills = new[]
                {
                new SkillResponseDto { Name = "цукйу", Level = 3 },
                new SkillResponseDto { Name = "йцук", Level = 4 }
            }
            };


            var jsonRequest = JsonConvert.SerializeObject(updatePersonRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");


            var response = await _client.PutAsync($"/api/v1/persons/{updatePersonRequest.Id}", content);


            response.EnsureSuccessStatusCode();


            var responseString = await response.Content.ReadAsStringAsync();

            var updatedPerson = JsonConvert.DeserializeObject<PersonResponseDto>(responseString);


            Assert.Equal(updatePersonRequest.Id, updatedPerson.Id);
            Assert.Equal(updatePersonRequest.Name, updatedPerson.Name);
            Assert.Equal(updatePersonRequest.DisplayName, updatedPerson.DisplayName);
        }
    }

}
