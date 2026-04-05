# GS4 Planner – Training Cost & Cap Data Model

## Overview

This document describes the database schema used by the GS4 Planner library to
store profession training costs and caps for GemStone IV.

---

## Entity Relationship

```
Profession (1) ──< (n) TrainingCost (n) >── (1) Skill (n) >── (1) SkillCategory
Profession (1) ──< (n) TrainingCap
                         ├── (n) >── (1) SkillCategory  (Lore / Spell Research caps)
                         └── (n) >── (1) Skill           (Standard skill caps)
```

---

## Data Models

### Profession

All 10 GemStone IV professions are seeded:

| Id | Name     |
|----|----------|
| 1  | Warrior  |
| 2  | Rogue    |
| 3  | Monk     |
| 4  | Ranger   |
| 5  | Bard     |
| 6  | Paladin  |
| 7  | Cleric   |
| 8  | Empath   |
| 9  | Wizard   |
| 10 | Sorcerer |

---

### SkillCategory

Groups skills into six types:

| Id | Name          | Type            |
|----|---------------|-----------------|
| 1  | Standard      | Standard        |
| 2  | Spell Research| SpellResearch   |
| 3  | Elemental Lore| ElementalLore   |
| 4  | Spiritual Lore| SpiritualLore   |
| 5  | Mental Lore   | MentalLore      |
| 6  | Sorcerous Lore| SorcerousLore   |

---

### Skill

Each skill belongs to a `SkillCategory`.

#### Standard Skills (Category: Standard)

| Name               |
|--------------------|
| Armor Use          |
| Shield Use         |
| Combat Maneuvers   |
| Edged Weapons      |
| Blunt Weapons      |
| Two-Handed Weapons |
| Polearm Weapons    |
| Ranged Weapons     |
| Thrown Weapons     |
| Brawling           |
| Two Weapon Combat  |

#### Spell Research Circles (Category: Spell Research)

Each caster profession has access to specific circles:

| Circle Name      | Used By                             |
|------------------|-------------------------------------|
| Cleric Base      | Cleric                              |
| Minor Spiritual  | Cleric, Empath, Ranger, Paladin, Sorcerer |
| Major Spiritual  | Cleric, Empath                      |
| Empath Base      | Empath                              |
| Ranger Base      | Ranger                              |
| Bard Base        | Bard                                |
| Paladin Base     | Paladin                             |
| Minor Elemental  | Wizard, Bard, Sorcerer              |
| Major Elemental  | Wizard                              |
| Wizard Base      | Wizard                              |
| Sorcerer Base    | Sorcerer                            |

#### Lore Skills

**Elemental Lore** (Category: ElementalLore)
- Elemental Lore, Air
- Elemental Lore, Earth
- Elemental Lore, Fire
- Elemental Lore, Water

**Spiritual Lore** (Category: SpiritualLore)
- Spiritual Lore, Blessings
- Spiritual Lore, Religion
- Spiritual Lore, Summoning

**Mental Lore** (Category: MentalLore)
- Mental Lore, Divination
- Mental Lore, Manipulation
- Mental Lore, Telepathy
- Mental Lore, Transference
- Mental Lore, Transformation

**Sorcerous Lore** (Category: SorcerousLore)
- Sorcerous Lore, Demonology
- Sorcerous Lore, Necromancy

---

### TrainingCost

Maps `Profession × Skill → PhysicalCost / MentalCost` (cost **per rank**).

- Costs are stored once per skill per profession.
- All spell research circles for a given profession share the same cost.
- All skills within a lore category for a given profession share the same cost.
- A `Unique(ProfessionId, SkillId)` constraint prevents duplicate entries.

**Seeded sample (Warrior)**

| Skill            | Physical | Mental |
|------------------|----------|--------|
| Armor Use        | 2        | 0      |
| Shield Use       | 2        | 0      |
| Combat Maneuvers | 4        | 3      |
| Edged Weapons    | 2        | 1      |
| Blunt Weapons    | 2        | 1      |
| ...              | ...      | ...    |
| Elemental Lore,* | 0        | 15     |
| Spiritual Lore,* | 0        | 15     |
| Mental Lore,*    | 0        | 40     |
| Sorcerous Lore,* | 0        | 30     |

**Seeded sample (Cleric)**

| Skill                  | Physical | Mental |
|------------------------|----------|--------|
| Armor Use              | 8        | 0      |
| Cleric Base            | 0        | 8      |
| Minor Spiritual        | 0        | 8      |
| Major Spiritual        | 0        | 8      |
| Spiritual Lore, *      | 0        | 6      |
| Elemental Lore, *      | 0        | 15     |

**Seeded sample (Wizard)**

| Skill                  | Physical | Mental |
|------------------------|----------|--------|
| Armor Use              | 14       | 0      |
| Minor Elemental        | 0        | 8      |
| Major Elemental        | 0        | 8      |
| Wizard Base            | 0        | 8      |
| Elemental Lore, *      | 0        | 6      |

---

### TrainingCap

Stores the maximum ranks that may be trained **per level** for a profession in
a given skill or category. Caps are **cumulative**: a cap of `N` per level
means the total maximum at level `L` is `N × L` (level 0 inclusive, so a cap
of 3 allows 3 ranks before first level advancement, then 3 more per level).

`TrainingCap` has two mutually exclusive foreign keys:

| Field              | Used for                                    |
|--------------------|---------------------------------------------|
| `SkillId`          | Standard skills – cap on individual skill   |
| `SkillCategoryId`  | Spell Research / Lore – combined category cap |

#### Standard skill caps (individual)

A cap entry with `SkillId` set means the profession may train at most
`MaxRanksPerLevel` ranks in **that specific skill** each level.

Example – Warrior, Armor Use:

```
MaxRanksPerLevel = 3
Cumulative max at level 10 = 3 × 10 = 30 ranks
```

#### Spell Research caps (category-level)

A cap entry with `SkillCategoryId = SpellResearch` means the profession may
train at most `MaxRanksPerLevel` ranks **total across all accessible spell
circles** each level.

Example – Cleric, Spell Research (Cleric Base + Minor Spiritual + Major Spiritual):

```
MaxRanksPerLevel = 3
At level 10, up to 30 ranks total in any combination of the three circles.
```

#### Lore caps (category-level)

Works identically to spell research caps, but scoped to a single lore category.

Example – Cleric, Spiritual Lore (Blessings + Religion + Summoning):

```
MaxRanksPerLevel = 2
At level 10, up to 20 ranks total in any combination of the three spiritual lores.
```

**Seeded caps (Warrior)**

| Scope    | Skill/Category    | Max/Level |
|----------|-------------------|-----------|
| Skill    | Armor Use         | 3         |
| Skill    | Shield Use        | 3         |
| Skill    | Combat Maneuvers  | 2         |
| Skill    | Edged Weapons     | 2         |
| Skill    | (other weapons)   | 2         |
| Category | Elemental Lore    | 1         |
| Category | Spiritual Lore    | 1         |
| Category | Mental Lore       | 1         |
| Category | Sorcerous Lore    | 1         |

**Seeded caps (Cleric)**

| Scope    | Skill/Category    | Max/Level |
|----------|-------------------|-----------|
| Skill    | Armor Use         | 1         |
| Skill    | (other standard)  | 1         |
| Category | Spell Research    | 3         |
| Category | Spiritual Lore    | 2         |
| Category | Elemental Lore    | 1         |
| Category | Mental Lore       | 2         |
| Category | Sorcerous Lore    | 1         |

**Seeded caps (Wizard)**

| Scope    | Skill/Category    | Max/Level |
|----------|-------------------|-----------|
| Skill    | Armor Use         | 1         |
| Skill    | (other standard)  | 1         |
| Category | Spell Research    | 3         |
| Category | Elemental Lore    | 3         |
| Category | Spiritual Lore    | 1         |
| Category | Mental Lore       | 2         |
| Category | Sorcerous Lore    | 1         |

---

## Querying the Data

### Dependency Injection Setup

#### SQLite (production)

```csharp
services.AddGS4PlannerSqlite("Data Source=gs4planner.db");
```

#### In-memory (testing)

```csharp
services.AddGS4PlannerData(options =>
    options.UseInMemoryDatabase("TestDb"));
```

### `ITrainingRepository` Methods

```csharp
// All 10 professions
IReadOnlyList<Profession> professions = await repo.GetAllProfessionsAsync();

// Profession by name (case-insensitive)
Profession? cleric = await repo.GetProfessionByNameAsync("Cleric");

// All skills in a category type
IReadOnlyList<Skill> spellCircles =
    await repo.GetSkillsByCategoryTypeAsync(SkillCategoryType.SpellResearch);

// All training costs for a profession
IReadOnlyList<TrainingCost> costs =
    await repo.GetTrainingCostsByProfessionAsync(cleric.Id);

// Cost for a specific profession + skill
TrainingCost? armorCost =
    await repo.GetTrainingCostAsync(cleric.Id, armorUseSkillId);
// armorCost.PhysicalCost == 8, armorCost.MentalCost == 0

// Spell Research / Lore category cap
TrainingCap? spellCap =
    await repo.GetCategoryCapAsync(cleric.Id, spellResearchCategoryId);
// spellCap.MaxRanksPerLevel == 3

// Standard skill cap
TrainingCap? armorCap =
    await repo.GetSkillCapAsync(warrior.Id, armorUseSkillId);
// armorCap.MaxRanksPerLevel == 3
```

### Cumulative Cap Calculation

```csharp
// "How many total spell research ranks can a Cleric have by level 10?"
int maxRanksAtLevel10 = spellCap.MaxRanksPerLevel * level; // 3 × 10 = 30

// The formula works identically for all cap types (skill or category).
```

---

## Extending the Seed Data

To add training data for more professions, follow the pattern in
`GS4PlannerDbContext`:

1. Add a `SeedXxxData(ModelBuilder)` private static method following the
   existing pattern (Warrior / Cleric / Wizard examples).
2. Reserve a cost ID block in `SeedIdRanges` (each profession gets 100 IDs).
3. Reserve a cap ID block in `SeedIdRanges`.
4. Call your new method from `OnModelCreating`.
5. Generate and apply a new EF Core migration:

```bash
dotnet ef migrations add AddXxxTrainingData --project src/GS4PlannerLib.Data
dotnet ef database update               --project src/GS4PlannerLib.Data
```

> **Note:** Training cost values in this initial seed are based on published
> GemStone IV wiki data. Always verify against
> [gswiki.play.net/Category:Professions](https://gswiki.play.net/Category:Professions)
> before using for game-accurate calculations.
