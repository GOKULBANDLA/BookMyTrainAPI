using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer;

namespace BookMyTrainAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainsController : ControllerBase
    {
        private readonly IBusinessManager _businessManager;
        private readonly ILogger<TrainsController> _logger;
        public TrainsController(IBusinessManager business, ILogger<TrainsController> logger)
        {
            _businessManager = business;
            _logger = logger;
        }
        /// <summary>
        /// Returns the list of stations 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStations")]
        public async Task<IActionResult> FetchStations()
        {
            _logger.LogInformation("Accessed FetchStations Method");
            try
            {
                var listStations = await _businessManager.FetchStations();
                return Ok(listStations);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while accessing stations data from database");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "There is an error while processing your request");
            }

        }
        /// <summary>
        /// REturns trains based on search criteria
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FetchTrains")]
        public async Task<IActionResult> FetchTrains([FromBody] TrainSearch search)
        {
            _logger.LogInformation("Accessed FetchTrains Method");
            try
            {
                var listTrains = await _businessManager.FetchTrains(search);
                return Ok(listTrains);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while accessing stations data from database", ex.Message);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "There is an error while processing your request");
            }
        }
        /// <summary>
        /// Book Train ticket and send an email
        /// </summary>
        /// <param name="bookingDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BookTicket")]
        public async Task<IActionResult> BookTicket([FromBody] TrainBookingDetails bookingDetails)
        {
            _logger.LogInformation("Accessed BookTicket Method");
            try
            {
                bool bookingStatus = await _businessManager.BookTrain(bookingDetails);
                if (bookingStatus)
                {
                    _businessManager.SendEmail(bookingDetails);
                }
                return Ok(bookingStatus);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while accessing stations data from database", ex.Message);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "There is an error while processing your request");
            }
        }


    }

}