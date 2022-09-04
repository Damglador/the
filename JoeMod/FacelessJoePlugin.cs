﻿using BepInEx;
using BepInEx.Logging;
using Modules.Survivors;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

[BepInDependency("com.cwmlolzlz.skills", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.KomradeSpectre.Aetherium", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.ThinkInvisible.TinkersSatchel", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.DrBibop.VRAPI", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]

[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
[R2APISubmoduleDependency(new string[]
{
    "PrefabAPI",
    "LanguageAPI",
    "SoundAPI",
    "LoadoutAPI",
    "DeployableAPI",
    "DamageAPI",
    "UnlockableAPI",
    "RecalculateStatsAPI"
})]

[BepInPlugin(MODUID, MODNAME, MODVERSION)]

public class FacelessJoePlugin : BaseUnityPlugin {
    public const string MODUID = "com.TheTimeSweeper.TeslaTrooper";
    public const string MODNAME = "Tesla Trooper";
    public const string MODVERSION = "1.2.2";

    // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
    public const string DEV_PREFIX = "HABIBI";

    internal List<SurvivorBase> Survivors = new List<SurvivorBase>();

    public static FacelessJoePlugin instance;
    public static ManualLogSource Log;

    public static bool Desolator = true;
    public static bool holdonasec;

    private void Start() {

        Logger.LogInfo("[Initializing Tesla Trooper]");

        instance = this;

        Log = Logger;

        // load assets and read config
        Modules.Assets.Initialize();
        Modules.Config.ReadConfig();
        if (Modules.Config.Debug)
            gameObject.AddComponent<TestValueManager>();

        Modules.Compat.Initialize();
        Modules.States.RegisterStates(); // register states for networking
        Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
        //Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
        Modules.Tokens.AddTokens(); // register name tokens
        Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

        Modules.DamageTypes.RegisterDamageTypes();
        Modules.Buffs.RegisterBuffs();

        // survivor initialization

        //someday
        //new JoeSurivor().Initialize();

        //init towers first
        TeslaTowerNotSurvivor baseTower = new TeslaTowerNotSurvivor();
        baseTower.Initialize();
            new TeslaTowerScepter().Initialize(baseTower);
        //the guy
        new TeslaTrooperSurvivor().Initialize();

        if (Desolator) {
            new DesolatorSurvivor().Initialize();
        }

        new Modules.ContentPacks().Initialize();

        Hook();

        Logger.LogInfo("[Initialized]");
    }

    private void Hook() {
        // run hooks here, disabling one is as simple as commenting out the line
        On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

        R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        //for figuring out plague knight throw bomb angles
        //On.EntityStates.Commando.CommandoWeapon.ThrowGrenade.PlayAnimation += ThrowGrenade_PlayAnimation;
    }

    private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args) {

        if (sender.HasBuff(Modules.Buffs.desolatorArmorBuff)) {
            args.armorAdd += 100f;
            args.moveSpeedMultAdd += 0.4f;
        }        
    }

    private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) {
        
        bool flag = (damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic;

        if (self && self.body && self.body.HasBuff(Modules.Buffs.zapShieldBuff) && !flag) {
            float mitigatedDamage = damageInfo.damage;
            
            if (Modules.Config.UtilityDamageAbsorption >= 1.0f) {
                damageInfo.rejected = true;
            } else {
                mitigatedDamage = (1.0f - Modules.Config.UtilityDamageAbsorption) * damageInfo.damage;
            }

            IReflectionBarrier bar = self.GetComponent<IReflectionBarrier>();
            if (bar != null) {
                bar.StoreDamage(damageInfo, damageInfo.damage);
                damageInfo.damage = mitigatedDamage;
            }
        }
        orig(self, damageInfo);
    }

    private void ThrowGrenade_PlayAnimation(On.EntityStates.Commando.CommandoWeapon.ThrowGrenade.orig_PlayAnimation orig, global::EntityStates.Commando.CommandoWeapon.ThrowGrenade self, float duration) {
        orig(self, duration);
        Chat.AddMessage(self.projectilePitchBonus.ToString());
    }
}
