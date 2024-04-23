using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.Models;
using API_LIBRARY.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_LIBRARY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public PublishersController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
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
        public async Task<ActionResult<Publisher>> AddPublisher(Publisher publisher)
        {
            try
            {
                var addedPublisher = await _bookRepository.AddPublisherAsync(publisher);
                return CreatedAtAction(nameof(GetPublisher), new { id = addedPublisher.Id }, addedPublisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Publisher>> UpdatePublisher(int id, Publisher publisher)
        {
            try
            {
                if (id != publisher.Id)
                {
                    return BadRequest("Publisher ID mismatch.");
                }

                var updatedPublisher = await _bookRepository.UpdatePublisherAsync(publisher);
                if (updatedPublisher == null)
                {
                    return NotFound($"Publisher with ID {id} not found.");
                }

                return Ok(updatedPublisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePublisher(int id)
        {
            try
            {
                var publisherToDelete = await _bookRepository.GetPublisherAsync(id);
                if (publisherToDelete == null)
                {
                    return NotFound($"Publisher with ID {id} not found.");
                }

                var result = await _bookRepository.DeletePublisherAsync(publisherToDelete);
                if (!result.Item1)
                {
                    return StatusCode(500, $"Error deleting publisher: {result.Item2}");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
