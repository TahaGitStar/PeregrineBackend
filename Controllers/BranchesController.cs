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
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        // GET: api/branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches([FromQuery] bool includeInactive = false)
        {
            var branches = await _branchService.GetAllBranchesAsync(includeInactive);
            return Ok(new { branches, total = branches.Count, success = true });
        }

        // GET: api/branches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(Guid id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);

            if (branch == null)
            {
                return NotFound(new { success = false, message = "الفرع غير موجود" });
            }

            return Ok(new { branch, success = true });
        }

        // POST: api/branches
        [HttpPost]
        public async Task<ActionResult<Branch>> CreateBranch([FromBody] BranchCreateDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var branch = new Branch
            {
                Name = branchDto.Name,
                Address = branchDto.Address,
                PhoneNumber = branchDto.PhoneNumber,
                ManagerName = branchDto.ManagerName,
                ManagerEmail = branchDto.ManagerEmail,
                ManagerPhone = branchDto.ManagerPhone,
                IsActive = true
            };

            var createdBranch = await _branchService.CreateBranchAsync(branch);

            return CreatedAtAction(nameof(GetBranch), new { id = createdBranch.Id }, new { branch = createdBranch, success = true });
        }

        // PUT: api/branches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] BranchUpdateDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound(new { success = false, message = "الفرع غير موجود" });
            }

            branch.Name = branchDto.Name;
            branch.Address = branchDto.Address;
            branch.PhoneNumber = branchDto.PhoneNumber;
            branch.ManagerName = branchDto.ManagerName;
            branch.ManagerEmail = branchDto.ManagerEmail;
            branch.ManagerPhone = branchDto.ManagerPhone;
            branch.IsActive = branchDto.IsActive;

            var result = await _branchService.UpdateBranchAsync(branch);
            if (!result)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث الفرع" });
            }

            return Ok(new { branch, success = true });
        }

        // DELETE: api/branches/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var result = await _branchService.DeleteBranchAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = "الفرع غير موجود" });
            }

            return Ok(new { success = true, message = "تم تعطيل الفرع بنجاح" });
        }
    }

    // DTOs
    public class BranchCreateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhone { get; set; }
    }

    public class BranchUpdateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerPhone { get; set; }
        public bool IsActive { get; set; }
    }
}
