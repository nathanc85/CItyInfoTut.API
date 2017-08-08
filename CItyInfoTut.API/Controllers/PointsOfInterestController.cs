using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CItyInfoTut.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CItyInfoTut.API.Services;

namespace CItyInfoTut.API.Controllers
{
    [Route("/api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
		{
            _logger = logger;
            _mailService = mailService;
		}

        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId) {
            try
            {
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

	            if (city == null) {
	                _logger.LogInformation($"The city with the id of {cityId} was not found!");
	                return NotFound();
	            }

                return Ok(city.PointsOfInterest);
			}
			catch (Exception ex)
			{
                _logger.LogCritical($"A server error took place while trying to get the city with the id of {cityId}.", ex);
                return StatusCode(500, "A server error took place. Please try again later!");
            }
        }

        [HttpGet("{cityId}/pointsOfInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id) {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null) {
                return NotFound("The city was not found!");
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            if (pointOfInterest == null) {
                return NotFound("The point of interest was not found!");
            }

            return Ok(pointOfInterest);
        }


        [HttpPost("{cityId}/pointsofinterest", Name = "GetCity")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
            if (pointOfInterest == null) {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description) {
                ModelState.AddModelError("Description", "The Name and the Description have to be different!");
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null) {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto
            {
                Id = maxPointOfInterestId + 1,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsOfInterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)  {
			if (pointOfInterest == null)
			{
				return BadRequest();
			}

			if (pointOfInterest.Name == pointOfInterest.Description)
			{
				ModelState.AddModelError("Description", "The Name and the Description have to be different!");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			if (city == null)
			{
				return NotFound("City not found!");
			}

            var updatedPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            // Update the POI.
            updatedPointOfInterest.Name = pointOfInterest.Name;
            updatedPointOfInterest.Description = pointOfInterest.Description;

            if (updatedPointOfInterest == null)
			{
				return NotFound("POI not found!");
			}

            return Ok(updatedPointOfInterest);
        }

		[HttpPatch("{cityId}/pointsOfInterest/{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
		{
			if (patchDoc == null)
			{
				return BadRequest();
			}

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			if (city == null)
			{
				return NotFound("City not found!");
			}

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

			if (pointOfInterestFromStore == null)
			{
				return NotFound("POI not found!");
			}

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

			if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
			{
				ModelState.AddModelError("Description", "The Name and the Description have to be different!");
			}

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

			return Ok(pointOfInterestFromStore);
		}

        [HttpDelete("{cityId}/pointOfInterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id) {
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			if (city == null)
			{
				return NotFound("City not found!");
			}

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

			if (pointOfInterestFromStore == null)
			{
				return NotFound("POI not found!");
			}

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send($"A point of interest has been deleted.", $"The point of interest {id}: {pointOfInterestFromStore.Name} has been deleted from the city {cityId}: {city.Name}.");

            return CreatedAtRoute("GetCity", new { cityId = cityId }, city);
        }
    }
}
