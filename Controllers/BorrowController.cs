using Library_Management.Models;
using Library_Management.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BorrowController : ControllerBase
	{
		private readonly BorrowRepository _borrowRepository;
		private readonly UserRepository _userRepository;
		private readonly BookRepository _bookRepository;

		public BorrowController(
			BorrowRepository borrowRepository,
			UserRepository userRepository,
			BookRepository bookRepository)
		{
			_borrowRepository = borrowRepository;
			_userRepository = userRepository;
			_bookRepository = bookRepository;
		}

		// POST: api/borrow/borrowbook
		[HttpPost("borrowbook")]
		public IActionResult BorrowBook([FromBody] BorrowRecord borrowRecord)
		{
			if (borrowRecord == null)
				return BadRequest("Invalid Borrow Data");

			var user = _userRepository.GetUserById(borrowRecord.UserId);

			if (user == null)
				return NotFound("User Not Found");

			var book = _bookRepository.GetBookById(borrowRecord.BookId);

			if (book == null)
				return NotFound("Book Not Found");

			
			var result = _borrowRepository.BorrowBook(borrowRecord);

			return Ok(result);
		}

		[HttpGet("getallborrowrecords")]
		public IActionResult GetAllBorrowRecords()
		{
			var records = _borrowRepository.GetAllBorrowRecords();

			return Ok(records);
		}


		// PUT: api/borrow/returnbook/1
		[HttpPut("returnbook/{borrowRecordId}")]
		public IActionResult ReturnBook(int borrowRecordId)
		{
			var result = _borrowRepository.ReturnBook(borrowRecordId);

			if (result == "Borrow Record Not Found")
				return NotFound(result);

			return Ok(result);
		}

		// GET: api/borrow/getborrowhistorybyuser/1
		[HttpGet("getborrowhistorybyuser/{userId}")]
		public IActionResult GetBorrowHistoryByUser(int userId)
		{
			var records = _borrowRepository.GetBorrowHistoryByUser(userId);

			return Ok(records);
		}
	}
}
