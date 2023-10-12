using Hall_Of_Fame.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hall_Of_Fame.Interface
{
    public interface IPersonService
    {
        Task<ActionResult> GetPeople();
        Task<ActionResult> GetPersonById(long id);
        Task<ActionResult> UpdatePerson(long id, PersonRequest personRequest);
        Task<PersonResponse> CreatePerson(PersonRequest personRequest);
        Task<ActionResult> DeletePersonById(long id);
    }
}
