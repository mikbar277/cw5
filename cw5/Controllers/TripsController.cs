using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using cw5.Context;
using cw5.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace cw5.Controllers;

[Route("api")]
[ApiController]
public class TripsController : ControllerBase
{
    public TripsController()
    {
    }

    [HttpGet("trips")]
    public IActionResult GetTrips()
    {
        var dbContext = new TripContext();

        var result = dbContext.Trips.Select(t => new
        {
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = t.IdCountries.Select(ct => new
            {
                ct.Name
            }),
            Clients = t.ClientTrips.Select(ct => new
            {
                ct.IdClientNavigation.FirstName,
                ct.IdClientNavigation.LastName
            }),
        }).OrderByDescending(t => t.DateFrom);
        // .Join(dbContext.ClientTrips,
        // t => t.IdTrip,
        // ct => ct.IdClient,
        // (t, ct) =>
        // new
        // {
        // Name = t.Name,
        // Description = t.Description,
        // DateFrom = t.DateFrom,
        // DateTo = t.DateTo,
        // MaxPeople = t.MaxPeople,
        // Countries = t.IdCountries,
        // Clients = ct.IdClient
        // });
        return Ok(result);
    }

    [HttpDelete("clients/{IdClient:int}")]
    public IActionResult DeleteClient(int IdClient)
    {
        var dbContext = new TripContext();

        var result = dbContext.Clients.Include(c => c.ClientTrips)
            .FirstOrDefault(c => c.IdClient == IdClient);

        if (result == null)
        {
            return NotFound();
        }

        if (result.ClientTrips.Any())
        {
            return Conflict();
        }

        dbContext.Clients.Remove(result);
        dbContext.SaveChanges();

        return NoContent();
    }

    [HttpPost("trips/{IdTrip:int}/clients")]
    public IActionResult AddClientToTrip(int IdTrip, [FromBody] Client client)
    {
        var dbContext = new TripContext();

        var ExistingTrip = dbContext.Trips.Find(IdTrip);

        if (ExistingTrip == null)
        {
            return NotFound("Trip not found");
        }

        var ExistingClient = dbContext.Clients.FirstOrDefault(c => c.Pesel == client.Pesel);


        if (ExistingClient != null)
        {
            var ExistingClientTrip = dbContext.ClientTrips
                .FirstOrDefault(ct => ct.IdClient == ExistingClient.IdClient && ct.IdTrip == IdTrip);

            if (ExistingClientTrip != null)
            {
                return BadRequest("Client is already assigned to the trip");
            }

            var now = DateTime.Now;
            var ClientTrip = new ClientTrip()
            {
                IdClient = ExistingClient.IdClient,
                IdTrip = IdTrip,
                RegisteredAt = now,
                PaymentDate = now,
                IdTripNavigation = dbContext.Trips.Find(IdTrip)!,
                IdClientNavigation = ExistingClient
            };
            dbContext.ClientTrips.Add(ClientTrip);
            dbContext.SaveChanges();
            
            var ExistClient = new Client
            {
                IdClient = ExistingClient.IdClient,
                FirstName = ExistingClient.FirstName,
                LastName = ExistingClient.LastName,
                Email = ExistingClient.Email,
                Telephone = ExistingClient.Telephone,
                Pesel = ExistingClient.Pesel
            };

            return Ok(ExistClient);
        }
        else
        {
            dbContext.Clients.Add(client);
            dbContext.SaveChanges();

            var aClient = dbContext.Clients.FirstOrDefault(c => c.Pesel == client.Pesel);

            if (aClient == null)
            {
                return StatusCode(500, "Client PESEL is null");
            }

            var now = DateTime.Now;
            var NewClientTrip = new ClientTrip()
            {
                IdClient = aClient.IdClient,
                IdTrip = IdTrip,
                RegisteredAt = now,
                PaymentDate = now,
                IdTripNavigation = dbContext.Trips.Find(IdTrip)!,
                IdClientNavigation = aClient
            };

            dbContext.ClientTrips.Add(NewClientTrip);
            dbContext.SaveChanges();

            client = dbContext.Clients.OrderBy(c => c.IdClient).LastOrDefault();

            var addedClient = new Client
            {
                IdClient = client.IdClient,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Telephone = client.Telephone,
                Pesel = client.Pesel
            };

            return Ok(addedClient);
        }
    }
}