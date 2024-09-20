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

        public RevenueReportController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }
   /*     [HttpGet("GetRevenueReportByDate")]
        public async Task<IActionResult> GetRevenueReportByDate([FromQuery] Guid garageId, [FromQuery] DateTime date)
        {
            try
            {
                var revenueReport = await _revenueService.GetRevenueReportByDateAsync(garageId, date);
                return Ok(revenueReport);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }*/

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
                // Log the exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    /*    [HttpGet("GetAllRevenueReports")]
        public async Task<IActionResult> GetAllRevenueReports([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var revenueReports = await _revenueService.GetAllRevenueReportsAsync(startDate, endDate);
                return Ok(revenueReports);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
*/
    }
}