using Grpc.Core;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Model;

namespace WebApi.Services;

public class BookingService(DataContext context, EventProto.EventProtoClient eventProto) 
{
    private readonly DataContext _context = context;
    private readonly EventProto.EventProtoClient _eventProto = eventProto;


    // Create
    public async Task<bool> CreateBooking(BookingRegForm booking)
    {
        try
        {
            var entity = new BookingEntity
            {
                TicketsAmount = booking.TicketsAmount,
                TicketPrice = booking.TicketPrice,
                EventId = booking.EventId,
                EventPackageId = booking.EventPackageId,
                UserId = booking.UserId,
                FirstName = booking.FirstName,
                LastName = booking.LastName,
                Email = booking.Email,
                Phone = booking.Phone,
                Street = booking.Street,
                City = booking.City,
                State = booking.State,
                Zip = booking.Zip,
                Country = booking.Country,
                Id = Guid.NewGuid().ToString()
            };

            _context.Bookings.Add(entity);
            

            // Set sold tickts for this event package
            var eventPackage = await _eventProto.SetTicketsSoldAsync(new SetTicketsSoldRequest
            {
                PackageId = booking.EventPackageId,
                TicketsSold = booking.TicketsAmount
            });
            if (eventPackage == null || !eventPackage.Success)
                throw new RpcException(new Status(StatusCode.Internal, "Failed to update tickets sold for the event package."));

            // Save the booking to the database
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }


    // Read
    public async Task<List<BookingModel>> GetBookings(string userId)
    {
        try
        {
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .ToList();

            if (bookings == null || bookings.Count == 0)
                throw new RpcException(new Status(StatusCode.NotFound, "No bookings found for this user."));

            var bookingModels = bookings.Select(b => new BookingModel
            {
                Id = b.Id,
                TicketsAmount = b.TicketsAmount,
                TicketPrice = b.TicketPrice,
                EventId = b.EventId,
                EventPackageId = b.EventPackageId,
                UserId = b.UserId,
                FirstName = b.FirstName,
                LastName = b.LastName,
                Email = b.Email,
                Phone = b.Phone,
                Street = b.Street,
                City = b.City,
                State = b.State ?? "",
                Zip = b.Zip,
                Country = b.Country,
                PaymentStatus = b.IsPayed ? "Paid" : "Not paid",
                PaymentTransactionId = ""
            }).ToList();

            foreach (var booking in bookingModels)
            {
                var eventData = await _eventProto.GetEventByIdAsync(new GetEventRequest
                {
                    EventId = booking.EventId
                });
                if (eventData != null)
                {
                    booking.EventName = eventData.EventName;
                    booking.EventImage = eventData.EventImage;
                    booking.EventDate = eventData.EventDate;
                    booking.EventTime = eventData.EventTime;
                    booking.EventDescription = eventData.EventDescription;
                    booking.EventStreet = eventData.EventAddress.Street;
                    booking.EventCity = eventData.EventAddress.City;
                    booking.EventZip = eventData.EventAddress.ZipCode;
                    booking.EventState = eventData.EventAddress.State;
                    booking.EventCountry = eventData.EventAddress.Country;
                }

                var packageData = eventData.EventPackages
                    .FirstOrDefault(p => p.Id == booking.EventPackageId);

                if (packageData != null)
                {
                    booking.PackageName = packageData.Title;
                    booking.Benefits = packageData.Benefits.ToArray();
                }
                else
                {
                    booking.PackageName = "Unknown Package";
                }
            }

            return bookingModels ?? [];
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    // Update
    public async Task<bool> PayNow(string bookingId)
    {
        try
        {
            if (string.IsNullOrEmpty(bookingId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Booking ID is required."));
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Booking not found."));
            booking.IsPayed = true;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
