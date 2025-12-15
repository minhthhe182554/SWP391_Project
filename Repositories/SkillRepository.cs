using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Skill>> GetAllAsync();
        Task<List<Skill>> GetSkillsByIdsAsync(List<int> ids);
    }

    public class SkillRepository : ISkillRepository
    {
        private readonly EzJobDbContext _context;
        public SkillRepository(EzJobDbContext context) => _context = context;

        public async Task<List<Skill>> GetAllAsync() => await _context.Skills.ToListAsync();

        public async Task<List<Skill>> GetSkillsByIdsAsync(List<int> ids)
        {
            return await _context.Skills.Where(s => ids.Contains(s.Id)).ToListAsync();
        }
    }
}
