using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data.Context;

public class GS4PlannerDbContext : DbContext
{
    public GS4PlannerDbContext(DbContextOptions<GS4PlannerDbContext> options)
        : base(options) { }

    public DbSet<Character> Characters => Set<Character>();
    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
    public DbSet<TrainingGoal> TrainingGoals => Set<TrainingGoal>();
    public DbSet<Profession> Professions => Set<Profession>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<TrainingCost> TrainingCosts => Set<TrainingCost>();
    public DbSet<TrainingCap> TrainingCaps => Set<TrainingCap>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCharacter(modelBuilder);
        ConfigureTrainingPlan(modelBuilder);
        ConfigureTrainingGoal(modelBuilder);
        ConfigureProfession(modelBuilder);
        ConfigureSkillCategory(modelBuilder);
        ConfigureSkill(modelBuilder);
        ConfigureTrainingCost(modelBuilder);
        ConfigureTrainingCap(modelBuilder);

        SeedProfessions(modelBuilder);
        SeedSkillCategories(modelBuilder);
        SeedSkills(modelBuilder);
        SeedWarriorData(modelBuilder);
        SeedClericData(modelBuilder);
        SeedWizardData(modelBuilder);
    }

    // -------------------------------------------------------------------------
    // Model Configuration
    // -------------------------------------------------------------------------

    private static void ConfigureCharacter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Race).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Profession).IsRequired().HasMaxLength(50);
        });
    }

    private static void ConfigureTrainingPlan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasOne(e => e.Character)
                  .WithMany(c => c.TrainingPlans)
                  .HasForeignKey(e => e.CharacterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTrainingGoal(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingGoal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SkillName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasOne(e => e.TrainingPlan)
                  .WithMany(p => p.TrainingGoals)
                  .HasForeignKey(e => e.TrainingPlanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProfession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profession>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(64);
            entity.HasIndex(p => p.Name).IsUnique();
        });
    }

    private static void ConfigureSkillCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SkillCategory>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.Name).IsRequired().HasMaxLength(64);
            entity.Property(sc => sc.Type).IsRequired();
        });
    }

    private static void ConfigureSkill(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(128);
            entity.Property(s => s.Description).HasMaxLength(512);

            entity.HasOne(s => s.SkillCategory)
                  .WithMany(sc => sc.Skills)
                  .HasForeignKey(s => s.SkillCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTrainingCost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingCost>(entity =>
        {
            entity.HasKey(tc => tc.Id);

            entity.HasIndex(tc => new { tc.ProfessionId, tc.SkillId }).IsUnique();

            entity.Property(tc => tc.PhysicalCost).IsRequired();
            entity.Property(tc => tc.MentalCost).IsRequired();

            entity.HasOne(tc => tc.Profession)
                  .WithMany(p => p.TrainingCosts)
                  .HasForeignKey(tc => tc.ProfessionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tc => tc.Skill)
                  .WithMany(s => s.TrainingCosts)
                  .HasForeignKey(tc => tc.SkillId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTrainingCap(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingCap>(entity =>
        {
            entity.HasKey(tc => tc.Id);

            // Unique cap per profession × skill category (for lore/spell research caps)
            entity.HasIndex(tc => new { tc.ProfessionId, tc.SkillCategoryId })
                  .IsUnique()
                  .HasFilter("[SkillCategoryId] IS NOT NULL");

            // Unique cap per profession × individual skill (for standard skill caps)
            entity.HasIndex(tc => new { tc.ProfessionId, tc.SkillId })
                  .IsUnique()
                  .HasFilter("[SkillId] IS NOT NULL");

            entity.Property(tc => tc.MaxRanksPerLevel).IsRequired();

            entity.HasOne(tc => tc.Profession)
                  .WithMany(p => p.TrainingCaps)
                  .HasForeignKey(tc => tc.ProfessionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tc => tc.SkillCategory)
                  .WithMany(sc => sc.TrainingCaps)
                  .HasForeignKey(tc => tc.SkillCategoryId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tc => tc.Skill)
                  .WithMany(s => s.TrainingCaps)
                  .HasForeignKey(tc => tc.SkillId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // -------------------------------------------------------------------------
    // Seed Data – Professions
    // -------------------------------------------------------------------------

    private static void SeedProfessions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profession>().HasData(
            new Profession { Id = ProfessionIds.Warrior,  Name = "Warrior"  },
            new Profession { Id = ProfessionIds.Rogue,    Name = "Rogue"    },
            new Profession { Id = ProfessionIds.Monk,     Name = "Monk"     },
            new Profession { Id = ProfessionIds.Ranger,   Name = "Ranger"   },
            new Profession { Id = ProfessionIds.Bard,     Name = "Bard"     },
            new Profession { Id = ProfessionIds.Paladin,  Name = "Paladin"  },
            new Profession { Id = ProfessionIds.Cleric,   Name = "Cleric"   },
            new Profession { Id = ProfessionIds.Empath,   Name = "Empath"   },
            new Profession { Id = ProfessionIds.Wizard,   Name = "Wizard"   },
            new Profession { Id = ProfessionIds.Sorcerer, Name = "Sorcerer" }
        );
    }

    // -------------------------------------------------------------------------
    // Seed Data – Skill Categories
    // -------------------------------------------------------------------------

    private static void SeedSkillCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SkillCategory>().HasData(
            new SkillCategory { Id = CategoryIds.Standard,      Name = "Standard",      Type = SkillCategoryType.Standard      },
            new SkillCategory { Id = CategoryIds.SpellResearch, Name = "Spell Research",Type = SkillCategoryType.SpellResearch  },
            new SkillCategory { Id = CategoryIds.ElementalLore, Name = "Elemental Lore",Type = SkillCategoryType.ElementalLore  },
            new SkillCategory { Id = CategoryIds.SpiritualLore, Name = "Spiritual Lore",Type = SkillCategoryType.SpiritualLore  },
            new SkillCategory { Id = CategoryIds.MentalLore,    Name = "Mental Lore",   Type = SkillCategoryType.MentalLore    },
            new SkillCategory { Id = CategoryIds.SorcerousLore, Name = "Sorcerous Lore",Type = SkillCategoryType.SorcerousLore }
        );
    }

    // -------------------------------------------------------------------------
    // Seed Data – Skills
    // -------------------------------------------------------------------------

    private static void SeedSkills(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasData(
            // Standard Skills
            new Skill { Id = SkillIds.ArmorUse,           SkillCategoryId = CategoryIds.Standard,      Name = "Armor Use"                           },
            new Skill { Id = SkillIds.ShieldUse,          SkillCategoryId = CategoryIds.Standard,      Name = "Shield Use"                          },
            new Skill { Id = SkillIds.CombatManeuvers,    SkillCategoryId = CategoryIds.Standard,      Name = "Combat Maneuvers"                    },
            new Skill { Id = SkillIds.EdgedWeapons,       SkillCategoryId = CategoryIds.Standard,      Name = "Edged Weapons"                       },
            new Skill { Id = SkillIds.BluntWeapons,       SkillCategoryId = CategoryIds.Standard,      Name = "Blunt Weapons"                       },
            new Skill { Id = SkillIds.TwoHandedWeapons,   SkillCategoryId = CategoryIds.Standard,      Name = "Two-Handed Weapons"                  },
            new Skill { Id = SkillIds.PolearmWeapons,     SkillCategoryId = CategoryIds.Standard,      Name = "Polearm Weapons"                     },
            new Skill { Id = SkillIds.RangedWeapons,      SkillCategoryId = CategoryIds.Standard,      Name = "Ranged Weapons"                      },
            new Skill { Id = SkillIds.ThrownWeapons,      SkillCategoryId = CategoryIds.Standard,      Name = "Thrown Weapons"                      },
            new Skill { Id = SkillIds.Brawling,           SkillCategoryId = CategoryIds.Standard,      Name = "Brawling"                            },
            new Skill { Id = SkillIds.TwoWeaponCombat,   SkillCategoryId = CategoryIds.Standard,      Name = "Two Weapon Combat"                   },

            // Spell Research Circles – Cleric
            new Skill { Id = SkillIds.ClericBase,         SkillCategoryId = CategoryIds.SpellResearch, Name = "Cleric Base",         Description = "Cleric spell circle (100s)"          },
            new Skill { Id = SkillIds.MinorSpiritualBase, SkillCategoryId = CategoryIds.SpellResearch, Name = "Minor Spiritual",     Description = "Minor Spiritual spell circle (900s)"  },
            new Skill { Id = SkillIds.MajorSpiritualBase, SkillCategoryId = CategoryIds.SpellResearch, Name = "Major Spiritual",     Description = "Major Spiritual spell circle (600s)"  },

            // Spell Research Circles – Empath
            new Skill { Id = SkillIds.EmpathBase,         SkillCategoryId = CategoryIds.SpellResearch, Name = "Empath Base",         Description = "Empath spell circle (1100s)"          },

            // Spell Research Circles – Ranger
            new Skill { Id = SkillIds.RangerBase,         SkillCategoryId = CategoryIds.SpellResearch, Name = "Ranger Base",         Description = "Ranger spell circle (600s)"           },

            // Spell Research Circles – Bard
            new Skill { Id = SkillIds.BardBase,           SkillCategoryId = CategoryIds.SpellResearch, Name = "Bard Base",           Description = "Bard spell circle (1000s)"            },

            // Spell Research Circles – Paladin
            new Skill { Id = SkillIds.PaladinBase,        SkillCategoryId = CategoryIds.SpellResearch, Name = "Paladin Base",        Description = "Paladin spell circle (1600s)"         },

            // Spell Research Circles – Wizard
            new Skill { Id = SkillIds.MinorElementalBase, SkillCategoryId = CategoryIds.SpellResearch, Name = "Minor Elemental",     Description = "Minor Elemental spell circle (900s)"  },
            new Skill { Id = SkillIds.MajorElementalBase, SkillCategoryId = CategoryIds.SpellResearch, Name = "Major Elemental",     Description = "Major Elemental spell circle (500s)"  },
            new Skill { Id = SkillIds.WizardBase,         SkillCategoryId = CategoryIds.SpellResearch, Name = "Wizard Base",         Description = "Wizard spell circle (400s)"           },

            // Spell Research Circles – Sorcerer
            new Skill { Id = SkillIds.SorcererBase,       SkillCategoryId = CategoryIds.SpellResearch, Name = "Sorcerer Base",       Description = "Sorcerer spell circle (700s)"         },

            // Elemental Lore
            new Skill { Id = SkillIds.ElementalLoreAir,   SkillCategoryId = CategoryIds.ElementalLore, Name = "Elemental Lore, Air"                 },
            new Skill { Id = SkillIds.ElementalLoreEarth, SkillCategoryId = CategoryIds.ElementalLore, Name = "Elemental Lore, Earth"               },
            new Skill { Id = SkillIds.ElementalLoreFire,  SkillCategoryId = CategoryIds.ElementalLore, Name = "Elemental Lore, Fire"                },
            new Skill { Id = SkillIds.ElementalLoreWater, SkillCategoryId = CategoryIds.ElementalLore, Name = "Elemental Lore, Water"               },

            // Spiritual Lore
            new Skill { Id = SkillIds.SpiritualLoreBlessings,  SkillCategoryId = CategoryIds.SpiritualLore, Name = "Spiritual Lore, Blessings"      },
            new Skill { Id = SkillIds.SpiritualLoreReligion,   SkillCategoryId = CategoryIds.SpiritualLore, Name = "Spiritual Lore, Religion"       },
            new Skill { Id = SkillIds.SpiritualLoreSummoning,  SkillCategoryId = CategoryIds.SpiritualLore, Name = "Spiritual Lore, Summoning"      },

            // Mental Lore
            new Skill { Id = SkillIds.MentalLoreDivination,    SkillCategoryId = CategoryIds.MentalLore,    Name = "Mental Lore, Divination"        },
            new Skill { Id = SkillIds.MentalLoreManipulation,  SkillCategoryId = CategoryIds.MentalLore,    Name = "Mental Lore, Manipulation"      },
            new Skill { Id = SkillIds.MentalLoreTelepathy,     SkillCategoryId = CategoryIds.MentalLore,    Name = "Mental Lore, Telepathy"         },
            new Skill { Id = SkillIds.MentalLoreTransference,  SkillCategoryId = CategoryIds.MentalLore,    Name = "Mental Lore, Transference"      },
            new Skill { Id = SkillIds.MentalLoreTransformation,SkillCategoryId = CategoryIds.MentalLore,    Name = "Mental Lore, Transformation"    },

            // Sorcerous Lore
            new Skill { Id = SkillIds.SorcerousLoreDemonology, SkillCategoryId = CategoryIds.SorcerousLore, Name = "Sorcerous Lore, Demonology"     },
            new Skill { Id = SkillIds.SorcerousLoreNecromancy, SkillCategoryId = CategoryIds.SorcerousLore, Name = "Sorcerous Lore, Necromancy"     }
        );
    }

    // -------------------------------------------------------------------------
    // Seed Data – Warrior
    // -------------------------------------------------------------------------

    private static void SeedWarriorData(ModelBuilder modelBuilder)
    {
        const int p = ProfessionIds.Warrior;
        int id = SeedIdRanges.WarriorCostBase;

        // Training Costs
        modelBuilder.Entity<TrainingCost>().HasData(
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,           PhysicalCost = 2,  MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,          PhysicalCost = 2,  MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,    PhysicalCost = 4,  MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,       PhysicalCost = 2,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,       PhysicalCost = 2,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,   PhysicalCost = 3,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,     PhysicalCost = 3,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,      PhysicalCost = 2,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,      PhysicalCost = 2,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,           PhysicalCost = 2,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,    PhysicalCost = 6,  MentalCost = 0  },
            // Lore skills
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreAir,          PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreEarth,         PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreFire,          PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreWater,         PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreBlessings,     PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreReligion,      PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreSummoning,     PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreDivination,       PhysicalCost = 0, MentalCost = 40 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreManipulation,     PhysicalCost = 0, MentalCost = 40 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTelepathy,        PhysicalCost = 0, MentalCost = 40 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransference,     PhysicalCost = 0, MentalCost = 40 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransformation,   PhysicalCost = 0, MentalCost = 40 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreDemonology,    PhysicalCost = 0, MentalCost = 30 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreNecromancy,    PhysicalCost = 0, MentalCost = 30 }
        );

        id = SeedIdRanges.WarriorCapBase;

        // Training Caps – Standard skills (caps on individual skills)
        modelBuilder.Entity<TrainingCap>().HasData(
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,         MaxRanksPerLevel = 3 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,         MaxRanksPerLevel = 3 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,   MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,      MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,      MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,  MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,    MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,     MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,     MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,          MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,   MaxRanksPerLevel = 2 },
            // Lore caps (category-level)
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.ElementalLore, MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SpiritualLore, MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.MentalLore,    MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SorcerousLore, MaxRanksPerLevel = 1 }
        );
    }

    // -------------------------------------------------------------------------
    // Seed Data – Cleric
    // -------------------------------------------------------------------------

    private static void SeedClericData(ModelBuilder modelBuilder)
    {
        const int p = ProfessionIds.Cleric;
        int id = SeedIdRanges.ClericCostBase;

        // Training Costs – Standard skills
        modelBuilder.Entity<TrainingCost>().HasData(
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,           PhysicalCost = 8,  MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,          PhysicalCost = 4,  MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,    PhysicalCost = 12, MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,       PhysicalCost = 6,  MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,       PhysicalCost = 6,  MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,   PhysicalCost = 10, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,     PhysicalCost = 10, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,      PhysicalCost = 10, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,      PhysicalCost = 8,  MentalCost = 2  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,           PhysicalCost = 8,  MentalCost = 2  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,    PhysicalCost = 15, MentalCost = 10 },
            // Spell Research circles – all cost the same for Cleric
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ClericBase,         PhysicalCost = 0,  MentalCost = 8  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MinorSpiritualBase, PhysicalCost = 0,  MentalCost = 8  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MajorSpiritualBase, PhysicalCost = 0,  MentalCost = 8  },
            // Lore skills
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreAir,          PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreEarth,         PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreFire,          PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreWater,         PhysicalCost = 0, MentalCost = 15 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreBlessings,     PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreReligion,      PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreSummoning,     PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreDivination,       PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreManipulation,     PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTelepathy,        PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransference,     PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransformation,   PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreDemonology,    PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreNecromancy,    PhysicalCost = 0, MentalCost = 20 }
        );

        id = SeedIdRanges.ClericCapBase;

        // Training Caps
        modelBuilder.Entity<TrainingCap>().HasData(
            // Standard skills – individual caps
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,         MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,         MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,   MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,      MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,      MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,  MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,    MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,     MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,     MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,          MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,   MaxRanksPerLevel = 1 },
            // Spell Research – category cap (Cleric Base + Minor Spiritual + Major Spiritual combined)
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SpellResearch, MaxRanksPerLevel = 3 },
            // Lore caps (category-level)
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.ElementalLore, MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SpiritualLore, MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.MentalLore,    MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SorcerousLore, MaxRanksPerLevel = 1 }
        );
    }

    // -------------------------------------------------------------------------
    // Seed Data – Wizard
    // -------------------------------------------------------------------------

    private static void SeedWizardData(ModelBuilder modelBuilder)
    {
        const int p = ProfessionIds.Wizard;
        int id = SeedIdRanges.WizardCostBase;

        // Training Costs – Standard skills
        modelBuilder.Entity<TrainingCost>().HasData(
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,           PhysicalCost = 14, MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,          PhysicalCost = 13, MentalCost = 0  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,    PhysicalCost = 12, MentalCost = 8  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,       PhysicalCost = 6,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,       PhysicalCost = 6,  MentalCost = 1  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,   PhysicalCost = 14, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,     PhysicalCost = 14, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,      PhysicalCost = 14, MentalCost = 3  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,      PhysicalCost = 8,  MentalCost = 2  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,           PhysicalCost = 10, MentalCost = 2  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,    PhysicalCost = 12, MentalCost = 12 },
            // Spell Research circles – all cost the same for Wizard
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MinorElementalBase, PhysicalCost = 0,  MentalCost = 8  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MajorElementalBase, PhysicalCost = 0,  MentalCost = 8  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.WizardBase,         PhysicalCost = 0,  MentalCost = 8  },
            // Lore skills
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreAir,          PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreEarth,         PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreFire,          PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.ElementalLoreWater,         PhysicalCost = 0, MentalCost = 6  },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreBlessings,     PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreReligion,      PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SpiritualLoreSummoning,     PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreDivination,       PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreManipulation,     PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTelepathy,        PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransference,     PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.MentalLoreTransformation,   PhysicalCost = 0, MentalCost = 20 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreDemonology,    PhysicalCost = 0, MentalCost = 10 },
            new TrainingCost { Id = id++, ProfessionId = p, SkillId = SkillIds.SorcerousLoreNecromancy,    PhysicalCost = 0, MentalCost = 10 }
        );

        id = SeedIdRanges.WizardCapBase;

        // Training Caps
        modelBuilder.Entity<TrainingCap>().HasData(
            // Standard skills – individual caps
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ArmorUse,         MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ShieldUse,         MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.CombatManeuvers,   MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.EdgedWeapons,      MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.BluntWeapons,      MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoHandedWeapons,  MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.PolearmWeapons,    MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.RangedWeapons,     MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.ThrownWeapons,     MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.Brawling,          MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillId = SkillIds.TwoWeaponCombat,   MaxRanksPerLevel = 1 },
            // Spell Research – category cap (Minor Elemental + Major Elemental + Wizard Base combined)
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SpellResearch, MaxRanksPerLevel = 3 },
            // Lore caps (category-level)
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.ElementalLore, MaxRanksPerLevel = 3 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SpiritualLore, MaxRanksPerLevel = 1 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.MentalLore,    MaxRanksPerLevel = 2 },
            new TrainingCap { Id = id++, ProfessionId = p, SkillCategoryId = CategoryIds.SorcerousLore, MaxRanksPerLevel = 1 }
        );
    }

    // -------------------------------------------------------------------------
    // Well-known ID constants (internal – used by seed methods and tests)
    // -------------------------------------------------------------------------

    internal static class ProfessionIds
    {
        public const int Warrior  = 1;
        public const int Rogue    = 2;
        public const int Monk     = 3;
        public const int Ranger   = 4;
        public const int Bard     = 5;
        public const int Paladin  = 6;
        public const int Cleric   = 7;
        public const int Empath   = 8;
        public const int Wizard   = 9;
        public const int Sorcerer = 10;
    }

    internal static class CategoryIds
    {
        public const int Standard      = 1;
        public const int SpellResearch = 2;
        public const int ElementalLore = 3;
        public const int SpiritualLore = 4;
        public const int MentalLore    = 5;
        public const int SorcerousLore = 6;
    }

    internal static class SkillIds
    {
        // Standard
        public const int ArmorUse         = 1;
        public const int ShieldUse        = 2;
        public const int CombatManeuvers  = 3;
        public const int EdgedWeapons     = 4;
        public const int BluntWeapons     = 5;
        public const int TwoHandedWeapons = 6;
        public const int PolearmWeapons   = 7;
        public const int RangedWeapons    = 8;
        public const int ThrownWeapons    = 9;
        public const int Brawling         = 10;
        public const int TwoWeaponCombat  = 11;
        // Spell Research
        public const int ClericBase         = 12;
        public const int MinorSpiritualBase = 13;
        public const int MajorSpiritualBase = 14;
        public const int EmpathBase         = 15;
        public const int RangerBase         = 16;
        public const int BardBase           = 17;
        public const int PaladinBase        = 18;
        public const int MinorElementalBase = 19;
        public const int MajorElementalBase = 20;
        public const int WizardBase         = 21;
        public const int SorcererBase       = 22;
        // Elemental Lore
        public const int ElementalLoreAir   = 23;
        public const int ElementalLoreEarth = 24;
        public const int ElementalLoreFire  = 25;
        public const int ElementalLoreWater = 26;
        // Spiritual Lore
        public const int SpiritualLoreBlessings = 27;
        public const int SpiritualLoreReligion  = 28;
        public const int SpiritualLoreSummoning = 29;
        // Mental Lore
        public const int MentalLoreDivination    = 30;
        public const int MentalLoreManipulation  = 31;
        public const int MentalLoreTelepathy     = 32;
        public const int MentalLoreTransference  = 33;
        public const int MentalLoreTransformation= 34;
        // Sorcerous Lore
        public const int SorcerousLoreDemonology = 35;
        public const int SorcerousLoreNecromancy = 36;
    }

    /// <summary>
    /// Reserved ID ranges for seeded TrainingCost and TrainingCap rows.
    /// Each profession gets a block of 100 IDs to allow future skill additions
    /// without renumbering existing rows.
    /// </summary>
    internal static class SeedIdRanges
    {
        public const int WarriorCostBase  = 1001;
        public const int ClericCostBase   = 1101;
        public const int WizardCostBase   = 1201;

        public const int WarriorCapBase   = 2001;
        public const int ClericCapBase    = 2101;
        public const int WizardCapBase    = 2201;
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<TrainingPlan>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
