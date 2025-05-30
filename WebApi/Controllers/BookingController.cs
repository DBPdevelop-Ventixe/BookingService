using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Controllers;
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

    [HttpGet]
    [Route("{userId}")]
    public async Task<IActionResult> GetBookings(string userId)
    {
        try
        {
            var bookings = await _bookingService.GetBookings(userId);
            if (bookings == null || bookings.Count == 0)
                return NotFound("No bookings found for this user.");

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
