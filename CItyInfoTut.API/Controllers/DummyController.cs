using System;
using CItyInfoTut.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace CItyInfoTut.API.Controllers
{
    public class DummyController: Controller
    {
        private CityInfoContext _ctx;
        public DummyController(CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Route("api/testdatabase")]
        public IActionResult TestDatabase() {
            return Ok();
        }

    }
}
