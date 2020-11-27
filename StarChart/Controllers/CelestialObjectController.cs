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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var objectToUpdate = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (objectToUpdate == null)
                return NotFound();
            objectToUpdate.Name = celestialObject.Name;
            objectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            objectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(objectToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var objectToUpdate = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (objectToUpdate == null)
                return NotFound();
            objectToUpdate.Name = name;
            _context.Update(objectToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectToDelete = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (objectToDelete == null)
                return NotFound();
            SetSatellites(objectToDelete);
            var allObjectsToRemove = objectToDelete.Satellites.ToList();
            allObjectsToRemove.Add(objectToDelete);
            _context.RemoveRange(allObjectsToRemove);
            _context.SaveChanges();
            return NoContent();
        }

        private void SetSatellites(CelestialObject celestialObject)
        {
            if (celestialObject == null)
                return;
            celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
        }
    }
}
