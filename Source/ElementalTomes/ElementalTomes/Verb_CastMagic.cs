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
            if (pawn.skills != null)
            {
                SkillDef magicDef = DefDatabase<SkillDef>.GetNamed("ET_Magic");
                pawn.skills.Learn(magicDef, 100f * this.verbProps.AdjustedFullCycleTime(this, pawn), false);
            }
        }

        public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false)
        {
            if (this.caster == null)
            {
                Log.Error("Verb " + this.GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).");
                return false;
            }
            if (!this.caster.Spawned)
            {
                return false;
            }
            if (this.state == VerbState.Bursting || !this.CanHitTarget(castTarg))
            {
                return false;
            }
            if (this.CausesTimeSlowdown(castTarg))
            {
                Find.TickManager.slower.SignalForceNormalSpeed();
            }
            this.surpriseAttack = surpriseAttack;
            this.canHitNonTargetPawnsNow = canHitNonTargetPawns;
            this.preventFriendlyFire = preventFriendlyFire;
            this.currentTarget = castTarg;
            this.currentDestination = destTarg;
            if (this.CasterIsPawn && this.verbProps.warmupTime > 0f)
            {
                ShootLine newShootLine;
                if (!this.TryFindShootLineFromTo(this.caster.Position, castTarg, out newShootLine))
                {
                    return false;
                }
                this.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, this.caster.Position);
                float statValue = this.CasterPawn.GetStatValue(DefDatabase<StatDef>.GetNamed("ET_ChannellingDelayFactor"), true);
                Log.Message("Channeling stat value is" + statValue.ToString());
                int ticks = (this.verbProps.warmupTime * statValue).SecondsToTicks();
                this.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
                if (this.verbProps.stunTargetOnCastStart && castTarg.Pawn != null)
                {
                    castTarg.Pawn.stances.stunner.StunFor(ticks, null, false, true);
                }
            }
            else
            {
                this.WarmupComplete();
            }
            return true;
        }
        #endregion Overrides

        private bool CausesTimeSlowdown(LocalTargetInfo castTarg)
        {
            if (!this.verbProps.CausesTimeSlowdown)
            {
                return false;
            }
            if (!castTarg.HasThing)
            {
                return false;
            }
            Thing thing = castTarg.Thing;
            if (thing.def.category != ThingCategory.Pawn && (thing.def.building == null || !thing.def.building.IsTurret))
            {
                return false;
            }
            Pawn pawn = thing as Pawn;
            bool flag = pawn != null && pawn.Downed;
            return (thing.Faction == Faction.OfPlayer && this.caster.HostileTo(Faction.OfPlayer)) || (this.caster.Faction == Faction.OfPlayer && thing.HostileTo(Faction.OfPlayer) && !flag);
        }
    }
}