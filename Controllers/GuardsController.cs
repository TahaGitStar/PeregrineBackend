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
    public class GuardsController : ControllerBase
    {
        private readonly IGuardService _guardService;

        public GuardsController(IGuardService guardService)
        {
            _guardService = guardService;
        }

        // GET: api/guards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guard>>> GetGuards([FromQuery] bool includeInactive = false)
        {
            var guards = await _guardService.GetAllGuardsAsync(includeInactive);
            return Ok(new { guards, total = guards.Count, success = true });
        }

        // GET: api/guards/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Guard>> GetGuard(Guid id)
        {
            var guard = await _guardService.GetGuardByIdAsync(id);

            if (guard == null)
            {
                return NotFound(new { success = false, message = "الحارس غير موجود" });
            }

            return Ok(new { guard, success = true });
        }

        // GET: api/guards/contract/{contractId}
        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<Guard>>> GetGuardsByContract(Guid contractId)
        {
            var guards = await _guardService.GetGuardsByContractAsync(contractId);
            return Ok(new { guards, total = guards.Count, success = true });
        }

        // POST: api/guards
        [HttpPost]
        public async Task<ActionResult<Guard>> CreateGuard([FromBody] GuardCreateDto guardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var guard = new Guard
            {
                Name = guardDto.Name,
                BadgeNumber = guardDto.BadgeNumber,
                PhoneNumber = guardDto.PhoneNumber,
                ProfileImageUrl = guardDto.ProfileImageUrl,
                Specialization = guardDto.Specialization,
                ContractId = guardDto.ContractId,
                IsActive = true,
                Schedule = new List<WorkSchedule>(),
                LeaveDays = new List<LeaveDay>()
            };

            // إضافة جداول العمل إذا كانت موجودة
            if (guardDto.Schedule != null && guardDto.Schedule.Count > 0)
            {
                foreach (var scheduleDto in guardDto.Schedule)
                {
                    guard.Schedule.Add(new WorkSchedule
                    {
                        DayOfWeek = scheduleDto.DayOfWeek,
                        StartTime = scheduleDto.StartTime,
                        EndTime = scheduleDto.EndTime,
                        Location = scheduleDto.Location
                    });
                }
            }

            var createdGuard = await _guardService.CreateGuardAsync(guard);

            return CreatedAtAction(nameof(GetGuard), new { id = createdGuard.Id }, new { guard = createdGuard, success = true });
        }

        // PUT: api/guards/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuard(Guid id, [FromBody] GuardUpdateDto guardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var guard = await _guardService.GetGuardByIdAsync(id);
            if (guard == null)
            {
                return NotFound(new { success = false, message = "الحارس غير موجود" });
            }

            guard.Name = guardDto.Name;
            guard.BadgeNumber = guardDto.BadgeNumber;
            guard.PhoneNumber = guardDto.PhoneNumber;
            guard.ProfileImageUrl = guardDto.ProfileImageUrl;
            guard.Specialization = guardDto.Specialization;
            guard.ContractId = guardDto.ContractId;
            guard.IsActive = guardDto.IsActive;

            var result = await _guardService.UpdateGuardAsync(guard);
            if (!result)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث الحارس" });
            }

            return Ok(new { guard, success = true });
        }

        // DELETE: api/guards/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuard(Guid id)
        {
            var result = await _guardService.DeleteGuardAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = "الحارس غير موجود" });
            }

            return Ok(new { success = true, message = "تم تعطيل الحارس بنجاح" });
        }

        // POST: api/guards/{id}/schedules
        [HttpPost("{id}/schedules")]
        public async Task<ActionResult<WorkSchedule>> AddSchedule(Guid id, [FromBody] WorkScheduleDto scheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var schedule = new WorkSchedule
            {
                DayOfWeek = scheduleDto.DayOfWeek,
                StartTime = scheduleDto.StartTime,
                EndTime = scheduleDto.EndTime,
                Location = scheduleDto.Location
            };

            var createdSchedule = await _guardService.AddScheduleAsync(id, schedule);
            if (createdSchedule == null)
            {
                return NotFound(new { success = false, message = "الحارس غير موجود" });
            }

            return Ok(new { schedule = createdSchedule, success = true });
        }

        // POST: api/guards/{id}/leaves
        [HttpPost("{id}/leaves")]
        public async Task<ActionResult<LeaveDay>> AddLeave(Guid id, [FromBody] LeaveDayDto leaveDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var leave = new LeaveDay
            {
                StartDate = leaveDto.StartDate,
                EndDate = leaveDto.EndDate,
                Reason = leaveDto.Reason,
                ReplacementId = leaveDto.ReplacementId
            };

            var createdLeave = await _guardService.AddLeaveAsync(id, leave);
            if (createdLeave == null)
            {
                return NotFound(new { success = false, message = "الحارس غير موجود" });
            }

            return Ok(new { leaveDay = createdLeave, success = true });
        }

        // PUT: api/guards/leaves/{leaveId}/status
        [HttpPut("leaves/{leaveId}/status")]
        public async Task<IActionResult> UpdateLeaveStatus(int leaveId, [FromBody] LeaveStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var result = await _guardService.UpdateLeaveStatusAsync(leaveId, statusDto.Status);
            if (!result)
            {
                return NotFound(new { success = false, message = "الإجازة غير موجودة" });
            }

            return Ok(new { success = true, message = "تم تحديث حالة الإجازة بنجاح" });
        }
    }

    // DTOs
    public class GuardCreateDto
    {
        public string Name { get; set; }
        public string BadgeNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Specialization { get; set; }
        public Guid ContractId { get; set; }
        public List<WorkScheduleDto> Schedule { get; set; }
    }

    public class GuardUpdateDto
    {
        public string Name { get; set; }
        public string BadgeNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Specialization { get; set; }
        public Guid ContractId { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkScheduleDto
    {
        public int DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
    }

    public class LeaveDayDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public Guid? ReplacementId { get; set; }
    }

    public class LeaveStatusDto
    {
        public string Status { get; set; }
    }
}
