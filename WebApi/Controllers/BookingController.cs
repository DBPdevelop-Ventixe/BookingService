using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BookingController(BookingService bookingService) : ControllerBase
{
    private readonly BookingService _bookingService = bookingService;


    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRegForm booking)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Missing credentials !"});

            var result = await _bookingService.CreateBooking(booking);
            if (!result)
                return BadRequest("Failed to create booking.");

            return Ok("Booking created successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("payments/{bookingId}")]
    public async Task<IActionResult> PayNow(string bookingId)
    {
        try
        {
            if (string.IsNullOrEmpty(bookingId))
                return BadRequest("Booking ID is required.");
            var result = await _bookingService.PayNow(bookingId);
            if (!result)
                return BadRequest("Failed to process payment for the booking.");
            return Ok("Payment processed successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        try
        {
            // Get the userid from bearer token 
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return BadRequest("User ID not found in the token.");

            var bookings = await _bookingService.GetBookings(userId);
            if (bookings == null || bookings.Count == 0)
                return NotFound("No bookings found for this user: ");

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
