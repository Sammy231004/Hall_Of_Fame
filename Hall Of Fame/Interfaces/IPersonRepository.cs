﻿using Hall_Of_Fame.DTO;
using Hall_Of_Fame.Entities;

public interface IPersonRepository
{
    Task<List<Person>> GetPeople();
    Task<Person> GetPersonById(long id);
    Task<Person> UpdatePerson(long id, PersonRequest personRequest);
    Task<Person> CreatePerson(Person person);
    Task DeletePersonById(long id);
}