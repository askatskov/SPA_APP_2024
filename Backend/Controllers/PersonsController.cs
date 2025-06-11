using backend.Model;
using Backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
	private readonly DataContext context;
	public PersonsController(DataContext c)
	{
		context = c;
	}
	[HttpGet]
	public IActionResult GetPersons()
	{
		var persons = context.PersonList!.AsQueryable();
		return Ok(persons);
	}
	[HttpPost]
	public IActionResult Create([FromBody] Person p)
	{
		var dbPerson = context.PersonList?.Find(p.Id);
		if (dbPerson == null)
		{
			context.PersonList?.Add(p);
			context.SaveChanges();
			return CreatedAtAction(nameof(GetPersons), new { p.Id }, p);
		}
		return Conflict();
	}
	[HttpPut("{id}")]
	public IActionResult Update(int? id, [FromBody] Person p)
	{
		var dbPerson = context.PersonList!.AsNoTracking().FirstOrDefault(personInDb => personInDb.Id == p.Id);
		if (id != p.Id || dbPerson == null) return NotFound();
		context.Update(p);
		context.SaveChanges();
		return NoContent();
	}
	[HttpDelete("{id}")]
	public IActionResult Delete(int id)
	{
		var personToDelete = context.PersonList?.Find(id);
		if (personToDelete == null) return NotFound();
		context.PersonList?.Remove(personToDelete);
		context.SaveChanges();
		return NoContent();
	}
}