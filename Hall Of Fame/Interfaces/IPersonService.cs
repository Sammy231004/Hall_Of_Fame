using Hall_Of_Fame.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hall_Of_Fame.Interface
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonResponseDto>> GetPeople();
        Task<PersonResponseDto> GetPersonById(long id);
        Task<PersonResponseDto> CreatePerson(CreatePersonRequestDto request);
        Task<PersonResponseDto> UpdatePerson(long id, UpdatePersonRequestDto request);
        Task DeletePersonById(long id);
    }
}
