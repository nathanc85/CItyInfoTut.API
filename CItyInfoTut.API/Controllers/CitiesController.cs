using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CItyInfoTut.API.Services;
using CItyInfoTut.API.Models;
using AutoMapper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CItyInfoTut.API.Controllers
{
    [Route("/api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository) {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            // The Data Store version.
            //return Ok(CitiesDataStore.Current.Cities);

            // The Entities/Repository Pattern version.
            var cityEntities = _cityInfoRepository.GetCities();

            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities);

            //var results = new List<CityWithoutPointsOfInterestDTO>();

            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDTO {
            //        Id = cityEntity.Id,
            //        Name = cityEntity.Name,
            //        Description = cityEntity.Description
            //    });
            //}

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
		{
            // The Data Store version.
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            //if (city == null) {
            //    return NotFound();
            //}

            //return Ok(city);

            // The Entities/Repository Pattern version.
            var cityEntity = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (cityEntity == null) {
                return NotFound();
            }
            if (includePointsOfInterest) {
                var cityResult = Mapper.Map<CityDto>(cityEntity);
                // Without AutoMapper.
                //var cityResult = new CityDto
                //{
                //    Id = cityEntity.Id,
                //    Name = cityEntity.Name,
                //    Description = cityEntity.Description
                //};
                //foreach (var poi in cityEntity.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(new PointOfInterestDto
                //    {
                //        Id = poi.Id,
                //        Name = poi.Name,
                //        Description = poi.Description
                //    });
                //}

                return Ok(cityResult);
            }

            var cityWithoutPOIResult = Mapper.Map<CityWithoutPointsOfInterestDTO>(cityEntity);
			//var cityWithoutPOIResult = new CityWithoutPointsOfInterestDTO
            //{
            //    Id = cityEntity.Id,
            //    Name = cityEntity.Name,
            //    Description = cityEntity.Description
            //};

            return Ok(cityWithoutPOIResult);
		}


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
