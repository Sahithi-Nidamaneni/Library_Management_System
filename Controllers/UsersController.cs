using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Models;
using Library_Management.Repositories;


namespace Library_Management.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly UserRepository _userRepository;

		public UsersController(UserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		// POST: api/users/adduser
		[HttpPost("adduser")]
		public IActionResult AddUser([FromBody] User user)
		{
			if (user == null)
				return BadRequest("Invalid User Data");

			var result = _userRepository.AddUser(user);

			return Ok(result);
		}

		// GET: api/users/getuserbyid/1
		[HttpGet("getuserbyid/{id}")]
		public IActionResult GetUserById(int id)
		{
			var user = _userRepository.GetUserById(id);

			if (user == null)
				return NotFound("User Not Found");

			return Ok(user);
		}

		// GET: api/users/getallusers
		[HttpGet("getallusers")]
		public IActionResult GetAllUsers()
		{
			var users = _userRepository.GetAllUsers();

			return Ok(users);
		}
	}
	}
