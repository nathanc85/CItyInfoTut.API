using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CItyInfoTut.API.Controllers
{
    [Route("/api/cities")]
    public class CitiesController : Controller
    {
        [HttpGet()]
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
		{
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (city == null) {
                return NotFound();
            }

            return Ok(city);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
