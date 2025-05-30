using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class BookingEntity
{
    [Key]
    public string Id { get; set; } = null!;

    public int TicketsAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TicketPrice { get; set; }

    public string EventId { get; set; } = null!;
    public string EventPackageId { get; set; } = null!;
    public string UserId { get; set; } = null!;


    // Personal Information
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;


    // Contact
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;


    // Address
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Zip { get; set; } = null!;
    public string Country { get; set; } = null!;


    // Payment(Temporary until payment service is created)
    public bool IsPayed { get; set; }
}
