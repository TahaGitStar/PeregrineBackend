using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PeregrineBackend.API.Services
{
    public interface IGuardService
    {
        Task<List<Guard>> GetAllGuardsAsync(bool includeInactive = false);
        Task<Guard> GetGuardByIdAsync(Guid id);
        Task<List<Guard>> GetGuardsByContractAsync(Guid contractId);
        Task<Guard> CreateGuardAsync(Guard guard);
        Task<bool> UpdateGuardAsync(Guard guard);
        Task<bool> DeleteGuardAsync(Guid id);
        Task<WorkSchedule> AddScheduleAsync(Guid guardId, WorkSchedule schedule);
        Task<LeaveDay> AddLeaveAsync(Guid guardId, LeaveDay leave);
        Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);
    }

    public class GuardService : IGuardService
    {
        private readonly ApplicationDbContext _context;

        public GuardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Guard>> GetAllGuardsAsync(bool includeInactive = false)
        {
            var query = _context.Guards
                .Include(g => g.Contract)
                .Include(g => g.Schedule)
                .Include(g => g.LeaveDays)
                .AsQueryable();
                
            if (!includeInactive)
                query = query.Where(g => g.IsActive);
                
            return await query.ToListAsync();
        }

        public async Task<Guard> GetGuardByIdAsync(Guid id)
        {
            return await _context.Guards
                .Include(g => g.Contract)
                .Include(g => g.Schedule)
                .Include(g => g.LeaveDays)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Guard>> GetGuardsByContractAsync(Guid contractId)
        {
            return await _context.Guards
                .Include(g => g.Schedule)
                .Include(g => g.LeaveDays)
                .Where(g => g.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<Guard> CreateGuardAsync(Guard guard)
        {
            guard.Id = Guid.NewGuid();
            _context.Guards.Add(guard);
            await _context.SaveChangesAsync();
            return guard;
        }

        public async Task<bool> UpdateGuardAsync(Guard guard)
        {
            _context.Entry(guard).State = EntityState.Modified;
            
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

        public async Task<bool> DeleteGuardAsync(Guid id)
        {
            var guard = await _context.Guards.FindAsync(id);
            if (guard == null)
                return false;

            // بدلاً من الحذف الفعلي، نقوم بتعطيل الحارس
            guard.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WorkSchedule> AddScheduleAsync(Guid guardId, WorkSchedule schedule)
        {
            var guard = await _context.Guards.FindAsync(guardId);
            if (guard == null)
                return null;

            schedule.GuardId = guardId;
            _context.WorkSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<LeaveDay> AddLeaveAsync(Guid guardId, LeaveDay leave)
        {
            var guard = await _context.Guards.FindAsync(guardId);
            if (guard == null)
                return null;

            leave.GuardId = guardId;
            leave.Status = "pending"; // الحالة الافتراضية هي "معلق"
            _context.LeaveDays.Add(leave);
            await _context.SaveChangesAsync();
            return leave;
        }

        public async Task<bool> UpdateLeaveStatusAsync(int leaveId, string status)
        {
            var leave = await _context.LeaveDays.FindAsync(leaveId);
            if (leave == null)
                return false;

            leave.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
