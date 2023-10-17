using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Hall_Of_Fame.ControllerAPI
{
    [Route("api/v1/persons")]
    [ApiController]
    public class ControllerPerson : Controller
    {
        private readonly IPersonService _personService;

        public ControllerPerson(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        // изменено с ActionResult в IActionResult
        public async Task<IActionResult> GetPeople() => await _personService.GetPeople();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(long id) => await _personService.GetPersonById(id);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonRequest personRequest) => (IActionResult)await _personService.CreatePerson(personRequest);

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] PersonRequest personRequest) => await _personService.UpdatePerson(id, personRequest);

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id) => await _personService.DeletePersonById(id);
    }
}
