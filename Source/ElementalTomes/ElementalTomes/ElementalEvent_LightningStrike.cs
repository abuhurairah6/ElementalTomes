using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace ElementalTomes
{
    [StaticConstructorOnStartup]
    class ElementalEvent_LightningStrike : WeatherEvent_LightningStrike
    {
        private IntVec3 strikeLoc = IntVec3.Invalid;
        private ThingDef def;
        private Thing launcher;
        private Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
        private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);

        public ElementalEvent_LightningStrike(Map map, IntVec3 forcedStrikeLoc, ThingDef hediffDef, Thing launcher) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.def = hediffDef;
            this.launcher = launcher;
        }

        #region Override;
        public override void FireEvent()
        {
            base.FireEvent();
            if (!this.strikeLoc.IsValid)
            {
                this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
            }
            if (!this.strikeLoc.Fogged(this.map))
            {
                Pawn pawn;
                pawn = (this.launcher as Pawn);
                int magic = (int) pawn.GetStatValue(DefDatabase<StatDef>.GetNamed("ET_MagicDPS"), true);

                float explosionRadius = this.def.projectile.explosionRadius;
                DamageDef damageDef = this.def.projectile.damageDef;
                int damageAmount = this.def.projectile.GetDamageAmount(1f, null) * magic;
                float armorPenetration = this.def.projectile.GetArmorPenetration(1f, null);
                ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
                float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
                int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
                bool applyDamageToExplosionCellsNeighbors = this.def.projectile.applyDamageToExplosionCellsNeighbors;
                ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
                float preExplosionSpawnChance = this.def.projectile.preExplosionSpawnChance;
                int preExplosionSpawnThingCount = this.def.projectile.preExplosionSpawnThingCount;
                float explosionChanceToStartFire = this.def.projectile.explosionChanceToStartFire;
                bool explosionDamageFalloff = this.def.projectile.explosionDamageFalloff;
                Log.Message("Magic is " + magic.ToString());
                Log.Message("Damage is " + damageAmount.ToString());
                Log.Message("Damage base is " + this.def.projectile.GetDamageAmount(1f, null).ToString());
                GenExplosion.DoExplosion(this.strikeLoc, this.map, explosionRadius, damageDef, null, 
                    damageAmount, armorPenetration, null, null, null, 
                    null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors, 
                    preExplosionSpawnThingDef, preExplosionSpawnChance, preExplosionSpawnThingCount, explosionChanceToStartFire, explosionDamageFalloff, 
                    null, null);

                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(loc, this.map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, this.map);
                    FleckMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(ElementalEvent_LightningStrike.LightningMat, base.LightningBrightness), 0);
        }
        #endregion Override;
    }
}
