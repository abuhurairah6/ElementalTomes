using RimWorld;
using Verse;

namespace ElementalTomes
{
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
