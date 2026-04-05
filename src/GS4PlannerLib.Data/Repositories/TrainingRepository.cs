using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data.Repositories;

public class TrainingRepository : ITrainingRepository
{
    private readonly GS4PlannerDbContext _context;

    public TrainingRepository(GS4PlannerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Profession>> GetAllProfessionsAsync()
    {
        return await _context.Professions
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<Profession?> GetProfessionByNameAsync(string name)
    {
        return await _context.Professions
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }

    public async Task<IReadOnlyList<SkillCategory>> GetAllSkillCategoriesAsync()
    {
        return await _context.SkillCategories
            .OrderBy(sc => sc.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Skill>> GetAllSkillsAsync()
    {
        return await _context.Skills
            .Include(s => s.SkillCategory)
            .OrderBy(s => s.SkillCategoryId)
            .ThenBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Skill>> GetSkillsByCategoryTypeAsync(SkillCategoryType categoryType)
    {
        return await _context.Skills
            .Include(s => s.SkillCategory)
            .Where(s => s.SkillCategory.Type == categoryType)
            .OrderBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TrainingCost>> GetTrainingCostsByProfessionAsync(int professionId)
    {
        return await _context.TrainingCosts
            .Include(tc => tc.Skill)
                .ThenInclude(s => s.SkillCategory)
            .Where(tc => tc.ProfessionId == professionId)
            .OrderBy(tc => tc.Skill.SkillCategoryId)
            .ThenBy(tc => tc.SkillId)
            .ToListAsync();
    }

    public async Task<TrainingCost?> GetTrainingCostAsync(int professionId, int skillId)
    {
        return await _context.TrainingCosts
            .Include(tc => tc.Skill)
                .ThenInclude(s => s.SkillCategory)
            .FirstOrDefaultAsync(tc => tc.ProfessionId == professionId && tc.SkillId == skillId);
    }

    public async Task<IReadOnlyList<TrainingCap>> GetTrainingCapsByProfessionAsync(int professionId)
    {
        return await _context.TrainingCaps
            .Include(tc => tc.SkillCategory)
            .Include(tc => tc.Skill)
            .Where(tc => tc.ProfessionId == professionId)
            .ToListAsync();
    }

    public async Task<TrainingCap?> GetCategoryCapAsync(int professionId, int skillCategoryId)
    {
        return await _context.TrainingCaps
            .Include(tc => tc.SkillCategory)
            .FirstOrDefaultAsync(tc =>
                tc.ProfessionId == professionId &&
                tc.SkillCategoryId == skillCategoryId);
    }

    public async Task<TrainingCap?> GetSkillCapAsync(int professionId, int skillId)
    {
        return await _context.TrainingCaps
            .Include(tc => tc.Skill)
            .FirstOrDefaultAsync(tc =>
                tc.ProfessionId == professionId &&
                tc.SkillId == skillId);
    }
}
