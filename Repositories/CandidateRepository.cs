using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly EzJobDbContext _context;

        public CandidateRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Candidate?> GetByUserIdAsync(int userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Candidate?> GetProfileByUserIdAsync(int userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.EducationRecords)
                .Include(c => c.WorkExperiences)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> UpdateCandidateAsync(Candidate candidate)
        {
            try
            {
                _context.Candidates.Update(candidate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;
                
                user.Password = newPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddEducationRecordAsync(EducationRecord record)
        {
            try
            {
                await _context.EducationRecords.AddAsync(record);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEducationRecordAsync(int recordId, int candidateId)
        {
            try
            {
                var record = await _context.EducationRecords
                    .FirstOrDefaultAsync(e => e.Id == recordId && e.CandidateId == candidateId);
                if (record == null) return false;
                
                _context.EducationRecords.Remove(record);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddWorkExperienceAsync(WorkExperience experience)
        {
            try
            {
                await _context.WorkExperiences.AddAsync(experience);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteWorkExperienceAsync(int experienceId, int candidateId)
        {
            try
            {
                var experience = await _context.WorkExperiences
                    .FirstOrDefaultAsync(w => w.Id == experienceId && w.CandidateId == candidateId);
                if (experience == null) return false;
                
                _context.WorkExperiences.Remove(experience);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddSkillToCandidateAsync(int candidateId, int skillId)
        {
            try
            {
                var candidate = await _context.Candidates
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == candidateId);
                var skill = await _context.Skills.FindAsync(skillId);
                
                if (candidate == null || skill == null) return false;
                if (candidate.Skills.Any(s => s.Id == skillId)) return true; // Already exists
                
                candidate.Skills.Add(skill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveSkillFromCandidateAsync(int candidateId, int skillId)
        {
            try
            {
                var candidate = await _context.Candidates
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == candidateId);
                
                if (candidate == null) return false;
                
                var skill = candidate.Skills.FirstOrDefault(s => s.Id == skillId);
                if (skill == null) return false;
                
                candidate.Skills.Remove(skill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<bool> CreateAndAddSkillAsync(int candidateId, string skillName)
        {
            try
            {
                // Check if skill already exists
                var existingSkill = await _context.Skills
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == skillName.ToLower());

                if (existingSkill != null)
                {
                    // If skill exists, just add it to candidate
                    return await AddSkillToCandidateAsync(candidateId, existingSkill.Id);
                }

                // Create new skill
                var newSkill = new Skill
                {
                    Name = skillName.Trim()
                };

                await _context.Skills.AddAsync(newSkill);
                await _context.SaveChangesAsync();

                // Add the new skill to candidate
                return await AddSkillToCandidateAsync(candidateId, newSkill.Id);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfileImageAsync(int candidateId, string publicId)
        {
            try
            {
                var candidate = await _context.Candidates.FindAsync(candidateId);
                if (candidate == null) return false;

                candidate.ImageUrl = publicId;
                _context.Candidates.Update(candidate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Job>> GetRecommendedJobsAsync(int limit = 10)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Job>> GetAllActiveJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
                .OrderByDescending(j => j.StartDate)
                .ToListAsync();
        }
    }
}
