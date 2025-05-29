using Microsoft.EntityFrameworkCore;
using PeregrineBackend.API.Models;
using PeregrineBackend.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PeregrineBackend.API.Services
{
    public interface IAccidentService
    {
        Task<List<Accident>> GetAllAccidentsAsync(string type = null, string status = null);
        Task<Accident> GetAccidentByIdAsync(Guid id);
        Task<List<Accident>> GetAccidentsByGuardAsync(Guid guardId);
        Task<List<Accident>> GetAccidentsByContractAsync(Guid contractId);
        Task<Accident> CreateAccidentAsync(Accident accident);
        Task<bool> UpdateAccidentAsync(Accident accident);
        Task<bool> UpdateAccidentStatusAsync(Guid id, string status);
        Task<Comment> AddCommentAsync(Guid accidentId, Comment comment);
    }

    public class AccidentService : IAccidentService
    {
        private readonly ApplicationDbContext _context;

        public AccidentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Accident>> GetAllAccidentsAsync(string type = null, string status = null)
        {
            var query = _context.Accidents
                .Include(a => a.Guard)
                .Include(a => a.Contract)
                .Include(a => a.Comments)
                .AsQueryable();
                
            if (!string.IsNullOrEmpty(type))
                query = query.Where(a => a.Type == type);
                
            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);
                
            return await query.ToListAsync();
        }

        public async Task<Accident> GetAccidentByIdAsync(Guid id)
        {
            return await _context.Accidents
                .Include(a => a.Guard)
                .Include(a => a.Contract)
                .Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Accident>> GetAccidentsByGuardAsync(Guid guardId)
        {
            return await _context.Accidents
                .Include(a => a.Comments)
                .Where(a => a.GuardId == guardId)
                .ToListAsync();
        }

        public async Task<List<Accident>> GetAccidentsByContractAsync(Guid contractId)
        {
            return await _context.Accidents
                .Include(a => a.Guard)
                .Include(a => a.Comments)
                .Where(a => a.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<Accident> CreateAccidentAsync(Accident accident)
        {
            accident.Id = Guid.NewGuid();
            accident.DateTime = DateTime.Now;
            _context.Accidents.Add(accident);
            await _context.SaveChangesAsync();
            return accident;
        }

        public async Task<bool> UpdateAccidentAsync(Accident accident)
        {
            _context.Entry(accident).State = EntityState.Modified;
            
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

        public async Task<bool> UpdateAccidentStatusAsync(Guid id, string status)
        {
            var accident = await _context.Accidents.FindAsync(id);
            if (accident == null)
                return false;

            accident.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment> AddCommentAsync(Guid accidentId, Comment comment)
        {
            var accident = await _context.Accidents.FindAsync(accidentId);
            if (accident == null)
                return null;

            comment.AccidentId = accidentId;
            comment.DateTime = DateTime.Now;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
