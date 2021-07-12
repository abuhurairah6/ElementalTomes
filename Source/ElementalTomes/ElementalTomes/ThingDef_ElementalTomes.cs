using RimWorld;
using Verse;

namespace ElementalTomes
{
    public class Verb_CastMagic : Verb_Shoot
    {
        #region Overrides
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Pawn pawn = this.caster as Pawn;
            HediffDef fatigueDef = DefDatabase<HediffDef>.GetNamed("ET_Fatigue");
            var fatigueOnPawn = pawn?.health?.hediffSet?.GetFirstHediffOfDef(fatigueDef);
            Log.Message("Running warmupcomplete");
            var randomSeverity = Rand.Range(0.01f, 0.02f);
            if (fatigueOnPawn != null)
            {
                fatigueOnPawn.Severity += randomSeverity;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(fatigueDef, pawn, null);
                hediff.Severity = randomSeverity;
                pawn.health.AddHediff(hediff, null, null);
            }
        }
        #endregion Overrides
    }

    public class Projectile_ElementalTomes : Projectile_Explosive
    {
        #region Properties
        public ThingDef Def
        {
            get
            {
                return this.def as ThingDef;
            }
        }
        #endregion Properties

        #region Override
        protected override void Explode()
        {
            IntVec3 strikeLoc = base.Position;
            this.Destroy(DestroyMode.Vanish);
            //Log.Message("Exploded: Call lightning");
            Find.CurrentMap.weatherManager.eventHandler.AddEvent(new ElementalEvent_LightningStrike(Find.CurrentMap, strikeLoc, this.def, this.launcher));
        }
        #endregion Override
    }
}
