using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModHelpers;

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
                    .AddFeatureAtLevel(createArcanistBonusSpells(), 3)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes.Subclasses.Add(definition.Name);
        }

        static FeatureDefinition createArcanistBonusSpells()
        {
            var bonus_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.FeatureDefinitionExtraSpellsKnown>.createFeature
                               (
                                   "RangerArcanistBonusSpells",
                                   GuidHelper.Create(RA_BASE_GUID, "RangerArcanistBonusSpells").ToString(),
                                   Common.common_no_title,
                                   Common.common_no_title,
                                   null,
                                   b =>
                                   {
                                       b.caster_class = DatabaseHelper.CharacterClassDefinitions.Ranger;
                                       b.level = 3;
                                       b.max_spells = 2;
                                   }
                               );


            return Helpers.FeatureBuilder<NewFeatureDefinitions.GrantSpells>.createFeature
            (
                "RangerArcanistBonusSpellsFeature",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistBonusSpellFeature").ToString(),
                "Feature/&RangerArcanistBonusSpellsTitle",
                "Feature/&RangerArcanistBonusSpellsDescription",
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
                                DatabaseHelper.SpellDefinitions.Shield
                            }
                        }

                    };
                }
            );
        }
    }
}
    /*

    internal class RangerArcanistFastSpellPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string RangerArcanistFastSpellPowerName = "RangerArcanistFastSpellPower";

        protected RangerArcanistFastSpellPowerBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RangerArcanistFastSpellPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistFastSpellPowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            Definition.SetUniqueInstance(true);
            Definition.SetCostPerUse(1);

            EffectForm fastSpellEffect = new EffectForm();
            fastSpellEffect.ConditionForm = new ConditionForm();
            fastSpellEffect.FormType = EffectForm.EffectFormType.Condition;
            fastSpellEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            fastSpellEffect.ConditionForm.ConditionDefinition = RangerArcanistFastSpellConditionBuilder.RangerArcanistFastSpellCondition;

            EffectDescriptionBuilder fastSpellEffectDescriptionBuilder = new EffectDescriptionBuilder();
            fastSpellEffectDescriptionBuilder.SetCreatedByCharacter();
            fastSpellEffectDescriptionBuilder.SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            fastSpellEffectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 0, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Weapon);
            fastSpellEffectDescriptionBuilder.AddEffectForm(fastSpellEffect);

            Definition.SetEffectDescription(fastSpellEffectDescriptionBuilder.Build());
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new RangerArcanistFastSpellPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower RangerArcanistFastSpellPower = CreateAndAddToDB(RangerArcanistFastSpellPowerName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistFastSpellPowerName).ToString());
    }

    internal class RangerArcanistFastSpellConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        const string RangerArcanistFastSpellConditionName = "RangerArcanistFastSpellCondition";

        protected RangerArcanistFastSpellConditionBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&RangerArcanistFastSpellConditionTitle";
            Definition.GuiPresentation.Description = "Condition/&RangerArcanistFastSpellConditionDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(1);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.Features.Add(RangerArcanistFastSpellActionBuilder.RangerArcanistFastSpellAction);

        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new RangerArcanistFastSpellConditionBuilder(name, guid).AddToDB();

        public static ConditionDefinition RangerArcanistFastSpellCondition = CreateAndAddToDB(RangerArcanistFastSpellConditionName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistFastSpellConditionName).ToString());
    }

    internal class RangerArcanistFastSpellActionBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalAction>
    {
        const string RangerArcanistFastSpellActionName = "RangerArcanistFastSpellAction";

        protected RangerArcanistFastSpellActionBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RangerArcanistFastSpellActionTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistFastSpellActionDescription";

            Definition.SetActionType(ActionDefinitions.ActionType.Bonus);
            Definition.AuthorizedActions.Add(ActionDefinitions.Id.CastBonus);
        }

        public static FeatureDefinitionAdditionalAction CreateAndAddToDB(string name, string guid)
            => new RangerArcanistFastSpellActionBuilder(name, guid).AddToDB();

        public static FeatureDefinitionAdditionalAction RangerArcanistFastSpellAction = CreateAndAddToDB(RangerArcanistFastSpellActionName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistFastSpellActionName).ToString());
    }

    internal class RangerArcanistCastSpellFeatureBuilder : BaseDefinitionBuilder<FeatureDefinitionCastSpell>
    {
        const string RangerArcanistCastSpellFeatureName = "RangerArcanistCastSpellFeature";

        protected RangerArcanistCastSpellFeatureBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionCastSpells.CastSpellRanger, name, guid)
        {
            Definition.GuiPresentation.Title       = "Feature/&RangerArcanistCastSpellFeatureTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistCastSpellFeatureDescription";

            Definition.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass);
            /*Definition.SpellListDefinition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 1,
                Spells = new List<SpellDefinition>
                {
                    ArcanistMarkSpellBuilder.ArcanistMarkSpell,
                    DatabaseHelper.SpellDefinitions.Shield
                }
            });
            Definition.SetSpellListDefinition(RangerArcanistSpellListBuilder.RangerArcanistSpellList);
            Definition.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            Definition.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
        }

        public static FeatureDefinitionCastSpell CreateAndAddToDB(string name, string guid)
           => new RangerArcanistCastSpellFeatureBuilder(name, guid).AddToDB();
        //public static FeatureDefinitionCastSpell CreateAndAddToDB(CharacterClassDefinition characterClass)
        //       => new RangerArcanistCastSpellFeatureBuilder(characterClass, RangerArcanistCastSpellFeatureName + characterClass.Name, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistCastSpellFeatureName + characterClass.Name).ToString()).AddToDB();

        public static FeatureDefinitionCastSpell RangerArcanistCastSpellFeature = CreateAndAddToDB(RangerArcanistCastSpellFeatureName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistCastSpellFeatureName).ToString());
        //public static FeatureDefinitionCastSpell GetOrAdd(CharacterClassDefinition characterClass)
        //{
        //    var db = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>();
        //    return db.TryGetElement(RangerArcanistCastSpellFeatureName + characterClass.Name, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistCastSpellFeatureName + characterClass.Name).ToString()) ?? CreateAndAddToDB(characterClass);
        //}
    }

    internal class RangerArcanistAutoPreparedSpellsFeatureBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        const string RangerArcanistAutoPreparedSpellsFeatureName = "RangerArcanistAutoPreparedSpellsFeature";

        protected RangerArcanistAutoPreparedSpellsFeatureBuilder(CharacterClassDefinition characterClass, string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RangerArcanistAutoPreparedSpellsFeatureTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistAutoPreparedSpellsFeatureDescription";

            Definition.SetSpellcastingClass(characterClass);
            Definition.AutoPreparedSpellsGroups.Clear();
            Definition.AutoPreparedSpellsGroups.Add(new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 0,
                SpellsList = new List<SpellDefinition>
                {
                    ArcanistMarkSpellBuilder.ArcanistMarkSpell,
                    DatabaseHelper.SpellDefinitions.Shield
                }
            });
        }

        //public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
        //    => new RangerArcanistAutoPreparedSpellsFeatureBuilder(name, guid).AddToDB();

        public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(CharacterClassDefinition characterClass)
               => new RangerArcanistAutoPreparedSpellsFeatureBuilder(characterClass, RangerArcanistAutoPreparedSpellsFeatureName + characterClass.Name, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistAutoPreparedSpellsFeatureName + characterClass.Name).ToString()).AddToDB();
        //public static FeatureDefinitionAutoPreparedSpells RangerArcanistAutoPreparedSpellsFeature = CreateAndAddToDB(RangerArcanistAutoPreparedSpellsFeatureName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistAutoPreparedSpellsFeatureName).ToString());
        public static FeatureDefinitionAutoPreparedSpells GetOrAdd(CharacterClassDefinition characterClass)
        {
            var db = DatabaseRepository.GetDatabase<FeatureDefinitionAutoPreparedSpells>();
            return db.TryGetElement(RangerArcanistAutoPreparedSpellsFeatureName + characterClass.Name, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistAutoPreparedSpellsFeatureName + characterClass.Name).ToString()) ?? CreateAndAddToDB(characterClass);
        }
    }

    internal class RangerArcanistMagicAffinityFeatureBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
    {
        const string RangerArcanistMagicAffinityFeatureName = "RangerArcanistMagicAffinityFeature";

        protected RangerArcanistMagicAffinityFeatureBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title       = "Feature/&RangerArcanistMagicAffinityFeatureTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistMagicAffinityFeatureDescription";

            Definition.SetSomaticWithWeaponOrShield(true);
            Definition.SetExtendedSpellList(RangerArcanistSpellListBuilder.RangerArcanistSpellList);
        }

        public static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
            => new RangerArcanistMagicAffinityFeatureBuilder(name, guid).AddToDB();

        public static FeatureDefinitionMagicAffinity RangerArcanistMagicAffinityFeature = CreateAndAddToDB(RangerArcanistMagicAffinityFeatureName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistMagicAffinityFeatureName).ToString());
    }

    internal class RangerArcanistSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        const string RangerArcanistSpellListName = "RangerArcanistSpellList";

        protected RangerArcanistSpellListBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title       = "Feature/&RangerArcanistSpellListTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerArcanistSpellListDescription";

            Definition.SpellsByLevel.Clear();
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 1,
                Spells = new List<SpellDefinition>
                {
                    DatabaseHelper.SpellDefinitions.Shield
                }
            });
        }
        public static SpellListDefinition CreateAndAddToDB(string name, string guid)
            => new RangerArcanistSpellListBuilder(name, guid).AddToDB();

        public static SpellListDefinition RangerArcanistSpellList = CreateAndAddToDB(RangerArcanistSpellListName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, RangerArcanistSpellListName).ToString());
    }


    internal class ArcanistMarkSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        const string ArcanistMarkSpellName = "ArcanistMarkSpell";

        protected ArcanistMarkSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.BrandingSmite, name, guid)
        {
            Definition.GuiPresentation.Title       = "Feature/&ArcanistMarkSpellTitle"; 
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkSpellDescription";

            Definition.SetCastingTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetSchoolOfMagic("SchoolEnchantment");
            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(true);
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);

            // Create the Mark effect
            EffectForm markEffect = new EffectForm();
            markEffect.ConditionForm = new ConditionForm();
            markEffect.FormType = EffectForm.EffectFormType.Condition;
            markEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            markEffect.ConditionForm.ConditionDefinition = ArcanistMarkConditionBuilder.ArcanistMarkCondition;

            // Create a Description for the Mark Effect
            EffectDescriptionBuilder markEffectDescriptionBuilder = new EffectDescriptionBuilder();
            markEffectDescriptionBuilder.SetCreatedByCharacter();
            markEffectDescriptionBuilder.SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            markEffectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 0, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Weapon);
            markEffectDescriptionBuilder.AddEffectForm(markEffect);

            Definition.SetEffectDescription(markEffectDescriptionBuilder.Build());
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkSpellBuilder(name, guid).AddToDB();

        public static SpellDefinition ArcanistMarkSpell = CreateAndAddToDB(ArcanistMarkSpellName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, ArcanistMarkSpellName).ToString());
    }

    internal class ArcanistMarkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        const string ArcanistMarkConditionName = "ArcanistMarkCondition";

        protected ArcanistMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, guid)
        {
            Definition.GuiPresentation.Title       = "Feature/&ArcanistMarkConditionTitle";
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkConditionDescription";

            Definition.Features.Clear();
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetDurationParameter(1);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkConditionBuilder(name, guid).AddToDB();

        public static ConditionDefinition ArcanistMarkCondition = CreateAndAddToDB(ArcanistMarkConditionName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, ArcanistMarkConditionName).ToString());
    }
}


    internal class ArcanistMarkPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string ArcanistMarkPowerName = "ArcanistMarkPower";

        protected ArcanistMarkPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ArcanistMarkPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkPowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.OnAttackHit);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            Definition.SetUniqueInstance(true);
            Definition.SetCostPerUse(0);
            Definition.SetShortTitleOverride("Feature/&ArcanistMarkPowerTitle");

            // Create the Mark effect
            EffectForm markEffect = new EffectForm();
            markEffect.ConditionForm = new ConditionForm();
            markEffect.FormType = EffectForm.EffectFormType.Condition;
            markEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            markEffect.ConditionForm.ConditionDefinition = ArcanistMarkConditionBuilder.ArcanistMarkCondition;

            // Add the Damage Dice
            EffectForm markDamageEffect = new EffectForm();
            markDamageEffect.DamageForm = new DamageForm();
            markDamageEffect.DamageForm.DiceNumber = 1;
            markDamageEffect.DamageForm.DieType = RuleDefinitions.DieType.D4;
            markDamageEffect.DamageForm.DamageType = "DamageForce";
            markDamageEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            // Create a Description for the Mark Effect
            EffectDescriptionBuilder markEffectDescriptionBuilder = new EffectDescriptionBuilder();
            markEffectDescriptionBuilder.SetCreatedByCharacter();
            markEffectDescriptionBuilder.SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            markEffectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.RangeHit, 120, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.Weapon);
            markEffectDescriptionBuilder.AddEffectForm(markEffect);
            //markEffectDescriptionBuilder.AddEffectForm(markDamageEffect);

            Definition.SetEffectDescription(markEffectDescriptionBuilder.Build());
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower ArcanistMarkPower = CreateAndAddToDB(ArcanistMarkPowerName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, ArcanistMarkPowerName).ToString());
    }

    internal class ArcanistMarkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        const string ArcanistMarkConditionName = "ArcanistMarkCondition";

        protected ArcanistMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ArcanistMarkConditionTitle";
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkConditionDescription";

            Definition.Features.Clear();
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetDurationParameter(1);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
            Definition.SetAdditionalDamageWhenHit(true);
            Definition.SetAdditionalDamageType("DamageForce");
            Definition.SetAdditionalDamageDieType(RuleDefinitions.DieType.D4);
            Definition.SetAdditionalDamageDieNumber(1);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkConditionBuilder(name, guid).AddToDB();

        public static ConditionDefinition ArcanistMarkCondition = CreateAndAddToDB(ArcanistMarkConditionName, GuidHelper.Create(RangerArcanistRangerSubclass.RA_BASE_GUID, ArcanistMarkConditionName).ToString());
    }

}
    */