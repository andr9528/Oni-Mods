﻿using KnastoronOniMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRockets
{
    class RocketAiConditions : KMonoBehaviour
    {
        public ConditionDestinationReachable reachable;
        public ConditionAllModulesComplete allModulesComplete;
        public ConditionHasCargoBayForNoseconeHarvest HasCargoBayForNoseconeHarvest;
        public ConditionHasEngine hasEngine;
        public ConditionHasNosecone hasNosecone;
        public ConditionOnLaunchPad onLaunchPad;
        public ConditionFlightPathIsClear flightPathIsClear;
        public ConditionAiHasControl conditionAiHasControl;

        public override void OnSpawn()
        {
            base.OnSpawn();
            RocketModule component = this.GetComponent<RocketModule>();
            this.allModulesComplete = (ConditionAllModulesComplete)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new ConditionAllModulesComplete(this.GetComponent<ILaunchableRocket>()));
            this.hasEngine = (ConditionHasEngine)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new ConditionHasEngine(this.GetComponent<ILaunchableRocket>()));
            this.hasNosecone = (ConditionHasNosecone)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new ConditionHasNosecone(this.GetComponent<LaunchableRocketCluster>()));
            this.onLaunchPad = (ConditionOnLaunchPad)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new ConditionOnLaunchPad(this.GetComponent<RocketModuleCluster>().CraftInterface));
            this.flightPathIsClear = (ConditionFlightPathIsClear)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketFlight, (ProcessCondition)new ConditionFlightPathIsClear(this.gameObject, 0));
            this.conditionAiHasControl = (ConditionAiHasControl)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, (ProcessCondition)new ConditionAiHasControl(this.GetComponent<RocketModuleCluster>()));
            this.HasCargoBayForNoseconeHarvest = (ConditionHasCargoBayForNoseconeHarvest)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition)new ConditionHasCargoBayForNoseconeHarvest(this.GetComponent<LaunchableRocketCluster>()));
            this.reachable = (ConditionDestinationReachable)component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition)new ConditionDestinationReachable(this.GetComponent<RocketModule>()));
        }
    }
}
