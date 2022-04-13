using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CompPawnGrower;

public class CompProperties_PawnGrower : CompProperties
{
    public bool canspawn = true;

    public List<PawnGenOption> Pawnkinds = new List<PawnGenOption>();

    public List<RaceGenOption> Races = new List<RaceGenOption>();

    public float spawnChance = 1f;

    public bool spawnwild = true;

    public CompProperties_PawnGrower()
    {
        compClass = typeof(Comp_PawnGrower);
    }
}