using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolastaModApi;
using SolastaModApi.Extensions;

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
                    .AddFeatureAtLevel(ArcanistMarkPowerBuilder.ArcanistMarkPower, 3)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes.Subclasses.Add(definition.Name);
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
            Definition.SetCostPerUse(1);
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

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();
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
