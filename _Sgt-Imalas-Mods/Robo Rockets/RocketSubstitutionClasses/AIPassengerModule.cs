﻿using HarmonyLib;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace RoboRockets
{
    public class AIPassengerModule : PassengerRocketModule
    {
        [Serialize]
        public bool variableSpeed = false;
        public override void OnSpawn()
        {
            base.OnSpawn();

            if(!variableSpeed)
            {
                this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, (object)Config.Instance.NoBrainRockets);
            }
        }

    }
}
