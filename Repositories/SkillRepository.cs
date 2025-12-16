using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Skill>> GetAllAsync();
        Task<List<Skill>> GetSkillsByIdsAsync(List<int> ids);
        Task<Skill> GetOrCreateAsync(string skillName);
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

        public async Task<Skill> GetOrCreateAsync(string skillName)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skillName);
            if (skill == null)
            {
                skill = new Skill { Name = skillName}; 
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();
            }
            return skill;
        }
    }
}
