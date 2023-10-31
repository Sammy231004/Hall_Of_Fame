using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
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

    public class PersonControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
 
	{
		private readonly CustomWebApplicationFactory<Program> _factory;           
		private readonly HttpClient _client;
        public PersonControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
	        _factory = factory;
	        _client = _factory.CreateClient();
        }

        private async Task SeedDataAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

               
                var person = new Person
                {
					Id=2,
                    Name = "123 ",
                    DisplayName = "123",
                    Skills = new List<Skills>
                    {
                        new Skills { Name = "1", Level = 1 },
                        new Skills { Name = "14", Level = 2 }
                    }
                };

                context.Persons.Add(person);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
		public async Task GetPeople_ReturnsOkObjectResult()
		{
            await SeedDataAsync();

            var response = await _client.GetAsync("/api/v1/persons");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			Assert.NotEmpty(responseString);
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
				Id = 2,
				Name = "2",
				DisplayName = "2",
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
