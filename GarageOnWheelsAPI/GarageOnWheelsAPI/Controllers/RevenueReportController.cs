using GarageOnWheelsAPI.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GarageOnWheelsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueReportController : ControllerBase
    {

        private readonly IRevenueService _revenueService;
        private readonly ILogger<RevenueReportController> _logger;

        public RevenueReportController(IRevenueService revenueService , ILogger<RevenueReportController> logger)
        {
            _revenueService = revenueService;
            _logger = logger;
        }

        [HttpGet("GetRevenueReportByDateRange")]   
        public async Task<IActionResult> GetRevenueReportByDateRange([FromQuery] Guid garageId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
            {
            try
            {
                var revenueReport = await _revenueService.GetRevenueReportByDateRangeAsync(garageId, startDate, endDate);
                return Ok(revenueReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to retrieve all users.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }  
    }
}