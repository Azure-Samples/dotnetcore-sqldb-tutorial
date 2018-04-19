using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.DataModels;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MediaTypesController : Controller
    {
        private readonly MyDatabaseContext _context;

        public MediaTypesController(MyDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/MediaTypes
        [HttpGet]
        public IEnumerable<MediaType> GetMediaType()
        {
            return _context.MediaType;
        }

        // GET: api/MediaTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMediaType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mediaType = await _context.MediaType.SingleOrDefaultAsync(m => m.MediaTypeId == id);

            if (mediaType == null)
            {
                return NotFound();
            }

            return Ok(mediaType);
        }

        // PUT: api/MediaTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMediaType([FromRoute] int id, [FromBody] MediaType mediaType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mediaType.MediaTypeId)
            {
                return BadRequest();
            }

            _context.Entry(mediaType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MediaTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MediaTypes
        [HttpPost]
        public async Task<IActionResult> PostMediaType([FromBody] MediaType mediaType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MediaType.Add(mediaType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMediaType", new { id = mediaType.MediaTypeId }, mediaType);
        }

        // DELETE: api/MediaTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mediaType = await _context.MediaType.SingleOrDefaultAsync(m => m.MediaTypeId == id);
            if (mediaType == null)
            {
                return NotFound();
            }

            _context.MediaType.Remove(mediaType);
            await _context.SaveChangesAsync();

            return Ok(mediaType);
        }

        private bool MediaTypeExists(int id)
        {
            return _context.MediaType.Any(e => e.MediaTypeId == id);
        }
    }
}