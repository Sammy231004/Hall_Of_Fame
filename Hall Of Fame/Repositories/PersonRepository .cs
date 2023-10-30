using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;
using Hall_Of_Fame.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hall_Of_Fame.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> CreatePerson(Person person)
        {
            await _context.AddAsync(person);
            await _context.SaveChangesAsync();
            return person;

        }

        public async Task DeletePersonById(long id)
        {
            var person = await _context.Persons.SingleOrDefaultAsync(p => p.Id == id);
            _context.Remove(person);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Person>> GetPeople()
        {
            return await _context.Persons.Include(p => p.Skills).ToListAsync();
        }

        public async Task<Person> GetPersonById(long id)
        {
            return await _context.Persons.Include(p => p.Skills).SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person> UpdatePerson(long id, Person person)
        {

            await _context.SaveChangesAsync();
            return person;
        }

    }
}