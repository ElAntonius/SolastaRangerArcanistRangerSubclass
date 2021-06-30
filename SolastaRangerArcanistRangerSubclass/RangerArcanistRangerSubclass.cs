using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
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

        public static void BuildAndAddSubclass()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&RangerArcanistRangerSubclassDescription",
                    "Subclass/&RangerArcanistRangerSubclassTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(RangerArcanistRangerSubclassName, RangerArcanistRangerSubclassGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(createRangerArcanistMagic(), 3)
                    .AddFeatureAtLevel(createArcanistMark(), 3)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes.Subclasses.Add(definition.Name);
        }

        static FeatureDefinition createRangerArcanistMagic()
        {
            var bonus_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.FeatureDefinitionExtraSpellsKnown>.createFeature
            (
                "RangerArcanistBonusSpells",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistBonusSpells").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                bonus =>
                {
                    bonus.caster_class = DatabaseHelper.CharacterClassDefinitions.Ranger;
                    bonus.level = 3;
                    bonus.max_spells = 3;
                }
            );

            var extra_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantSpells>.createFeature
            (
                "RangerArcanistExtraSpells",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistExtraSpells").ToString(),
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
                                DatabaseHelper.SpellDefinitions.Haste
                            }
                        }
                    };
                }
            );

            var arcanist_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>.createFeatureCopy
            (
                "RangerArcanistAffinity",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistAffinity").ToString(),
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

        static FeatureDefinition createArcanistMark()
        {
            var mark_condition = Helpers.ConditionBuilder.createCondition
            (
                "ConditionArcanistMark",
                GuidHelper.Create(RA_BASE_GUID, "ConditionArcanistMark").ToString(),
                "Condition/&ConditionArcanistMarkTitle",
                "Condition/&ConditionArcanistMarkDescription",
                null,
                DatabaseHelper.ConditionDefinitions.ConditionBrandingSmite
            );
            mark_condition.SetAllowMultipleInstances(false);

            var mark_form = new EffectForm();
            mark_form.ConditionForm = new ConditionForm();
            mark_form.FormType = EffectForm.EffectFormType.Condition;
            mark_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            mark_form.ConditionForm.ConditionDefinition = mark_condition;

            // Create a Description for the Mark Effect
            var mark_description_builder = new EffectDescriptionBuilder();
            mark_description_builder.SetCreatedByCharacter();
            mark_description_builder.SetDurationData(RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            mark_description_builder.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 0, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Weapon);
            mark_description_builder.AddEffectForm(mark_form);

            var mark_description = mark_description_builder.Build();
            mark_description.SetTargetConditionName("Arcanist's Mark");

            var marked_condition = Helpers.ConditionBuilder.createCondition
            (
                "ConditionMarkedByArcanist",
                GuidHelper.Create(RA_BASE_GUID, "ConditionMarkedByArcanist").ToString(),
                "Condition/&ConditionMarkedByArcanistTitle",
                "Condition/&ConditionMarkedByArcanistDescription",
                null,
                DatabaseHelper.ConditionDefinitions.ConditionMarkedByBrandingSmite
            );
            marked_condition.SetAllowMultipleInstances(false);

            var marked_form = new EffectForm();
            marked_form.ConditionForm = new ConditionForm();
            marked_form.FormType = EffectForm.EffectFormType.Condition;
            marked_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            marked_form.ConditionForm.ConditionDefinition = marked_condition;

            // Create a Description for the Mark Effect
            var marked_description_builder = new EffectDescriptionBuilder();
            marked_description_builder.SetCreatedByCharacter();
            marked_description_builder.SetDurationData(RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            marked_description_builder.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 120, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Weapon);
            marked_description_builder.AddEffectForm(marked_form);

            var marked_description = marked_description_builder.Build();
            marked_description.SetTargetConditionName("Arcanist's Mark");

            var mark_on_hit = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnDamageTakenToTarget>.createFeature
            (
                "ArcanistMarkOnHit",
                GuidHelper.Create(RA_BASE_GUID, "ArcanistMarkOnHit").ToString(),
                "Feature/&ArcanistMarkOnHitTitle",
                "Feature/&ArcanistMarkOnHitDescription",
                null,
                f =>
                {
                    f.condition = marked_condition;
                    f.durationType = RuleDefinitions.DurationType.Permanent;
                    f.durationValue = 1;
                    f.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                }
            );

            var mark_action = Helpers.PowerBuilder.createPower
            (
                "ArcanistMarkPower",
                GuidHelper.Create(RA_BASE_GUID, "ArcanistMarkPower").ToString(),
                "Feature/&ArcanistMarkPowerTitle",
                "Feature/&ArcanistMarkPowerDescription",
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike,
                marked_description,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                RuleDefinitions.RechargeRate.AtWill,
                Helpers.Stats.Wisdom,
                Helpers.Stats.Wisdom,
                0,
                false
            );
            mark_action.SetShortTitleOverride("Arcanist's Mark");

            var mark_apply = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy
            (
                "AdditionalDamageArcanistMarkApplication",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcanistMarkApplication").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                a =>
                {
                    a.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
                    a.SetSpecificDamageType("DamageForce");
                    a.SetRequiredTargetCondition(marked_condition);
                    a.SetDamageDiceNumber(0);
                    a.SetDamageDieType(RuleDefinitions.DieType.D8);
                    a.SetNotificationTag("ArcanistMark");
                    a.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition);
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

            var mark_damage = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy
            (
                "AdditionalDamageArcanistMark",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcanistMark").ToString(),
                Common.common_no_title,
                Common.common_no_title,
                null,
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                a =>
                {
                    a.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
                    a.SetSpecificDamageType("DamageForce");
                    a.SetRequiredTargetCondition(marked_condition);
                    a.SetDamageDiceNumber(1);
                    a.SetDamageDieType(RuleDefinitions.DieType.D8);
                    a.SetNotificationTag("ArcanistMark");
                    a.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe);
                    a.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
                    a.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
                    a.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
                }
            );

            return Helpers.FeatureSetBuilder.createFeatureSet
            (
                "ArcanistMark",
                GuidHelper.Create(RA_BASE_GUID, "ArcanistMark").ToString(),
                "Feature/&ArcanistMarkTitle",
                "Feature/&ArcanistMarkDescription",
                false,
                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                false,
                mark_on_hit, mark_action, mark_apply, mark_damage
            );
        }
    }
}