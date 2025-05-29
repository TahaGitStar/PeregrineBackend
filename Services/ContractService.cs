using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PeregrineBackend.API.Services
{
    public interface IContractService
    {
        Task<List<Contract>> GetAllContractsAsync(string status = null, string type = null);
        Task<Contract> GetContractByIdAsync(Guid id);
        Task<List<Contract>> GetContractsByBranchAsync(Guid branchId);
        Task<Contract> CreateContractAsync(Contract contract);
        Task<bool> UpdateContractAsync(Contract contract);
        Task<bool> DeleteContractAsync(Guid id);
    }

    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _context;

        public ContractService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllContractsAsync(string status = null, string type = null)
        {
            var query = _context.Contracts
                .Include(c => c.Branch)
                .Include(c => c.Guards)
                .AsQueryable();
                
            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);
                
            if (!string.IsNullOrEmpty(type))
                query = query.Where(c => c.Type == type);
                
            return await query.ToListAsync();
        }

        public async Task<Contract> GetContractByIdAsync(Guid id)
        {
            return await _context.Contracts
                .Include(c => c.Branch)
                .Include(c => c.Guards)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contract>> GetContractsByBranchAsync(Guid branchId)
        {
            return await _context.Contracts
                .Include(c => c.Guards)
                .Where(c => c.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<Contract> CreateContractAsync(Contract contract)
        {
            contract.Id = Guid.NewGuid();
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            _context.Entry(contract).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteContractAsync(Guid id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return false;

            // بدلاً من الحذف الفعلي، نقوم بتغيير حالة العقد إلى "منتهي"
            contract.Status = "terminated";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
