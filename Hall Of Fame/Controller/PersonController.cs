using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hall_Of_Fame.ControllerAPI
{
    [Route("api/v1/persons")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PersonResponseDto>))] 
        public async Task<IActionResult> GetPeople() => Ok(await _personService.GetPeople());

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PersonResponseDto))] 
        [ProducesResponseType(404)] 
        public async Task<IActionResult> GetPersonById(long id) => Ok(await _personService.GetPersonById(id));

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PersonResponseDto))] 
        public async Task<IActionResult> Create([FromBody] CreatePersonRequestDto personRequest) => CreatedAtAction("GetPersonById", new { id = (await _personService.CreatePerson(personRequest)).Id }, personRequest);

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(PersonResponseDto))] 
        [ProducesResponseType(404)] 
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePersonRequestDto personRequest)
        {
            var updatedPerson = await _personService.UpdatePerson(id, personRequest);
            if (updatedPerson == null)
            {
                return NotFound();
            }

            return Ok(updatedPerson);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(404)] 
        public async Task<IActionResult> Delete(long id)
        {
            await _personService.DeletePersonById(id);
            return NoContent();
        }
    }
}
