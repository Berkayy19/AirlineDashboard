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

        [HttpGet]
        public async Task<IActionResult> GetFlights(string from, string to, string date)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(date))
                return BadRequest("Parameters 'from', 'to', and 'date' are required.");

            try
            {
                var flightsJson = await _lufthansaService.GetFlightsAsync(from, to, date);
                return Ok(flightsJson);
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
