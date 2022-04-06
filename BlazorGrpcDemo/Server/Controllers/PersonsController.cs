using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorGrpcDemo.Server.Data;
using BlazorGrpcDemo.Shared;

namespace BlazorGrpcDemo.Server.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PersonsController : Controller
	{
		PersonsManager personsManager;

		public PersonsController(PersonsManager _personsManager)
		{
			personsManager = _personsManager;

		}

		[HttpGet]
		public List<Person> GetAll()
		{
			return personsManager.People;
		}

		[HttpGet("getstream")]
		public async IAsyncEnumerable<Person> GetAllStream()
		{
			await Task.Delay(0);
			var people = personsManager.People;
			foreach (var person in people)
			{
				yield return person;
			}
		}

		[HttpGet("{Id}/getbyid")]
		public Person GetPersonById(int Id)
		{
			return (from x in personsManager.People
						  where x.Id == Id
						  select x).FirstOrDefault();
		}

	}
	
}
