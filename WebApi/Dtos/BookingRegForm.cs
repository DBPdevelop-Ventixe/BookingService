using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos;

public class BookingRegForm
{
    [Required]
    public int TicketsAmount { get; set; }
    [Required]
    public decimal TicketPrice { get; set; }
    [Required]
    public string EventId { get; set; } = null!;
    [Required]
    public string EventPackageId { get; set; } = null!;
    [Required]
    public string UserId { get; set; } = null!;


    // Personal Information
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;


    // Contact
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Phone { get; set; } = null!;


    // Address
    [Required]
    public string Street { get; set; } = null!;
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string State { get; set; } = null!;
    [Required]
    public string Zip { get; set; } = null!;
    [Required]
    public string Country { get; set; } = null!;
}
