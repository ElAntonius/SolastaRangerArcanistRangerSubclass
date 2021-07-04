using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using SolastaModHelpers;
using UnityEngine.AddressableAssets;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;


namespace SolastaRangerArcanistRangerSubclass
{
    class RangerArcanistRangerSubclass
    {
        const string RangerArcanistRangerSubclassName = "RangerArcanistRangerSubclass";
        const string RangerArcanistRangerSubclassGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";

        public static Guid RA_BASE_GUID = new Guid(RangerArcanistRangerSubclassGuid);

        static public FeatureDefinitionFeatureSet ranger_arcanist_magic = createRangerArcanistMagic();
        static public FeatureDefinitionAdditionalDamage arcanist_mark = createArcanistMark();
        static public FeatureDefinitionAdditionalDamage arcane_detonation = createArcaneDetonation();
        static public FeatureDefinition arcane_detonation_upgrade = createArcaneDetonationUpgrade();
        static public Dictionary<int, FeatureDefinitionPower> arcane_pulse_dict = createArcanePulseDict();

        public static void BuildAndAddSubclass()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&RangerArcanistRangerSubclassDescription",
                    "Subclass/&RangerArcanistRangerSubclassTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(RangerArcanistRangerSubclassName, RangerArcanistRangerSubclassGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(ranger_arcanist_magic, 3)
                    .AddFeatureAtLevel(arcanist_mark, 3)
                    .AddFeatureAtLevel(arcane_detonation, 3)
                    .AddFeatureAtLevel(arcane_pulse_dict[7], 7)
                    .AddFeatureAtLevel(arcane_detonation_upgrade, 11)
                    .AddFeatureAtLevel(arcane_pulse_dict[15], 15)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes.Subclasses.Add(definition.Name);
        }

        private static DiceByRank buildDiceByRank(int rank, int dice)
        {
            DiceByRank diceByRank = new DiceByRank();
            diceByRank.SetField("rank", rank);
            diceByRank.SetField("diceNumber", dice);
            return diceByRank;
        }

        static FeatureDefinitionFeatureSet createRangerArcanistMagic()
        {
            var bonus_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.FeatureDefinitionExtraSpellsKnown>.createFeature
            (
                "BonusSpellsRangerArcanist",
                GuidHelper.Create(RA_BASE_GUID, "BonusSpellsRangerArcanist").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                bonus =>
                {
                    bonus.caster_class = DatabaseHelper.CharacterClassDefinitions.Ranger;
                    bonus.level = 3;
                    bonus.max_spells = 5;
                }
            );

            var extra_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantSpells>.createFeature
            (
                "ExtraSpellsRangerArcanist",
                GuidHelper.Create(RA_BASE_GUID, "ExtraSpellsRangerArcanist").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                feature =>
                {
                    feature.spellcastingClass = DatabaseHelper.CharacterClassDefinitions.Ranger;
                    feature.spellGroups = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
                    {
                        new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
                        {
                            ClassLevel = 3,
                            SpellsList = new List<SpellDefinition>()
                            {
                                DatabaseHelper.SpellDefinitions.Shield,
                                DatabaseHelper.SpellDefinitions.MistyStep,
                                DatabaseHelper.SpellDefinitions.Haste,
                                DatabaseHelper.SpellDefinitions.DimensionDoor,
                                DatabaseHelper.SpellDefinitions.HoldMonster
                            }
                        }
                    };
                }
            );

            var arcanist_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>.createFeatureCopy
            (
                "MagicAffinityRangerArcanist",
                GuidHelper.Create(RA_BASE_GUID, "MagicAffinityRangerArcanist").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic
            );

            return Helpers.FeatureSetBuilder.createFeatureSet
            (
                "RangerArcanistMagic",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistManaTouchedGuardian").ToString(),
                "Feature/&RangerArcanistMagicTitle",
                "Feature/&RangerArcanistMagicDescription",
                false,
                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                false,
                extra_spells, bonus_spells, arcanist_affinity
            );
        }

        static FeatureDefinitionAdditionalDamage createArcanistMark()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var mark_apply = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy
            (
                "AdditionalDamageArcanistMark",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcanistMark").ToString(),
                "Feature/&ArcanistMarkTitle",
                "Feature/&ArcanistMarkDescription",
                null,
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                a =>
                {
                    a.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
                    a.SetSpecificDamageType("DamageForce");
                    a.SetDamageDiceNumber(0);
                    a.SetDamageDieType(RuleDefinitions.DieType.D6);
                    a.SetNotificationTag("ArcanistMark");
                    a.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive);
                    a.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
                    a.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
                    a.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
                }
            );

            mark_apply.ConditionOperations.Clear();
            mark_apply.ConditionOperations.Add
            (
                new ConditionOperationDescription()
                {
                    ConditionDefinition = marked_condition,
                    Operation = ConditionOperationDescription.ConditionOperation.Add
                }
            );

            return mark_apply;
        }

        static FeatureDefinitionAdditionalDamage createArcaneDetonation()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var mark_damage = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy
            (
                "AdditionalDamageArcaneDetonation",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcaneDetonation").ToString(),
                "Feature/&ArcaneDetonationTitle",
                "Feature/&ArcaneDetonationDescription",
                null,
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                a =>
                {
                    a.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
                    a.SetRequiredTargetCondition(marked_condition);
                    a.SetSpecificDamageType("DamageForce");
                    a.SetNotificationTag("ArcanistMark");
                    a.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe);
                    a.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
                    a.SetDamageDiceNumber(1);
                    a.SetDamageDieType(RuleDefinitions.DieType.D6);
                    a.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
                    a.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel);
                    a.SetLimitedUsage(RuleDefinitions.FeatureLimitedUsage.None);

                    a.DiceByRankTable.Clear();
                    a.DiceByRankTable.AddRange(new List<DiceByRank>
                    {
                        buildDiceByRank(1, 1),
                        buildDiceByRank(2, 1),
                        buildDiceByRank(3, 1),
                        buildDiceByRank(4, 1),
                        buildDiceByRank(5, 1),
                        buildDiceByRank(6, 1),
                        buildDiceByRank(7, 1),
                        buildDiceByRank(8, 1),
                        buildDiceByRank(9, 1),
                        buildDiceByRank(10, 1),
                        buildDiceByRank(11, 2),
                        buildDiceByRank(12, 2),
                        buildDiceByRank(13, 2),
                        buildDiceByRank(14, 2),
                        buildDiceByRank(15, 2),
                        buildDiceByRank(16, 2),
                        buildDiceByRank(17, 2),
                        buildDiceByRank(18, 2),
                        buildDiceByRank(19, 2),
                        buildDiceByRank(20, 2)
                    });

                    a.ConditionOperations.Clear();
                    a.ConditionOperations.Add
                    (
                        new ConditionOperationDescription()
                        {
                            ConditionDefinition = marked_condition,
                            Operation = ConditionOperationDescription.ConditionOperation.Remove
                        }
                    );
                }
            );

            return mark_damage;
        }

        static FeatureDefinition createArcaneDetonationUpgrade()
        {
            // This is a blank feature. It does nothing except create a description for what happens at level 11.
            var blank_feature = Helpers.FeatureBuilder<FeatureDefinition>.createFeature
            (
                "AdditionalDamageArcaneDetonationUpgrade",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcaneDetonationUpgrade").ToString(),
                "Feature/&ArcaneDetonationUpgradeTitle",
                "Feature/&ArcaneDetonationUpgradeDescription",
                null
            );

            return blank_feature;
        }

        static Dictionary<int, FeatureDefinitionPower> createArcanePulseDict()
        {
            var marked_effect = new EffectForm();
            marked_effect.ConditionForm = new ConditionForm();
            marked_effect.FormType = EffectForm.EffectFormType.Condition;
            marked_effect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            marked_effect.ConditionForm.ConditionDefinition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var damage_effect = new EffectForm();
            damage_effect.DamageForm = new DamageForm();
            damage_effect.DamageForm.DamageType = "DamageForce";
            damage_effect.DamageForm.DieType = RuleDefinitions.DieType.D8;
            damage_effect.DamageForm.DiceNumber = 4;
            damage_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var pulse_description = new EffectDescription();
            pulse_description.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription);
            pulse_description.SetCreatedByCharacter(true);
            pulse_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            pulse_description.SetTargetType(RuleDefinitions.TargetType.Sphere);
            pulse_description.SetTargetParameter(3);
            pulse_description.SetRangeType(RuleDefinitions.RangeType.Distance);
            pulse_description.SetRangeParameter(30);

            pulse_description.EffectForms.Clear();
            pulse_description.EffectForms.AddRange(new List<EffectForm>
            {
                damage_effect,
                marked_effect
                
            });

            var arcane_pulse_action = Helpers.PowerBuilder.createPower
            (
                "ArcanePulse",
                GuidHelper.Create(RA_BASE_GUID, "ArcanePulse").ToString(),
                "Feature/&ArcanePulseTitle",
                "Feature/&ArcanePulseDescription",
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder,
                pulse_description,
                RuleDefinitions.ActivationTime.Action,
                0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                RuleDefinitions.RechargeRate.LongRest,
                Helpers.Stats.Wisdom,
                Helpers.Stats.Wisdom,
                1,
                false
            );
            arcane_pulse_action.SetShortTitleOverride("Arcane Pulse");

            var damage_upgrade_effect = new EffectForm();
            damage_upgrade_effect.DamageForm = new DamageForm();
            damage_upgrade_effect.DamageForm.DamageType = "DamageForce";
            damage_upgrade_effect.DamageForm.DieType = RuleDefinitions.DieType.D8;
            damage_upgrade_effect.DamageForm.DiceNumber = 8;
            damage_upgrade_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_upgrade_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var pulse_upgrade_description = new EffectDescription();
            pulse_upgrade_description.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription);
            pulse_upgrade_description.SetCreatedByCharacter(true);
            pulse_upgrade_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            pulse_upgrade_description.SetTargetType(RuleDefinitions.TargetType.Sphere);
            pulse_upgrade_description.SetTargetParameter(3);
            pulse_upgrade_description.SetRangeType(RuleDefinitions.RangeType.Distance);
            pulse_upgrade_description.SetRangeParameter(30);

            pulse_upgrade_description.EffectForms.Clear();
            pulse_upgrade_description.EffectForms.AddRange(new List<EffectForm>
            {
                damage_upgrade_effect,
                marked_effect
            });

            var arcane_pulse_upgrade_action = Helpers.PowerBuilder.createPower
            (
                "ArcanePulseUpgrade",
                GuidHelper.Create(RA_BASE_GUID, "ArcanePulseUpgrade").ToString(),
                "Feature/&ArcanePulseUpgradeTitle",
                "Feature/&ArcanePulseUpgradeDescription",
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder,
                pulse_upgrade_description,
                RuleDefinitions.ActivationTime.Action,
                0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                RuleDefinitions.RechargeRate.LongRest,
                Helpers.Stats.Wisdom,
                Helpers.Stats.Wisdom,
                1,
                false
            );
            arcane_pulse_upgrade_action.SetShortTitleOverride("Arcanist's Blast");
            arcane_pulse_upgrade_action.SetOverriddenPower(arcane_pulse_action);

            var arcane_pulse_dict = new Dictionary<int, FeatureDefinitionPower>();
            arcane_pulse_dict.Add(7, arcane_pulse_action);
            arcane_pulse_dict.Add(15, arcane_pulse_upgrade_action);

            return arcane_pulse_dict;
        }
    }

    // Creates a dedicated builder for the marked by arcanist condition. This helps with GUID wonkiness on the fact that separate features interact with it.
    internal class ConditionMarkedByArcanistBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        protected ConditionMarkedByArcanistBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByBrandingSmite, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionMarkedByArcanistTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionMarkedByArcanistDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(1);
            Definition.SetDurationType(RuleDefinitions.DurationType.Permanent);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSpecialDuration(true);
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionMarkedByArcanistBuilder("ConditionMarkedByArcanist", GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionMarkedByArcanist", GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()) ?? CreateAndAddToDB();
        }
    }
}