using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostmanAssignment.Entities;
using PostmanAssignment.Services;
using PostmanAssignment.QueryModels;
using PostmanAssignment.Commands;
using PostmanAssignment.Exceptions;

namespace PostmanAssignment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;
        private readonly ILogger<SlotsController> _logger;

        public SlotsController(ISlotService slotService, ILogger<SlotsController> logger)
        {
            _slotService = slotService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Slot slot)
        {
            try
            {
                var newSlot = await _slotService.CreateAsync(slot);
                return CreatedAtRoute("SlotLink", newSlot.Id, newSlot);
            }
            catch (InvalidArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}", Name = "SlotLink")]
        public async Task<ActionResult<Slot>> GetAsync(Guid id)
        {
            try
            {
                var slot = await _slotService.GetAsync(id);
                return Ok(slot);
            }
            catch (InvalidArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("_search")]
        public async Task<ActionResult<IEnumerable<Duration>>> GetAvailableSlotDurationsAsync([FromBody] SlotSearchQuery query)
        {
            try
            {
                return Ok(await _slotService.GetAvailableSlotDurationsAsync(query));
            }
            catch (InvalidArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}