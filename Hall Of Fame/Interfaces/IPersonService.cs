
using Hall_Of_Fame.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hall_Of_Fame.Interface
{
    public interface IPersonService
    {
        Task<IActionResult> GetPeople();
        Task<IActionResult> GetPersonById(long id);
        Task<IActionResult> UpdatePerson(long id, PersonRequest personRequest);
        Task<PersonResponse> CreatePerson(PersonRequest personRequest);
        Task<IActionResult> DeletePersonById(long id);
    }
}
