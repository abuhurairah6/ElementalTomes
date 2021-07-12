using RimWorld;
using Verse;

namespace ElementalTomes
{
    public class ThingDef_ElementalTomes : ThingDef
    {
        public float AddHediffChance;
        public HediffDef HediffToAdd;
    }

    public class Projectile_ElementalTomes : Projectile_Explosive
    {
        #region Properties
        public ThingDef_ElementalTomes Def
        {
            get
            {
                return this.def as ThingDef_ElementalTomes;
            }
        }
        #endregion Properties

        #region Override
        protected override void Explode()
        {
            IntVec3 strikeLoc = base.Position;
            this.Destroy(DestroyMode.Vanish);
            Find.CurrentMap.weatherManager.eventHandler.AddEvent(new ElementalEvent_LightningStrike(Find.CurrentMap, strikeLoc, def));
        }
        #endregion Override
    }
}
