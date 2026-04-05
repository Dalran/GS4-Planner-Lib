using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Data.Repositories;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Tests.Training;

/// <summary>
/// Integration-style tests for <see cref="TrainingRepository"/> using an
/// EF Core InMemory database pre-seeded with the real seed data.
/// </summary>
public class TrainingRepositoryTests : IDisposable
{
    private readonly GS4PlannerDbContext _context;
    private readonly TrainingRepository _repository;

    public TrainingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GS4PlannerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GS4PlannerDbContext(options);
        // EnsureCreated applies the seeded data to the in-memory store
        _context.Database.EnsureCreated();

        _repository = new TrainingRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // -------------------------------------------------------------------------
    // Professions
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAllProfessionsAsync_Returns10Professions()
    {
        var professions = await _repository.GetAllProfessionsAsync();
        Assert.Equal(10, professions.Count);
    }

    [Theory]
    [InlineData("Warrior")]
    [InlineData("Rogue")]
    [InlineData("Monk")]
    [InlineData("Ranger")]
    [InlineData("Bard")]
    [InlineData("Paladin")]
    [InlineData("Cleric")]
    [InlineData("Empath")]
    [InlineData("Wizard")]
    [InlineData("Sorcerer")]
    public async Task GetAllProfessionsAsync_ContainsExpectedProfession(string name)
    {
        var professions = await _repository.GetAllProfessionsAsync();
        Assert.Contains(professions, p => p.Name == name);
    }

    [Fact]
    public async Task GetProfessionByNameAsync_CaseInsensitive_ReturnsMatch()
    {
        var cleric = await _repository.GetProfessionByNameAsync("cleric");
        Assert.NotNull(cleric);
        Assert.Equal("Cleric", cleric!.Name);
    }

    [Fact]
    public async Task GetProfessionByNameAsync_UnknownName_ReturnsNull()
    {
        var result = await _repository.GetProfessionByNameAsync("Necromancer");
        Assert.Null(result);
    }

    // -------------------------------------------------------------------------
    // Skill Categories
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAllSkillCategoriesAsync_Returns6Categories()
    {
        var categories = await _repository.GetAllSkillCategoriesAsync();
        Assert.Equal(6, categories.Count);
    }

    [Theory]
    [InlineData("Standard",      SkillCategoryType.Standard)]
    [InlineData("Spell Research",SkillCategoryType.SpellResearch)]
    [InlineData("Elemental Lore",SkillCategoryType.ElementalLore)]
    [InlineData("Spiritual Lore",SkillCategoryType.SpiritualLore)]
    [InlineData("Mental Lore",   SkillCategoryType.MentalLore)]
    [InlineData("Sorcerous Lore",SkillCategoryType.SorcerousLore)]
    public async Task GetAllSkillCategoriesAsync_ContainsExpectedCategory(string name, SkillCategoryType type)
    {
        var categories = await _repository.GetAllSkillCategoriesAsync();
        Assert.Contains(categories, sc => sc.Name == name && sc.Type == type);
    }

    // -------------------------------------------------------------------------
    // Skills
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAllSkillsAsync_Returns36Skills()
    {
        var skills = await _repository.GetAllSkillsAsync();
        // 11 standard + 11 spell research + 4+3+5+2 lore = 36
        Assert.Equal(36, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_Standard_Returns11Skills()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.Standard);
        Assert.Equal(11, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_SpellResearch_Returns11Circles()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.SpellResearch);
        Assert.Equal(11, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_ElementalLore_Returns4Skills()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.ElementalLore);
        Assert.Equal(4, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_SpiritualLore_Returns3Skills()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.SpiritualLore);
        Assert.Equal(3, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_MentalLore_Returns5Skills()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.MentalLore);
        Assert.Equal(5, skills.Count);
    }

    [Fact]
    public async Task GetSkillsByCategoryTypeAsync_SorcerousLore_Returns2Skills()
    {
        var skills = await _repository.GetSkillsByCategoryTypeAsync(SkillCategoryType.SorcerousLore);
        Assert.Equal(2, skills.Count);
    }

    // -------------------------------------------------------------------------
    // Warrior Training Costs
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetTrainingCostsByProfession_Warrior_HasCostForAllStandardSkills()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        Assert.NotNull(warrior);

        var costs = await _repository.GetTrainingCostsByProfessionAsync(warrior!.Id);
        var standardCosts = costs.Where(tc => tc.Skill.SkillCategory.Type == SkillCategoryType.Standard).ToList();

        Assert.Equal(11, standardCosts.Count);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Warrior_ArmorUse_Returns2Physical0Mental()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        Assert.NotNull(warrior);

        var armorUseSkill = await _context.Skills.FirstAsync(s => s.Name == "Armor Use");
        var cost = await _repository.GetTrainingCostAsync(warrior!.Id, armorUseSkill.Id);

        Assert.NotNull(cost);
        Assert.Equal(2, cost!.PhysicalCost);
        Assert.Equal(0, cost.MentalCost);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Warrior_ElementalLoreAir_Returns0Physical15Mental()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Elemental Lore, Air");

        var cost = await _repository.GetTrainingCostAsync(warrior!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(0, cost!.PhysicalCost);
        Assert.Equal(15, cost.MentalCost);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Warrior_HasNoSpellResearchCosts()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        Assert.NotNull(warrior);

        var costs = await _repository.GetTrainingCostsByProfessionAsync(warrior!.Id);
        var spellCosts = costs.Where(tc => tc.Skill.SkillCategory.Type == SkillCategoryType.SpellResearch).ToList();

        Assert.Empty(spellCosts);
    }

    // -------------------------------------------------------------------------
    // Warrior Training Caps
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetSkillCapAsync_Warrior_ArmorUse_Returns3PerLevel()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Armor Use");

        var cap = await _repository.GetSkillCapAsync(warrior!.Id, skill.Id);

        Assert.NotNull(cap);
        Assert.Equal(3, cap!.MaxRanksPerLevel);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Warrior_ElementalLore_Returns1PerLevel()
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.ElementalLore);

        var cap = await _repository.GetCategoryCapAsync(warrior!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(1, cap!.MaxRanksPerLevel);
    }

    // -------------------------------------------------------------------------
    // Cleric Training Costs & Caps
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetTrainingCostAsync_Cleric_ArmorUse_Returns8Physical0Mental()
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Armor Use");

        var cost = await _repository.GetTrainingCostAsync(cleric!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(8, cost!.PhysicalCost);
        Assert.Equal(0, cost.MentalCost);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Cleric_ClericBase_Returns0Physical8Mental()
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Cleric Base");

        var cost = await _repository.GetTrainingCostAsync(cleric!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(0, cost!.PhysicalCost);
        Assert.Equal(8, cost.MentalCost);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Cleric_SpellResearch_Returns3PerLevel()
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.SpellResearch);

        var cap = await _repository.GetCategoryCapAsync(cleric!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(3, cap!.MaxRanksPerLevel);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Cleric_SpiritualLore_Returns2PerLevel()
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.SpiritualLore);

        var cap = await _repository.GetCategoryCapAsync(cleric!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(2, cap!.MaxRanksPerLevel);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Cleric_ElementalLore_Returns1PerLevel()
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.ElementalLore);

        var cap = await _repository.GetCategoryCapAsync(cleric!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(1, cap!.MaxRanksPerLevel);
    }

    // -------------------------------------------------------------------------
    // Wizard Training Costs & Caps
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetTrainingCostAsync_Wizard_ArmorUse_Returns14Physical0Mental()
    {
        var wizard = await _repository.GetProfessionByNameAsync("Wizard");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Armor Use");

        var cost = await _repository.GetTrainingCostAsync(wizard!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(14, cost!.PhysicalCost);
        Assert.Equal(0, cost.MentalCost);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Wizard_WizardBase_Returns0Physical8Mental()
    {
        var wizard = await _repository.GetProfessionByNameAsync("Wizard");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Wizard Base");

        var cost = await _repository.GetTrainingCostAsync(wizard!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(0, cost!.PhysicalCost);
        Assert.Equal(8, cost.MentalCost);
    }

    [Fact]
    public async Task GetTrainingCostAsync_Wizard_ElementalLoreAir_Returns0Physical6Mental()
    {
        var wizard = await _repository.GetProfessionByNameAsync("Wizard");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Elemental Lore, Air");

        var cost = await _repository.GetTrainingCostAsync(wizard!.Id, skill.Id);

        Assert.NotNull(cost);
        Assert.Equal(0, cost!.PhysicalCost);
        Assert.Equal(6, cost.MentalCost);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Wizard_SpellResearch_Returns3PerLevel()
    {
        var wizard = await _repository.GetProfessionByNameAsync("Wizard");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.SpellResearch);

        var cap = await _repository.GetCategoryCapAsync(wizard!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(3, cap!.MaxRanksPerLevel);
    }

    [Fact]
    public async Task GetCategoryCapAsync_Wizard_ElementalLore_Returns3PerLevel()
    {
        var wizard = await _repository.GetProfessionByNameAsync("Wizard");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.ElementalLore);

        var cap = await _repository.GetCategoryCapAsync(wizard!.Id, category.Id);

        Assert.NotNull(cap);
        Assert.Equal(3, cap!.MaxRanksPerLevel);
    }

    // -------------------------------------------------------------------------
    // Cumulative cap calculation example
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(0,  0)]
    [InlineData(1,  3)]
    [InlineData(5,  15)]
    [InlineData(10, 30)]
    public async Task CumulativeCap_Warrior_ArmorUse_CorrectAtLevel(int level, int expectedMaxRanks)
    {
        var warrior = await _repository.GetProfessionByNameAsync("Warrior");
        var skill = await _context.Skills.FirstAsync(s => s.Name == "Armor Use");

        var cap = await _repository.GetSkillCapAsync(warrior!.Id, skill.Id);

        Assert.NotNull(cap);
        int cumulativeMax = cap!.MaxRanksPerLevel * level;
        Assert.Equal(expectedMaxRanks, cumulativeMax);
    }

    [Theory]
    [InlineData(0,  0)]
    [InlineData(1,  3)]
    [InlineData(10, 30)]
    public async Task CumulativeCap_Cleric_SpellResearch_CorrectAtLevel(int level, int expectedMaxRanks)
    {
        var cleric = await _repository.GetProfessionByNameAsync("Cleric");
        var category = await _context.SkillCategories.FirstAsync(sc => sc.Type == SkillCategoryType.SpellResearch);

        var cap = await _repository.GetCategoryCapAsync(cleric!.Id, category.Id);

        Assert.NotNull(cap);
        int cumulativeMax = cap!.MaxRanksPerLevel * level;
        Assert.Equal(expectedMaxRanks, cumulativeMax);
    }
}
