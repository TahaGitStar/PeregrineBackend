using Microsoft.AspNetCore.Mvc;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeregrineBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccidentsController : ControllerBase
    {
        private readonly IAccidentService _accidentService;

        public AccidentsController(IAccidentService accidentService)
        {
            _accidentService = accidentService;
        }

        // GET: api/accidents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Accident>>> GetAccidents(
            [FromQuery] string type = null,
            [FromQuery] string status = null)
        {
            var accidents = await _accidentService.GetAllAccidentsAsync(type, status);
            return Ok(new { reports = accidents, total = accidents.Count, success = true });
        }

        // GET: api/accidents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Accident>> GetAccident(Guid id)
        {
            var accident = await _accidentService.GetAccidentByIdAsync(id);

            if (accident == null)
            {
                return NotFound(new { success = false, message = "الحادث غير موجود" });
            }

            return Ok(new { report = accident, success = true });
        }

        // GET: api/accidents/guard/{guardId}
        [HttpGet("guard/{guardId}")]
        public async Task<ActionResult<IEnumerable<Accident>>> GetAccidentsByGuard(Guid guardId)
        {
            var accidents = await _accidentService.GetAccidentsByGuardAsync(guardId);
            return Ok(new { reports = accidents, total = accidents.Count, success = true });
        }

        // GET: api/accidents/contract/{contractId}
        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<Accident>>> GetAccidentsByContract(Guid contractId)
        {
            var accidents = await _accidentService.GetAccidentsByContractAsync(contractId);
            return Ok(new { reports = accidents, total = accidents.Count, success = true });
        }

        // POST: api/accidents
        [HttpPost]
        public async Task<ActionResult<Accident>> CreateAccident([FromBody] AccidentCreateDto accidentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var accident = new Accident
            {
                Title = accidentDto.Title,
                Description = accidentDto.Description,
                Type = accidentDto.Type,
                Status = accidentDto.Status,
                Location = accidentDto.Location,
                MediaUrls = string.Join(",", accidentDto.MediaUrls ?? new List<string>()),
                GuardId = accidentDto.GuardId,
                ContractId = accidentDto.ContractId,
                Comments = new List<Comment>()
            };

            var createdAccident = await _accidentService.CreateAccidentAsync(accident);

            return CreatedAtAction(nameof(GetAccident), new { id = createdAccident.Id }, new { report = createdAccident, success = true });
        }

        // PUT: api/accidents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccident(Guid id, [FromBody] AccidentUpdateDto accidentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var accident = await _accidentService.GetAccidentByIdAsync(id);
            if (accident == null)
            {
                return NotFound(new { success = false, message = "الحادث غير موجود" });
            }

            accident.Title = accidentDto.Title;
            accident.Description = accidentDto.Description;
            accident.Type = accidentDto.Type;
            accident.Status = accidentDto.Status;
            accident.Location = accidentDto.Location;
            accident.MediaUrls = string.Join(",", accidentDto.MediaUrls ?? new List<string>());

            var result = await _accidentService.UpdateAccidentAsync(accident);
            if (!result)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث الحادث" });
            }

            return Ok(new { report = accident, success = true });
        }

        // PUT: api/accidents/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAccidentStatus(Guid id, [FromBody] AccidentStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var result = await _accidentService.UpdateAccidentStatusAsync(id, statusDto.Status);
            if (!result)
            {
                return NotFound(new { success = false, message = "الحادث غير موجود" });
            }

            return Ok(new { success = true, message = "تم تحديث حالة الحادث بنجاح" });
        }

        // POST: api/accidents/{id}/comments
        [HttpPost("{id}/comments")]
        public async Task<ActionResult<Comment>> AddComment(Guid id, [FromBody] CommentCreateDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                Author = commentDto.Author,
                IsAdminComment = commentDto.IsAdminComment
            };

            var createdComment = await _accidentService.AddCommentAsync(id, comment);
            if (createdComment == null)
            {
                return NotFound(new { success = false, message = "الحادث غير موجود" });
            }

            return Ok(new { comment = createdComment, success = true });
        }
    }

    // DTOs
    public class AccidentCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public List<string> MediaUrls { get; set; }
        public Guid GuardId { get; set; }
        public Guid ContractId { get; set; }
    }

    public class AccidentUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public List<string> MediaUrls { get; set; }
    }

    public class AccidentStatusDto
    {
        public string Status { get; set; }
    }

    public class CommentCreateDto
    {
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsAdminComment { get; set; }
    }
}
