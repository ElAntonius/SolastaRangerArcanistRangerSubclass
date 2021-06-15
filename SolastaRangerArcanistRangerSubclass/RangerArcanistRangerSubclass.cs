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
        const string RangerArcanistRangerSubclassNameGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";
        
        public static void BuildAndAddSubclass()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&RangerArcanistRangerSubclassDescription",
                    "Subclass/&RangerArcanistRangerSubclassTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialSpellblade.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(RangerArcanistRangerSubclassName, RangerArcanistRangerSubclassNameGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(ArcanistMarkPowerBuilder.ArcanistMarkPower, 3)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes.Subclasses.Add(definition.Name);
        }
    }

    internal class ArcanistMarkPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string ArcanistMarkPowerName = "ArcanistMarkPower";
        const string ArcanistMarkPowerNameGuid = "E2C7DBDD-5218-419E-89FE-5457FBCD7A0C";

        protected ArcanistMarkPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ArcanistMarkPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkPowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.OnAttackHit);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            Definition.SetCostPerUse(1);
            Definition.SetShortTitleOverride("Feature/&ArcanistMarkPowerTitle");

            //Create the Mark effect
            EffectForm markEffect = new EffectForm();
            markEffect.ConditionForm = new ConditionForm();
            markEffect.FormType = EffectForm.EffectFormType.Condition;
            markEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            markEffect.ConditionForm.ConditionDefinition = ArcanistMarkConditionBuilder.ArcanistMarkCondition;

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(markEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
            newEffectDescription.DurationParameter = 1;
            //newEffectDescription.

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower ArcanistMarkPower = CreateAndAddToDB(ArcanistMarkPowerName, ArcanistMarkPowerNameGuid);
    }

    internal class ArcanistMarkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        const string ArcanistMarkConditionName = "ArcanistMarkCondition";
        const string ArcanistMarkConditionNameGuid = "944DE98F-1CFD-4DEA-BE3E-CECDC98B7C35";

        protected ArcanistMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ArcanistMarkConditionTitle";
            Definition.GuiPresentation.Description = "Feature/&ArcanistMarkConditionDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetDurationParameter(1);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new ArcanistMarkConditionBuilder(name, guid).AddToDB();

        public static ConditionDefinition ArcanistMarkCondition = CreateAndAddToDB(ArcanistMarkConditionName, ArcanistMarkConditionNameGuid);
    }

    internal class ArcanistMarkEffectBuilder : BaseDefinitionBuilder<EffectDescription>
    {
        const string ArcanistMarkEffectName = "ArcanistMarkEffect";
        const string ArcanistMarkEffectNameGuid =
    }
}
