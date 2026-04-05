using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Data.Repositories;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Tests;

public class RepositoryTests
{
    private static GS4PlannerDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GS4PlannerDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new GS4PlannerDbContext(options);
    }

    // ─── Character repository ────────────────────────────────────────────────

    [Fact]
    public async Task CharacterRepository_AddAndGetById_ReturnsCharacter()
    {
        using var context = CreateContext(nameof(CharacterRepository_AddAndGetById_ReturnsCharacter));
        var repo = new CharacterRepository(context);

        var character = new Character { Name = "Arinthia", Race = "Elf", Profession = "Wizard", Level = 5 };
        await repo.AddAsync(character);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(character.Id);

        Assert.NotNull(result);
        Assert.Equal("Arinthia", result!.Name);
    }

    [Fact]
    public async Task CharacterRepository_GetByName_ReturnsCorrectCharacter()
    {
        using var context = CreateContext(nameof(CharacterRepository_GetByName_ReturnsCorrectCharacter));
        var repo = new CharacterRepository(context);

        await repo.AddAsync(new Character { Name = "Borrin", Race = "Dwarf", Profession = "Warrior", Level = 10 });
        await context.SaveChangesAsync();

        var result = await repo.GetByNameAsync("borrin");

        Assert.NotNull(result);
        Assert.Equal("Borrin", result!.Name);
    }

    [Fact]
    public async Task CharacterRepository_ListAll_ReturnsAllCharacters()
    {
        using var context = CreateContext(nameof(CharacterRepository_ListAll_ReturnsAllCharacters));
        var repo = new CharacterRepository(context);

        await repo.AddAsync(new Character { Name = "C1", Race = "Human", Profession = "Cleric", Level = 1 });
        await repo.AddAsync(new Character { Name = "C2", Race = "Halfling", Profession = "Rogue", Level = 2 });
        await context.SaveChangesAsync();

        var results = await repo.ListAllAsync();

        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task CharacterRepository_Remove_DeletesCharacter()
    {
        using var context = CreateContext(nameof(CharacterRepository_Remove_DeletesCharacter));
        var repo = new CharacterRepository(context);

        var character = new Character { Name = "ToDelete", Race = "Gnome", Profession = "Bard", Level = 3 };
        await repo.AddAsync(character);
        await context.SaveChangesAsync();

        repo.Remove(character);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(character.Id);
        Assert.Null(result);
    }

    // ─── TrainingPlan repository ─────────────────────────────────────────────

    [Fact]
    public async Task TrainingPlanRepository_GetByCharacterId_ReturnsCorrectPlans()
    {
        using var context = CreateContext(nameof(TrainingPlanRepository_GetByCharacterId_ReturnsCorrectPlans));
        var charRepo = new CharacterRepository(context);
        var planRepo = new TrainingPlanRepository(context);

        var character = new Character { Name = "Mira", Race = "Human", Profession = "Empath", Level = 1 };
        await charRepo.AddAsync(character);
        await context.SaveChangesAsync();

        await planRepo.AddAsync(new TrainingPlan { Name = "Plan A", CharacterId = character.Id });
        await planRepo.AddAsync(new TrainingPlan { Name = "Plan B", CharacterId = character.Id });
        await context.SaveChangesAsync();

        var plans = await planRepo.GetByCharacterIdAsync(character.Id);

        Assert.Equal(2, plans.Count());
    }

    // ─── TrainingGoal repository ─────────────────────────────────────────────

    [Fact]
    public async Task TrainingGoalRepository_GetByTrainingPlanId_ReturnsPrioritized()
    {
        using var context = CreateContext(nameof(TrainingGoalRepository_GetByTrainingPlanId_ReturnsPrioritized));
        var charRepo = new CharacterRepository(context);
        var planRepo = new TrainingPlanRepository(context);
        var goalRepo = new TrainingGoalRepository(context);

        var character = new Character { Name = "Kelos", Race = "Elf", Profession = "Ranger", Level = 1 };
        await charRepo.AddAsync(character);
        await context.SaveChangesAsync();

        var plan = new TrainingPlan { Name = "Ranger Build", CharacterId = character.Id };
        await planRepo.AddAsync(plan);
        await context.SaveChangesAsync();

        await goalRepo.AddAsync(new TrainingGoal { SkillName = "Bows", TargetRanks = 50, Priority = 2, TrainingPlanId = plan.Id });
        await goalRepo.AddAsync(new TrainingGoal { SkillName = "Survival", TargetRanks = 30, Priority = 1, TrainingPlanId = plan.Id });
        await context.SaveChangesAsync();

        var goals = (await goalRepo.GetByTrainingPlanIdAsync(plan.Id)).ToList();

        Assert.Equal(2, goals.Count);
        Assert.Equal("Survival", goals[0].SkillName);  // priority 1 comes first
        Assert.Equal("Bows", goals[1].SkillName);       // priority 2 comes second
    }

    // ─── UnitOfWork ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UnitOfWork_SaveChanges_PersistsAllRepositories()
    {
        var options = new DbContextOptionsBuilder<GS4PlannerDbContext>()
            .UseInMemoryDatabase(nameof(UnitOfWork_SaveChanges_PersistsAllRepositories))
            .Options;

        using var context = new GS4PlannerDbContext(options);
        using var uow = new UnitOfWork(context);

        var character = new Character { Name = "Zephir", Race = "Sylvankind", Profession = "Bard", Level = 1 };
        await uow.Characters.AddAsync(character);
        await uow.SaveChangesAsync();

        var plan = new TrainingPlan { Name = "Bard Build", CharacterId = character.Id };
        await uow.TrainingPlans.AddAsync(plan);
        await uow.SaveChangesAsync();

        await uow.TrainingGoals.AddAsync(new TrainingGoal { SkillName = "Singing", TargetRanks = 20, Priority = 1, TrainingPlanId = plan.Id });
        int saved = await uow.SaveChangesAsync();

        Assert.Equal(1, saved);

        var goals = await uow.TrainingGoals.GetByTrainingPlanIdAsync(plan.Id);
        Assert.Single(goals);
    }
}
