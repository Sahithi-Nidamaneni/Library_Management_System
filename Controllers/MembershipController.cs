using Library_Management.Models;

using Library_Management.Repositories;

using Microsoft.AspNetCore.Mvc;

using Azure.Messaging.ServiceBus;

using System.Text.Json;

namespace Library_Management.Controllers

{

	[Route("api/[controller]")]

	[ApiController]

	public class MembershipController : ControllerBase

	{

		private readonly MembershipRepository _membershipRepository;

		private readonly UserRepository _userRepository;

		private readonly ServiceBusClient _serviceBusClient;

		private readonly string _queueName;

		public MembershipController(

			MembershipRepository membershipRepository,

			UserRepository userRepository,

			ServiceBusClient serviceBusClient,

			IConfiguration config)

		{

			_membershipRepository = membershipRepository;

			_userRepository = userRepository;

			_serviceBusClient = serviceBusClient;

			_queueName = config["ServiceBus:QueueName"]

				?? throw new InvalidOperationException("ServiceBus:QueueName not configured");

		}

		// POST: api/membership/addmembership

		[HttpPost("addmembership")]

		public async Task<IActionResult> AddMembership([FromBody] Membership membership)

		{

			if (membership == null)

				return BadRequest("Invalid Membership Data");

			// Check if user exists

			var existingUser = _userRepository.GetUserById(membership.UserId);

			if (existingUser == null)

				return NotFound("User does not exist. Please create user first.");

			// Date validation

			if (membership.EndDate <= membership.StartDate)

				return BadRequest("EndDate must be greater than StartDate.");

			// Save to DB

			var result = _membershipRepository.AddMembership(membership);

			// Send message to Service Bus

			try

			{

				string jsonMessage = JsonSerializer.Serialize(membership);

				ServiceBusSender sender = _serviceBusClient.CreateSender(_queueName);

				await sender.SendMessageAsync(new ServiceBusMessage(jsonMessage));

				await sender.DisposeAsync();

			}

			catch (Exception ex)

			{

				// Log karo, par DB save ho chuka hai isliye 200 hi return kar

				Console.WriteLine($"Service Bus send failed: {ex.Message}");

				return Ok(new { result, warning = "Saved but message not sent to queue" });

			}

			return Ok(new { result, message = "Membership Added + Queue Message Sent" });

		}

		// GET: api/membership/getmembershipbyuserid/1

		[HttpGet("getmembershipbyuserid/{userId}")]

		public IActionResult GetMembershipByUserId(int userId)

		{

			var membership = _membershipRepository.GetMembershipByUserId(userId);

			if (membership == null)

				return NotFound("Membership Not Found");

			return Ok(membership);

		}

	}

}
