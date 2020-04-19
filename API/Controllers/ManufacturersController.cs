using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain.Core;
using Domain.Entities;
using Domain.Services;
using System.Linq.Expressions;
using Dtos;
using API.Mappers;

namespace API.Controllers
{
    [Authorize]
    public class ManufacturersController : ApiController
    {
        private readonly IManufacturerService _manufacturerService;

        private static readonly Expression<Func<Manufacturer, ManufacturerDto>> AsManufacturerDto =
            x => new ManufacturerDto
            {
                ManufacturerId = x.ManufacturerId,
                ManufacturerName = x.ManufacturerName
            };
        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        // GET: api/Manufacturers
        public IQueryable<ManufacturerDto> GetManufacturers()
        {
            return _manufacturerService.GetAll().Select(AsManufacturerDto);
        }

        // GET: api/Manufacturers/5
        [ResponseType(typeof(ManufacturerDto))]
        public async Task<IHttpActionResult> GetManufacturer(int id)
        {
            Manufacturer manufacturer = await _manufacturerService.GetSingle(id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToManufacturerDto(manufacturer));
        }

        // PUT: api/Manufacturers/5
        [ResponseType(typeof(ManufacturerDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutManufacturer(int id, ManufacturerDto manufacturerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != manufacturerDto.ManufacturerId)
            {
                return BadRequest("Mismatch");
            }

            var updatedManufacturer = await _manufacturerService.Edit(Mapping.ToManufacturer(manufacturerDto));

            return Ok(Mapping.ToManufacturerDto(updatedManufacturer));            
        }

        // POST: api/Manufacturers
        [ResponseType(typeof(ManufacturerDto))]
        public async Task<IHttpActionResult> PostManufacturer(ManufacturerDto manufacturerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var manufacturer = await _manufacturerService.Add(Mapping.ToManufacturer(manufacturerDto));

            return CreatedAtRoute("DefaultApi", new { id = manufacturer.ManufacturerId }, Mapping.ToManufacturerDto(manufacturer));            
        }

        // DELETE: api/Manufacturers/5
        [ResponseType(typeof(ManufacturerDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteManufacturer(int id)
        {
            Manufacturer manufacturer = await _manufacturerService.GetSingle(id);
            Manufacturer deletedManufacturer = await _manufacturerService.Delete(manufacturer);

            return Ok(Mapping.ToManufacturerDto(deletedManufacturer));            
        }
        
    }
}