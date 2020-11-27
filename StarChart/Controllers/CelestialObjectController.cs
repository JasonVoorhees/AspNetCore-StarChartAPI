using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if(celestialObject == null)
                return NotFound();
            SetSatellites(celestialObject);
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);
            if ((celestialObjects == null) || (celestialObjects.Count() == 0))
                return NotFound();
            foreach (var singleObject in celestialObjects)
                SetSatellites(singleObject);
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObjects = _context.CelestialObjects;
            foreach (var theObject in allObjects)
                SetSatellites(theObject);
            return Ok(allObjects);
        }

        private void SetSatellites(CelestialObject celestialObject)
        {
            if (celestialObject == null)
                return;
            celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
        }
    }
}
