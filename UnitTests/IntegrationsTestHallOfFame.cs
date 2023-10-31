using Hall_Of_Fame.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using Xunit;

namespace IntegrationsTestHallOfFame
{

    public class PersonControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
 
	{
		private readonly WebApplicationFactory<Program> _factory;           
		private readonly HttpClient _client;

        // сохраняет в бд     
        /*public PersonControllerIntegrationTests(WebApplicationFactory<Program>factory)
            {
                _factory = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("DB");
                        });
                    });
                });

                _client = _factory.CreateClient();

            }*/
        public PersonControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddEntityFrameworkInMemoryDatabase();
                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("Identity");
                        options.UseInternalServiceProvider(provider);
                    });
                });
            });

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
				Name = "2",
				DisplayName = "2",
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
				Id = 12,
				Name = "1",
				DisplayName = "21",
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
