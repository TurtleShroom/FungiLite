using System;
using System.Linq;
using RimWorld;
using Verse;

namespace CompPawnGrower;

public class Comp_PawnGrower : ThingComp
{
    private float age;

    public Faction faction;

    private float fertility;

    public PawnGenerationContext generationContext;

    public PawnKindDef pawnKindDef;

    public CompProperties_PawnGrower Props => (CompProperties_PawnGrower)props;

    public Plant plant => parent as Plant;

    public bool canspawn => plant.HarvestableNow && Props.canspawn;

    public bool spawnwild => Props.spawnwild;

    public float spawnChance => Props.spawnChance;

    public float Age
    {
        get
        {
            if (plant != null)
            {
                age = plant.Age;
            }

            return age;
        }
    }

    public float Fertility
    {
        get
        {
            if (plant is { Map: { } })
            {
                fertility = plant.GrowthRateFactor_Fertility;
            }

            return fertility;
        }
    }

    public override void PostDeSpawn(Map map)
    {
        if (canspawn)
        {
            var value = Rand.Value;
            if (value < spawnChance * plant.Growth)
            {
                if (Props.Pawnkinds.NullOrEmpty())
                {
                    if (Props.Races.NullOrEmpty())
                    {
                        Log.Error($"Comp_PawnGrower: No race or kinddefs set in {parent.def.defName}");
                        return;
                    }

                    var race = Props.Races.RandomElementByWeight(x => x.selectionWeight).thingdef;
                    try
                    {
                        pawnKindDef = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.race == race).RandomElement();
                    }
                    catch (Exception)
                    {
                        Log.Error($"Comp_PawnGrower: No kinddefs found for {race.defName}");
                        return;
                    }
                }
                else
                {
                    pawnKindDef = Props.Pawnkinds.RandomElementByWeight(x => x.selectionWeight).kind;
                }

                faction = spawnwild ? null : Faction.OfPlayer;
                generationContext = PawnGenerationContext.NonPlayer;
                var pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
                if (pawnKindDef.RaceProps.Humanlike)
                {
                    if (!spawnwild && Faction.OfPlayer.def.basicMemberKind != null)
                    {
                        pawn.ChangeKind(Faction.OfPlayer.def.basicMemberKind);
                    }
                    else
                    {
                        pawn.ChangeKind(PawnKindDefOf.WildMan);
                    }
                }

                if (Fertility < 1f)
                {
                    foreach (var allNeed in pawn.needs.AllNeeds)
                    {
                        allNeed.CurLevel = 0f;
                    }

                    var hediff = HediffMaker.MakeHediff(HediffDefOf.Malnutrition, pawn);
                    hediff.Severity = Math.Min(1f - Fertility, 0.75f);
                    pawn.health.AddHediff(hediff);
                }
                else
                {
                    foreach (var allNeed2 in pawn.needs.AllNeeds)
                    {
                        allNeed2.CurLevel = Fertility - 1f;
                    }
                }

                GenSpawn.Spawn(pawn, parent.Position, map);
            }
        }

        base.PostDeSpawn(map);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref age, "PlantAge");
        Scribe_Values.Look(ref fertility, "PlantFertility");
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (age == 0f)
        {
            age = Age;
        }

        if (fertility == 0f)
        {
            fertility = Fertility;
        }
    }
}