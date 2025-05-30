using System.ComponentModel.DataAnnotations;

namespace WebApi.Model;

public class BookingModel
{
    public string Id { get; set; } = null!;

    public int TicketsAmount { get; set; }
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


    // Event
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public string EventImage { get; set; } = null!;
    public string EventDate { get; set; } = null!;
    public string EventTime { get; set; } = null!;
    public string EventStreet { get; set; } = null!;
    public string EventCity { get; set; } = null!;
    public string EventZip { get; set; } = null!;
    public string EventState { get; set; } = null!;
    public string EventCountry { get; set; } = null!;

    // Package
    public string PackageName { get; set; } = null!;
    public string[] Benefits { get; set; } = null!;


    // Payment
    public string PaymentStatus { get; set; } = null!;
    public string PaymentTransactionId { get; set; } = null!;
}
