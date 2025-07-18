﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CthulhuRealm.Common.Global;
/// <summary>
/// Offer powerful stats modify tool and easy code flow logic<br/>
/// Direct stats increases is much more efficient than using <see cref="PlayerStatsHandle.AddStatsToPlayer(PlayerStats, StatModifier)"/> or any of the relate<br/>
/// Due to some system uses <see cref="PlayerStats"/> so the above must be uses for ease of access
/// </summary>
public class PlayerStatsHandle : ModPlayer {
	public StatModifier UpdateMovement = new StatModifier();
	public StatModifier UpdateJumpBoost = new StatModifier();
	public StatModifier UpdateHPMax = new StatModifier();
	public StatModifier UpdateHPRegen = new StatModifier();
	public StatModifier UpdateManaMax = new StatModifier();
	public StatModifier UpdateManaRegen = new StatModifier();
	public StatModifier UpdateDefenseBase = new StatModifier();
	public StatModifier UpdateThorn = new StatModifier();
	public StatModifier UpdateDefEff = new StatModifier();
	public StatModifier UpdateMinion = new StatModifier();
	public StatModifier UpdateSentry = new StatModifier();
	/// <summary>
	/// This is the debuff buff time modifier for NPC<br/>
	/// this modifier will modify the duration of debuff on NPC<br/>
	/// Not to be confused with <see cref="DebuffBuffTime"/>
	/// </summary>
	public StatModifier DebuffTime = new StatModifier();
	public StatModifier BuffTime = new StatModifier();
	/// <summary>
	/// This is the debuff buff time modifier for player<br/>
	/// this modifier will modify the duration of debuff on player<br/>
	/// Not to be confused with <see cref="DebuffTime"/>
	/// </summary>
	public StatModifier DebuffBuffTime = new StatModifier();
	public StatModifier AttackSpeed = new StatModifier();
	public StatModifier HealEffectiveness = new StatModifier();
	public StatModifier DebuffDamage = new StatModifier();
	public StatModifier Iframe = new StatModifier();
	public StatModifier DirectItemDamage = new();
	public float Hostile_ProjectileVelocityAddition = 0;
	//public float LuckIncrease = 0; 
	/// <summary>
	/// This is a universal dodge chance that work like <see cref="Player.endurance"/><br/>
	/// Having the chance value over 1f would make it 100% dodge chance<br/>
	/// The dodge immunity frame is <see cref="DodgeTimer"/>
	/// </summary>
	public float DodgeChance = 0;
	/// <summary>
	/// This is the universal dodge timer
	/// </summary>
	public int DodgeTimer = 44;
	/// <summary>
	/// This is a universal life steal that work depend on weapon damage <br/>
	/// This have a forced cool down so that it is not OP <br/>
	/// The cool down are made public and free to be modify cause fun
	/// </summary>
	public StatModifier LifeSteal = StatModifier.Default - 1;
	/// <summary>
	/// This is the public count cool down of <see cref="LifeSteal"/>
	/// </summary>
	public int LifeSteal_CoolDownCounter = 0;
	/// <summary>
	/// This is the count down, by default it set to 1s
	/// </summary>
	public int LifeSteal_CoolDown = 60;
	public int Rapid_LifeRegen = 0;
	public int Rapid_ManaRegen = 0;
	public int Debuff_LifeStruct = 0;
	/// <summary>
	/// This only work if no where in the code don't uses <see cref="NPC.HitModifiers.DisableCrit"/>
	/// </summary>
	public bool ModifyHit_Before_Crit = false;

	/// <summary>
	/// Use this if you want to make a series of item that shoot out all of the effect in the same timeline
	/// </summary>
	public int synchronize_Counter = 0;
	public int TemporaryLife = 0;
	public int TemporaryLife_Counter = 0;
	public int TemporaryLife_Limit = 0;
	public int TemporaryLife_CounterLimit = 120;
	public float ItemRangeMultiplier = 1;
	/// <summary>
	/// This chance will decay for each success roll <br/>
	/// For direct adding augmentation but still random use <code>AugmentsPlayer.SafeRequest_AddAugments(float chance, int limit, bool decayable)</code>
	/// </summary>
	public float AugmentationChance = 0;
	/// <summary>
	/// Uses for enchantment cool down effect
	/// </summary>
	public StatModifier EnchantmentCoolDown = StatModifier.Default;
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		if (LifeSteal_CoolDownCounter <= 0 && LifeSteal.Additive > 0 && LifeSteal.ApplyTo(1) > 0) {
			Player.Heal((int)Math.Ceiling(LifeSteal.ApplyTo(hit.Damage)));
			LifeSteal_CoolDownCounter = LifeSteal_CoolDown;
		}
	}
	public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo) {
		if (UpdateThorn.ApplyTo(1) > 0) {
			NPC.HitInfo newhitinfo = new();
			newhitinfo.Damage = (int)UpdateThorn.ApplyTo(hurtInfo.Damage);
			newhitinfo.Crit = false;
			newhitinfo.DamageType = DamageClass.Default;
			Player.StrikeNPCDirect(npc, newhitinfo);
		}
	}
	public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
		modifiers.SourceDamage.CombineWith(DirectItemDamage);
	}
	public StatModifier UpdateCritDamage = new StatModifier();
	public StatModifier Melee_CritDamage = StatModifier.Default;
	public StatModifier Range_CritDamage = StatModifier.Default;
	public StatModifier Magic_CritDamage = StatModifier.Default;
	public StatModifier Summon_CritDamage = StatModifier.Default;
	public StatModifier NonCriticalDamage = new StatModifier();
	public StatModifier Melee_NonCritDmg = StatModifier.Default;
	public StatModifier Range_NonCritDmg = StatModifier.Default;
	public StatModifier Magic_NonCritDmg = StatModifier.Default;
	public StatModifier Summon_NonCritDmg = StatModifier.Default;
	public StatModifier MeleeAtkSpeed = StatModifier.Default;
	public StatModifier RangeAtkSpeed = StatModifier.Default;
	public StatModifier MagicAtkSpeed = StatModifier.Default;
	public StatModifier SummonAtkSpeed = StatModifier.Default;
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
		var item = Player.HeldItem;
		modifiers.CritDamage = modifiers.CritDamage.CombineWith(UpdateCritDamage);
		modifiers.NonCritDamage = modifiers.NonCritDamage.CombineWith(NonCriticalDamage);
		if (modifiers.DamageType == DamageClass.Melee) {
			modifiers.CritDamage = modifiers.CritDamage.CombineWith(Melee_CritDamage);
			modifiers.NonCritDamage = modifiers.NonCritDamage.CombineWith(Melee_NonCritDmg);
		}
		else if (modifiers.DamageType == DamageClass.Ranged) {
			modifiers.CritDamage = modifiers.CritDamage.CombineWith(Range_CritDamage);
			modifiers.NonCritDamage = modifiers.NonCritDamage.CombineWith(Range_NonCritDmg);
		}
		else if (modifiers.DamageType == DamageClass.Magic) {
			modifiers.CritDamage = modifiers.CritDamage.CombineWith(Magic_CritDamage);
			modifiers.NonCritDamage = modifiers.NonCritDamage.CombineWith(Magic_NonCritDmg);
		}
		else if (modifiers.DamageType == DamageClass.Summon) {
			modifiers.CritDamage = modifiers.CritDamage.CombineWith(Summon_CritDamage);
			modifiers.NonCritDamage = modifiers.NonCritDamage.CombineWith(Summon_NonCritDmg);
		}
		bool HasDebuff = false; int count = 0;
		for (int i = 0; i < target.buffType.Length; i++) {
			if (target.buffType[i] <= 0) {
				continue;
			}
			if (Main.debuff[target.buffType[i]]) {
				HasDebuff = true;
				count++;
			}
		}
		if (HasDebuff)
			modifiers.SourceDamage = modifiers.SourceDamage.CombineWith(DebuffDamage * count);

		modifiers.ModifyHitInfo += Modifiers_ModifyHitInfo;

		if (AlwaysCritValue > 0) {
			modifiers.SetCrit();
		}
		else if (AlwaysCritValue < 0) {
			modifiers.DisableCrit();
		}
	}
	private void Modifiers_ModifyHitInfo(ref NPC.HitInfo info) {
		ModifyHit_Before_Crit = info.Crit;
		if (info.Crit) {
			ModifyHit_Before_Crit = true;
		}
	}
	public override bool FreeDodge(Player.HurtInfo info) {
		if (Main.rand.NextFloat() <= DodgeChance) {
			Player.AddImmuneTime(info.CooldownCounter, DodgeTimer);
			return true;
		}
		return base.FreeDodge(info);
	}
	public override void PostUpdate() {
		Player.statLife = Math.Clamp(Player.statLife + Rapid_LifeRegen, 1, Player.statLifeMax2);
		Player.statMana = Math.Clamp(Player.statMana + Rapid_ManaRegen, 0, Player.statManaMax2);
	}
	public override void UpdateLifeRegen() {
		Player.lifeRegen = (int)UpdateHPRegen.ApplyTo(Player.lifeRegen);
	}
	public void Set_TemporaryLife(int limit, int counterlimit) {
		TemporaryLife_Limit += limit;
		TemporaryLife_CounterLimit += counterlimit;
	}
	public float CurrentMinionAmount = 0;
	public int CappedHealthAmount = -1;
	public override void ResetEffects() {
		CurrentMinionAmount = 0;
		for (int i = 0; i < Main.projectile.Length; i++) {
			if (Main.projectile[i].minion && Main.projectile[i].active) {
				CurrentMinionAmount += Main.projectile[i].minionSlots;
			}
		}
		synchronize_Counter = ModUtils.Safe_SwitchValue(synchronize_Counter, int.MaxValue);
		Player.moveSpeed = UpdateMovement.ApplyTo(Player.moveSpeed);
		Player.jumpSpeedBoost = UpdateJumpBoost.ApplyTo(Player.jumpSpeedBoost);
		Player.manaRegen = (int)UpdateManaRegen.ApplyTo(Player.manaRegen);
		Player.statDefense += (int)(UpdateDefenseBase.Base + UpdateDefenseBase.Flat);
		Player.statDefense.AdditiveBonus += UpdateDefenseBase.Additive - 1;
		Player.statDefense.FinalMultiplier *= UpdateDefenseBase.Multiplicative;
		Player.DefenseEffectiveness *= UpdateDefEff.ApplyTo(Player.DefenseEffectiveness.Value);

		Player.maxMinions = (int)UpdateMinion.ApplyTo(Player.maxMinions);
		Player.maxTurrets = (int)UpdateSentry.ApplyTo(Player.maxTurrets);

		if (TemporaryLife > 0) {
			if (++TemporaryLife_Counter >= TemporaryLife_CounterLimit) {
				TemporaryLife -= 1;
				TemporaryLife_Counter = 0;
			}
			TemporaryLife = Math.Clamp(TemporaryLife, 0, TemporaryLife_Limit);
			TemporaryLife_Limit = 0;
			TemporaryLife_CounterLimit = 0;
		}

		if (CappedHealthAmount == -1) {
			Player.statLifeMax2 = Math.Clamp((int)UpdateHPMax.ApplyTo(Player.statLifeMax2) + TemporaryLife, 1, int.MaxValue);
		}
		else {
			Player.statLifeMax2 = Math.Clamp((int)UpdateHPMax.ApplyTo(Player.statLifeMax2) + TemporaryLife, 1, CappedHealthAmount);
		}
		CappedHealthAmount = -1;
		Player.statManaMax2 = Math.Clamp((int)UpdateManaMax.ApplyTo(Player.statManaMax2), 1, int.MaxValue);

		UpdateCritDamage = StatModifier.Default;
		Melee_CritDamage = StatModifier.Default;
		Range_CritDamage = StatModifier.Default;
		Magic_CritDamage = StatModifier.Default;
		Summon_CritDamage = StatModifier.Default;

		NonCriticalDamage = StatModifier.Default;
		Melee_NonCritDmg = StatModifier.Default;
		Range_NonCritDmg = StatModifier.Default;
		Magic_NonCritDmg = StatModifier.Default;
		Summon_NonCritDmg = StatModifier.Default;

		MeleeAtkSpeed = StatModifier.Default;
		RangeAtkSpeed = StatModifier.Default;
		MagicAtkSpeed = StatModifier.Default;
		SummonAtkSpeed = StatModifier.Default;
		UpdateMinion = StatModifier.Default;
		UpdateSentry = StatModifier.Default;
		UpdateMovement = StatModifier.Default;
		UpdateJumpBoost = StatModifier.Default;
		UpdateHPMax = StatModifier.Default;
		UpdateManaMax = StatModifier.Default;
		UpdateHPRegen = StatModifier.Default;
		UpdateManaRegen = StatModifier.Default;
		UpdateDefenseBase = StatModifier.Default;
		UpdateDefEff = StatModifier.Default;
		UpdateThorn = StatModifier.Default - 1;
		DebuffTime = StatModifier.Default;
		BuffTime = StatModifier.Default;
		DebuffBuffTime = StatModifier.Default;
		AttackSpeed = StatModifier.Default;
		HealEffectiveness = StatModifier.Default;
		DebuffDamage = StatModifier.Default;
		Iframe = StatModifier.Default;
		LifeSteal = StatModifier.Default - 1;
		DirectItemDamage = StatModifier.Default;
		DodgeChance = 0;
		DodgeTimer = 44;
		successfullyKillNPCcount = 0;
		LifeSteal_CoolDown = 60;
		LifeSteal_CoolDownCounter = ModUtils.CountDown(LifeSteal_CoolDownCounter);
		ModifyHit_Before_Crit = false;
		Rapid_LifeRegen = 0;
		Rapid_ManaRegen = 0;
		ItemRangeMultiplier = 1;
	
		AlwaysCritValue = 0;
	}
	/// <summary>
	/// This value will do a smarter handling of whenever or not should a critical strike be guaranteed or disable<br/>
	/// If this value is <![CDATA[>]]> 0 then the hit will always be critical<br/>
	/// If this value is <![CDATA[<]]> 0 then the hit will never be critical<br/>
	/// If this value is = 0 then the hit will use vanilla critical hit calculation<br/>
	/// </summary>
	public short AlwaysCritValue = 0;
	public override float UseSpeedMultiplier(Item item) {
		float useSpeed = base.UseSpeedMultiplier(item);
		StatModifier global = AttackSpeed;
		if (item.DamageType == DamageClass.Melee) {
			global = AttackSpeed.CombineWith(MeleeAtkSpeed);
		}
		else if (item.DamageType == DamageClass.Ranged) {
			global = AttackSpeed.CombineWith(RangeAtkSpeed);
		}
		else if (item.DamageType == DamageClass.Magic) {
			global = AttackSpeed.CombineWith(MagicAtkSpeed);
		}
		else if (item.DamageType == DamageClass.Summon) {
			global = AttackSpeed.CombineWith(SummonAtkSpeed);
		}
		return MathF.Round(global.ApplyTo(useSpeed), 2);
	}
	public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
		foreach (var chance in Chance_SecondLife.Values) {
			if (Main.rand.NextFloat() <= chance.ChanceValue) {
				chance.ApprovedConditionPass();
				return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
			}
		}
		foreach (var chance in SecondLife.Values) {
			if (chance.Condition) {
				chance.ApprovedConditionPass();
				return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
			}
		}
		return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
	}
	/// <summary>
	/// This should be uses in always update code
	/// when creating a new stat modifier, pleases uses the default and increases from there
	/// </summary>
	/// <param name="stat"></param>
	public void AddStatsToPlayer(PlayerStats stat, StatModifier StatMod, float singularAdditiveMultiplier = 1, float singularBaseMultiplier = 1) {
		if (stat == PlayerStats.None) {
			return;
		}
		StatMod = new(MathF.Round(StatMod.Additive + (StatMod.Additive - 1) * singularAdditiveMultiplier, 2), MathF.Round(StatMod.Multiplicative, 2), MathF.Round(StatMod.Flat, 2), MathF.Round(StatMod.Base * singularBaseMultiplier, 2));
		switch (stat) {
			case PlayerStats.MeleeDMG:
				Player.GetDamage(DamageClass.Melee) = Player.GetDamage(DamageClass.Melee).CombineWith(StatMod);
				break;
			case PlayerStats.RangeDMG:
				Player.GetDamage(DamageClass.Ranged) = Player.GetDamage(DamageClass.Ranged).CombineWith(StatMod);
				break;
			case PlayerStats.MagicDMG:
				Player.GetDamage(DamageClass.Magic) = Player.GetDamage(DamageClass.Magic).CombineWith(StatMod);
				break;
			case PlayerStats.SummonDMG:
				Player.GetDamage(DamageClass.Summon) = Player.GetDamage(DamageClass.Summon).CombineWith(StatMod);
				break;
			case PlayerStats.MeleeCritChance:
				Player.GetCritChance(DamageClass.Melee) = StatMod.ApplyTo(Player.GetCritChance(DamageClass.Melee));
				break;
			case PlayerStats.RangeCritChance:
				Player.GetCritChance(DamageClass.Ranged) = StatMod.ApplyTo(Player.GetCritChance(DamageClass.Ranged));
				break;
			case PlayerStats.MagicCritChance:
				Player.GetCritChance(DamageClass.Magic) = StatMod.ApplyTo(Player.GetCritChance(DamageClass.Magic));
				break;
			case PlayerStats.SummonCritChance:
				Player.GetCritChance(DamageClass.Summon) = StatMod.ApplyTo(Player.GetCritChance(DamageClass.Summon));
				break;
			case PlayerStats.MovementSpeed:
				UpdateMovement = UpdateMovement.CombineWith(StatMod);
				break;
			case PlayerStats.JumpBoost:
				UpdateJumpBoost = UpdateJumpBoost.CombineWith(StatMod);
				break;
			case PlayerStats.MaxHP:
				UpdateHPMax = UpdateHPMax.CombineWith(StatMod);
				break;
			case PlayerStats.RegenHP:
				UpdateHPRegen = UpdateHPRegen.CombineWith(StatMod);
				break;
			case PlayerStats.MaxMana:
				UpdateManaMax = UpdateManaMax.CombineWith(StatMod);
				break;
			case PlayerStats.RegenMana:
				UpdateManaRegen = UpdateManaRegen.CombineWith(StatMod);
				break;
			case PlayerStats.Defense:
				UpdateDefenseBase = UpdateDefenseBase.CombineWith(StatMod);
				break;
			case PlayerStats.PureDamage:
				Player.GetDamage(DamageClass.Generic) = Player.GetDamage(DamageClass.Generic).CombineWith(StatMod);
				break;
			case PlayerStats.CritChance:
				Player.GetCritChance(DamageClass.Generic) = StatMod.ApplyTo(Player.GetCritChance(DamageClass.Generic));
				break;
			case PlayerStats.CritDamage:
				UpdateCritDamage = UpdateCritDamage.CombineWith(StatMod);
				break;
			case PlayerStats.DefenseEffectiveness:
				UpdateDefEff = UpdateDefEff.CombineWith(StatMod);
				break;
			case PlayerStats.Thorn:
				UpdateThorn = UpdateThorn.CombineWith(StatMod);
				break;
			case PlayerStats.MaxMinion:
				UpdateMinion = UpdateMinion.CombineWith(StatMod);
				break;
			case PlayerStats.MaxSentry:
				UpdateSentry = UpdateSentry.CombineWith(StatMod);
				break;
			case PlayerStats.AttackSpeed:
				AttackSpeed = AttackSpeed.CombineWith(StatMod);
				break;
			case PlayerStats.HealEffectiveness:
				HealEffectiveness = HealEffectiveness.CombineWith(StatMod);
				break;
			case PlayerStats.Iframe:
				Iframe = Iframe.CombineWith(StatMod);
				break;
			case PlayerStats.DebuffDurationInflict:
				DebuffTime = DebuffTime.CombineWith(StatMod);
				break;
			case PlayerStats.LifeSteal:
				LifeSteal = LifeSteal.CombineWith(StatMod - 1);
				break;
			case PlayerStats.MeleeCritDmg:
				Melee_CritDamage = Melee_CritDamage.CombineWith(StatMod);
				break;
			case PlayerStats.RangeCritDmg:
				Range_CritDamage = Range_CritDamage.CombineWith(StatMod);
				break;
			case PlayerStats.MagicCritDmg:
				Magic_CritDamage = Magic_CritDamage.CombineWith(StatMod);
				break;
			case PlayerStats.SummonCritDmg:
				Summon_CritDamage = Summon_CritDamage.CombineWith(StatMod);
				break;
			case PlayerStats.MeleeNonCritDmg:
				Melee_NonCritDmg = Melee_NonCritDmg.CombineWith(StatMod);
				break;
			case PlayerStats.RangeNonCritDmg:
				Range_NonCritDmg = Range_NonCritDmg.CombineWith(StatMod);
				break;
			case PlayerStats.MagicNonCritDmg:
				Magic_NonCritDmg = Magic_NonCritDmg.CombineWith(StatMod);
				break;
			case PlayerStats.SummonNonCritDmg:
				Summon_NonCritDmg = Summon_NonCritDmg.CombineWith(StatMod);
				break;
			default:
				break;
		}
	}

	public int successfullyKillNPCcount = 0;
	/// <summary>
	/// This should be uses in always update code
	/// when creating a new stat modifier, pleases uses the default and increases from there
	/// </summary>
	/// <param name="stat"></param>
	/// <param name="Additive"></param>
	/// <param name="Multiplicative"></param>
	/// <param name="Flat"></param>
	/// <param name="Base"></param>
	public void AddStatsToPlayer(PlayerStats stat, float Additive = 1, float Multiplicative = 1, float Flat = 0, float Base = 0, float singularAdditiveMultiplier = 1, float singularBaseMultiplier = 1) {
		var StatMod = new StatModifier();
		StatMod += Additive - 1;
		StatMod *= Multiplicative;
		StatMod.Flat = Flat;
		StatMod.Base = Base;
		AddStatsToPlayer(stat, StatMod, singularAdditiveMultiplier, singularBaseMultiplier);
	}
	/// <summary>
	/// Use this for a universal way to increases stats without fear of accidentally create multiplicative<br/>
	/// This should be uses in always update code<br/>
	/// when creating a new stat modifier, pleases uses the default and increases from there
	/// </summary>
	/// <param name="stat"></param>
	/// <param name="Additive"></param>
	/// <param name="Multiplicative"></param>
	/// <param name="Flat"></param>
	/// <param name="Base"></param>
	public static void AddStatsToPlayer(Player player, PlayerStats stat, float Additive = 1, float Multiplicative = 1, float Flat = 0, float Base = 0, float singularAdditiveMultiplier = 1, float singularBaseMultiplier = 1) {
		player.GetModPlayer<PlayerStatsHandle>().AddStatsToPlayer(stat, Additive, Multiplicative, Flat, Base, singularAdditiveMultiplier, singularBaseMultiplier);
	}
	/// <summary>
	/// Use this for a universal way to increases stats without fear of accidentally create multiplicative<br/>
	/// This should be uses in always update code<br/>
	/// when creating a new stat modifier, pleases uses the default and increases from there
	/// </summary>
	/// <param name="stat"></param>
	/// <param name="Additive"></param>
	/// <param name="Multiplicative"></param>
	/// <param name="Flat"></param>
	/// <param name="Base"></param>
	public static void AddStatsToPlayer(Player player, PlayerStats stat, StatModifier modifier, float singularAdditiveMultiplier = 1, float singularBaseMultiplier = 1) {
		player.GetModPlayer<PlayerStatsHandle>().AddStatsToPlayer(stat, modifier, singularAdditiveMultiplier, singularBaseMultiplier);
	}
	/// <summary>
	/// Only work with certain PlayerStats
	/// </summary>
	/// <param name="stat">The stats to convert</param>
	/// <returns>
	/// Return DamageClass.Class if the condition met<br/>
	/// Otherwise return <see cref="DamageClass.Default"/>
	/// </returns>
	public static DamageClass PlayerStatsToDamageClass(PlayerStats stat) {
		switch (stat) {
			case PlayerStats.MeleeDMG:
				return DamageClass.Melee;
			case PlayerStats.RangeDMG:
				return DamageClass.Ranged;
			case PlayerStats.MagicDMG:
				return DamageClass.Magic;
			case PlayerStats.SummonDMG:
				return DamageClass.Summon;
			case PlayerStats.PureDamage:
				return DamageClass.Generic;
			default:
				return DamageClass.Default;
		}
	}
	public static int WE_CoolDown(Player player, int cooldown) {
		PlayerStatsHandle handle = player.GetModPlayer<PlayerStatsHandle>();
		float newcooldown = handle.EnchantmentCoolDown.ApplyTo(cooldown);
		return (int)Math.Max(Math.Ceiling(newcooldown), 0);
	}
	public Dictionary<string, ConditionApproved> SecondLife = new();
	public static void SetSecondLifeCondition(Player player, string context, bool condition) {
		var modplayer = player.GetModPlayer<PlayerStatsHandle>();
		if (modplayer.SecondLife.ContainsKey(context)) {
			modplayer.SecondLife[context].ChangeCondition(condition);
		}
		else {
			modplayer.SecondLife.Add(context, new(condition));
		}
	}
	public static bool GetSecondLife(Player player, string context) {
		var modplayer = player.GetModPlayer<PlayerStatsHandle>();
		if (modplayer.SecondLife.ContainsKey(context)) {
			if (modplayer.SecondLife[context].Approved) {
				modplayer.SecondLife[context].DeApproved();
				return true;
			}
		}
		return false;
	}
	public Dictionary<string, ConditionApproved> Chance_SecondLife = new();
	public static void Set_Chance_SecondLifeCondition(Player player, string context, float chance) {
		var modplayer = player.GetModPlayer<PlayerStatsHandle>();
		if (modplayer.Chance_SecondLife.ContainsKey(context)) {
			modplayer.Chance_SecondLife[context].SetChanceValue(chance);
		}
		else {
			modplayer.Chance_SecondLife.Add(context, new(chance));
		}
	}
	public static bool Get_Chance_SecondLife(Player player, string context) {
		var modplayer = player.GetModPlayer<PlayerStatsHandle>();
		if (modplayer.Chance_SecondLife.ContainsKey(context)) {
			if (modplayer.Chance_SecondLife[context].Approved) {
				modplayer.Chance_SecondLife[context].DeApproved();
				return true;
			}
		}
		return false;
	}
}
/// <summary>
/// This is for the second life mechanic
/// </summary>
public class ConditionApproved {
	public bool Condition = false;
	public bool Approved = false;
	public float ChanceValue = 0f;
	public ConditionApproved() {
		Condition = false;
		Approved = false;
		ChanceValue = 0;
	}
	public ConditionApproved(bool condition) {
		Condition = condition;
		Approved = false;
		ChanceValue = 0;
	}
	public ConditionApproved(float chance) {
		Condition = false;
		Approved = false;
		ChanceValue = chance;
	}
	public void SetChanceValue(float chance) {
		ChanceValue = chance;
	}
	public void ChangeCondition(bool condition) {
		Condition = condition;
	}
	public void ApprovedConditionPass() {
		Approved = true;
	}
	public void DeApproved() {
		Approved = false;
	}
	public void Reset() {
		Condition = false;
		Approved = false;
	}
}
public class PlayerStatsHandleSystem : ModSystem {
	public override void Load() {
		On_NPC.AddBuff += HookBuffTimeModify;
		On_Player.AddBuff += IncreasesPlayerBuffTime;
		On_Player.Heal += On_Player_Heal;
		On_Player.AddImmuneTime += On_Player_AddImmuneTime;
		On_Player.GiveImmuneTimeForCollisionAttack += On_Player_GiveImmuneTimeForCollisionAttack;
		On_Player.SetImmuneTimeForAllTypes += On_Player_SetImmuneTimeForAllTypes;
		On_Player.GetItemGrabRange += On_Player_GetItemGrabRange;
		On_NPC.HitModifiers.GetDamage += HitModifiers_GetDamage;
	}
	private int HitModifiers_GetDamage(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck) {
		int fixedDamage = 0;
		if (self.FinalDamage.Flat > 0) {
			fixedDamage = (int)self.FinalDamage.Flat;
		}
		int damage = orig(ref self, baseDamage, crit, damageVariation, luck) + fixedDamage;
		return damage;
	}

	private int On_Player_GetItemGrabRange(On_Player.orig_GetItemGrabRange orig, Player self, Item item) {
		int itemRange = orig(self, item);
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			return (int)(itemRange * modplayer.ItemRangeMultiplier);
		}
		else {
			return itemRange;
		}
	}

	private void On_Player_SetImmuneTimeForAllTypes(On_Player.orig_SetImmuneTimeForAllTypes orig, Player self, int time) {
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			orig(self, (int)(modplayer.Iframe - 1).ApplyTo(time) + time);
		}
		else {
			orig(self, time);
		}
	}

	private void On_Player_GiveImmuneTimeForCollisionAttack(On_Player.orig_GiveImmuneTimeForCollisionAttack orig, Player self, int time) {
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			orig(self, (int)(modplayer.Iframe - 1).ApplyTo(time) + time);
		}
		else {
			orig(self, time);
		}
	}

	private void On_Player_AddImmuneTime(On_Player.orig_AddImmuneTime orig, Player self, int cooldownCounterId, int immuneTime) {
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			orig(self, cooldownCounterId, (int)(modplayer.Iframe - 1).ApplyTo(immuneTime) + immuneTime);
		}
		else {
			orig(self, cooldownCounterId, immuneTime);
		}
	}

	private void On_Player_Heal(On_Player.orig_Heal orig, Player self, int amount) {
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			orig(self, (int)modplayer.HealEffectiveness.ApplyTo(amount));
		}
		else {
			orig(self, amount);
		}
	}

	private void IncreasesPlayerBuffTime(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack) {
		if (self.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			if (!Main.debuff[type]) {
				orig(self, type, (int)modplayer.BuffTime.ApplyTo(timeToAdd), quiet, foodHack);
			}
			else {
				int time = (int)modplayer.DebuffBuffTime.ApplyTo(timeToAdd);
				orig(self, type, time, quiet, foodHack);
			}
		}
		else {
			orig(self, type, timeToAdd, quiet, foodHack);
		}
	}

	private void HookBuffTimeModify(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet) {
		var player = Main.player[self.lastInteraction];
		if (player.TryGetModPlayer(out PlayerStatsHandle modplayer)) {
			orig(self, type, (int)modplayer.DebuffTime.ApplyTo(time), quiet);
		}
		else {
			orig(self, type, time, quiet);
		}
	}
}
