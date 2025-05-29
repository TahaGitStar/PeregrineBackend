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
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        // GET: api/contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            [FromQuery] string status = null, 
            [FromQuery] string type = null)
        {
            var contracts = await _contractService.GetAllContractsAsync(status, type);
            return Ok(new { contracts, total = contracts.Count, success = true });
        }

        // GET: api/contracts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(Guid id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound(new { success = false, message = "العقد غير موجود" });
            }

            return Ok(new { contract, success = true });
        }

        // GET: api/contracts/branch/{branchId}
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByBranch(Guid branchId)
        {
            var contracts = await _contractService.GetContractsByBranchAsync(branchId);
            return Ok(new { contracts, total = contracts.Count, success = true });
        }

        // POST: api/contracts
        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract([FromBody] ContractCreateDto contractDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var contract = new Contract
            {
                Title = contractDto.Title,
                BranchId = contractDto.BranchId,
                ClientId = contractDto.ClientId,
                StartDate = contractDto.StartDate,
                EndDate = contractDto.EndDate,
                Status = contractDto.Status,
                Type = contractDto.Type,
                GuardsCount = contractDto.GuardsCount,
                Value = contractDto.Value,
                Notes = contractDto.Notes
            };

            var createdContract = await _contractService.CreateContractAsync(contract);

            return CreatedAtAction(nameof(GetContract), new { id = createdContract.Id }, new { contract = createdContract, success = true });
        }

        // PUT: api/contracts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(Guid id, [FromBody] ContractUpdateDto contractDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
            {
                return NotFound(new { success = false, message = "العقد غير موجود" });
            }

            contract.Title = contractDto.Title;
            contract.BranchId = contractDto.BranchId;
            contract.ClientId = contractDto.ClientId;
            contract.StartDate = contractDto.StartDate;
            contract.EndDate = contractDto.EndDate;
            contract.Status = contractDto.Status;
            contract.Type = contractDto.Type;
            contract.GuardsCount = contractDto.GuardsCount;
            contract.Value = contractDto.Value;
            contract.Notes = contractDto.Notes;

            var result = await _contractService.UpdateContractAsync(contract);
            if (!result)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث العقد" });
            }

            return Ok(new { contract, success = true });
        }

        // DELETE: api/contracts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(Guid id)
        {
            var result = await _contractService.DeleteContractAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = "العقد غير موجود" });
            }

            return Ok(new { success = true, message = "تم إنهاء العقد بنجاح" });
        }
    }

    // DTOs
    public class ContractCreateDto
    {
        public string Title { get; set; }
        public Guid BranchId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int GuardsCount { get; set; }
        public decimal Value { get; set; }
        public string Notes { get; set; }
    }

    public class ContractUpdateDto
    {
        public string Title { get; set; }
        public Guid BranchId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int GuardsCount { get; set; }
        public decimal Value { get; set; }
        public string Notes { get; set; }
    }
}
