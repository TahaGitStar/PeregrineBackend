using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PeregrineBackend.API.Controllers
{
    [ApiController]
    [Route("api/contract-types")]
    public class ContractTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/contract-types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractType>>> GetContractTypes()
        {
            var types = await _context.ContractTypes.ToListAsync();
            return Ok(new { types, success = true });
        }

        // POST: api/contract-types
        [HttpPost]
        public async Task<ActionResult<ContractType>> CreateContractType([FromBody] ContractTypeDto typeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });
            }

            var contractType = new ContractType
            {
                Name = typeDto.Name,
                ArabicName = typeDto.ArabicName,
                IconName = typeDto.IconName
            };

            _context.ContractTypes.Add(contractType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContractTypes), new { type = contractType, success = true });
        }
    }

    // DTOs
    public class ContractTypeDto
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string IconName { get; set; }
    }
}
