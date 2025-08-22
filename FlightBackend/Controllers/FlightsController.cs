using Microsoft.AspNetCore.Mvc;
using FlightBackend.Services;

namespace FlightBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly LufthansaService _lufthansaService;

        public FlightsController(LufthansaService lufthansaService)
        {
            _lufthansaService = lufthansaService;
        }

        // GET api/flights?from=ZRH&to=FRA&date=2025-08-25
        [HttpGet]
        public async Task<IActionResult> GetFlights(string from, string to, string date)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(date))
                return BadRequest("Parameters 'from', 'to' und 'date' sind noetig.");

            try
            {
                var flightsJson = await _lufthansaService.GetFlightsAsync(from.ToUpperInvariant(), to.ToUpperInvariant(), date);
                // Rohes JSON antworten
                return Content(flightsJson, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error calling Lufthansa API", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/flights/by-number?flight=LH400&date=2025-08-25
        [HttpGet("by-number")]
        public async Task<IActionResult> GetFlightByNumber(string flight, string date)
        {
            if (string.IsNullOrWhiteSpace(flight) || string.IsNullOrWhiteSpace(date))
                return BadRequest("Parameters 'flight' und 'date' sind noetig, z B flight=LH400.");

            try
            {
                var json = await _lufthansaService.GetFlightStatusByNumberAsync(flight, date);
                return Content(json, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error calling Lufthansa API", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
