using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CItyInfoTut.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CItyInfoTut.API.Services;
using AutoMapper;

namespace CItyInfoTut.API.Controllers
{
    [Route("/api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId) {
            try
            {
                // This is the Store data approach.
                // var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"The city with the id of {cityId} was not found!");
                //    return NotFound();
                //}

                //return Ok(city.PointsOfInterest);

                if (!_cityInfoRepository.CityExists(cityId)) {
                    _logger.LogInformation($"The city with the id of {cityId} was not found!");
                    return NotFound($"The city with the id of {cityId} was not found!");
                }

                var pointsOfInterest = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                if (pointsOfInterest == null) {
                    _logger.LogInformation($"No points of interest were found for {cityId}.");
                    return NotFound(pointsOfInterest);
                }

                var poiResults = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
                // Old way of mapping.
                //var poiResults = new List<PointOfInterestDto>();
                //foreach (var poi in pointsOfInterest)
                //{
                //    poiResults.Add(new PointOfInterestDto {
                //        Id = poi.Id,
                //        Name = poi.Name,
                //        Description = poi.Description
                //    });
                //}

                return Ok(poiResults);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"A server error took place while trying to get the city with the id of {cityId}.", ex);
                return StatusCode(500, "A server error took place. Please try again later!");
            }
        }

        [HttpGet("{cityId}/pointsOfInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id) {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null) {
            //    return NotFound("The city was not found!");
            //}

            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterest == null) {
            //    return NotFound("The point of interest was not found!");
            //}

            //return Ok(pointOfInterest);
            if (!_cityInfoRepository.CityExists(cityId)) {
                string message = $"The city with the id of {cityId} was not found!";
                _logger.LogInformation(message);
                return NotFound(message);
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (poiEntity == null)
            {
                string message = $"No point of interest was found with the id of {id}.";
                _logger.LogInformation(message);
                return NotFound(message);
            }

            var poiResult = Mapper.Map<PointOfInterestDto>(poiEntity);
            // Old mapping.
            //var poiResult = new PointOfInterestDto() {
            //    Id = poiEntity.Id,
            //    Name = poiEntity.Name,
            //    Description = poiEntity.Description
            //};

            return Ok(poiResult);
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

            if (!_cityInfoRepository.CityExists(cityId)){
                return NotFound();
            }
			//var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			//if (city == null) {
			//    return NotFound();
			//}

			//var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

			//var finalPointOfInterest = new PointOfInterestDto
			//{
			//    Id = maxPointOfInterestId + 1,
			//    Name = pointOfInterest.Name,
			//    Description = pointOfInterest.Description
			//};

			// city.PointsOfInterest.Add(finalPointOfInterest);

			var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if(!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem occured while handling the request!");
            }

            var createdPointOfInterest = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = createdPointOfInterest.Id }, createdPointOfInterest);
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

            if(!_cityInfoRepository.CityExists(cityId)) {
                return NotFound("City not found!");
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
			if (poiEntity == null)
			{
				return NotFound("POI not found!");
			}

            Mapper.Map(pointOfInterest, poiEntity);

			if (!_cityInfoRepository.Save())
			{
				return StatusCode(500, "A problem occured while handling the request!");
			}

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound("City not found!");
            //}

            //var updatedPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            // Update the POI.
            //updatedPointOfInterest.Name = pointOfInterest.Name;
            //updatedPointOfInterest.Description = pointOfInterest.Description;

            //if (updatedPointOfInterest == null)
            //{
            //    return NotFound("POI not found!");
            //}

            var updatedPointOfInterest = Mapper.Map<PointOfInterestDto>(poiEntity);
            return Ok(updatedPointOfInterest);
        }

        [HttpPatch("{cityId}/pointsOfInterest/{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound("City not found!");
            //}

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound("POI not found!");
            //}

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (poiEntity == null) {
                return NotFound("POI not found!");
            }

            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = pointOfInterestFromStore.Name,
            //    Description = pointOfInterestFromStore.Description
            //};

            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(poiEntity); 
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The Name and the Description have to be different!");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            //return Ok(pointOfInterestFromStore);

            Mapper.Map(pointOfInterestToPatch, poiEntity);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem occured while handling the request!");
            }

            return Ok(pointOfInterestToPatch);
        }

        [HttpDelete("{cityId}/pointOfInterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id) {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound("City not found!");
            //}

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);

            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound("POI not found!");
            //}

            //city.PointsOfInterest.Remove(pointOfInterestFromStore);

            //_mailService.Send($"A point of interest has been deleted.", $"The point of interest {id}: {pointOfInterestFromStore.Name} has been deleted from the city {cityId}: {city.Name}.");

            //return CreatedAtRoute("GetCity", new { cityId = cityId }, city);

            if (!_cityInfoRepository.CityExists(cityId)) {
                return NotFound("City not found!");
            }
            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (poiEntity == null) {
                return NotFound("POI not found!");
            }

            _cityInfoRepository.DeletePointOfInterest(poiEntity);
            if (!_cityInfoRepository.Save()){
                return StatusCode(500, "A problem occured while handling the request!");
            }

            var cityEntity = _cityInfoRepository.GetCity(cityId, true);
            var cityDto = Mapper.Map<Models.CityDto>(cityEntity);
            return Ok(cityDto);
        }
    }
}
