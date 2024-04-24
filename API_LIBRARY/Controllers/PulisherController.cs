using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.Models;
using API_LIBRARY.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API_LIBRARY.Data;
using API_LIBRARY.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API_LIBRARY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublishersController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly LibaryDbContext libraryDbContext;

        public PublishersController(IBookRepository bookRepository,LibaryDbContext libaryDbContext)
        {
            _bookRepository = bookRepository;
            libraryDbContext = libaryDbContext;
        }

        [HttpGet("get-all-publisher")]
        public async Task<ActionResult<List<Publisher>>> GetPublishers()
        {
            try
            {
                var publishers = await _bookRepository.GetPublishersAsync();
                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id, [FromQuery] bool includeBooks = false)
        {
            try
            {
                var publisher = await _bookRepository.GetPublisherAsync(id, includeBooks);
                if (publisher == null)
                {
                    return NotFound($"Publisher with ID {id} not found.");
                }
                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Publisher>> AddPublisher(PulisherDTO publisherDTO)
        {
            try
            {
                var publisher = new Publisher
                {
                    Name = publisherDTO.Name
                };

                libraryDbContext.Publisher.Add(publisher);
                await libraryDbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, "Publisher added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Publisher>> UpdatePublisher(int id, [FromBody] PulisherDTO pulisherDTO)
        {
            var publisher = await libraryDbContext.Publisher.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            publisher.Name= pulisherDTO.Name;

            libraryDbContext.Entry(publisher).State = EntityState.Modified;

            try
            {
                await libraryDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        private bool PublisherExists(int id)
        {
            return libraryDbContext.Authors.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePublisher(int id)
        {
            try
            {
                var publisher = await libraryDbContext.Publisher.FindAsync(id);
                if (publisher == null)
                {
                    return NotFound();
                }

                libraryDbContext.Remove(publisher);
                await libraryDbContext.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
