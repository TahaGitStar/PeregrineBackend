using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PeregrineBackend.API.Services
{
    public interface IBranchService
    {
        Task<List<Branch>> GetAllBranchesAsync(bool includeInactive = false);
        Task<Branch> GetBranchByIdAsync(Guid id);
        Task<Branch> CreateBranchAsync(Branch branch);
        Task<bool> UpdateBranchAsync(Branch branch);
        Task<bool> DeleteBranchAsync(Guid id);
    }

    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> GetAllBranchesAsync(bool includeInactive = false)
        {
            var query = _context.Branches.AsQueryable();
            
            if (!includeInactive)
                query = query.Where(b => b.IsActive);
                
            return await query.Include(b => b.Contracts).ToListAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(Guid id)
        {
            return await _context.Branches
                .Include(b => b.Contracts)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Branch> CreateBranchAsync(Branch branch)
        {
            branch.Id = Guid.NewGuid();
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<bool> UpdateBranchAsync(Branch branch)
        {
            _context.Entry(branch).State = EntityState.Modified;
            
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

        public async Task<bool> DeleteBranchAsync(Guid id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return false;

            // بدلاً من الحذف الفعلي، نقوم بتعطيل الفرع
            branch.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
