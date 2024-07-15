﻿using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
    internal class AutomatedBrokenRebuild : KMonoBehaviour
    {
        [MyCmpGet]
        Reconstructable reconstructable;
        [MyCmpGet]
        PrimaryElement primaryElement;
        [MyCmpGet]
        BuildingHP hp;

        [Serialize]
        public bool RebuildOnBreaking = false;

        private static readonly EventSystem.IntraObjectHandler<AutomatedBrokenRebuild> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AutomatedBrokenRebuild>((component, data) => component.OnCopySettings(data));

        public bool IsValidForRebuilding => CanRebuild();

        bool CanRebuild()
        {
            if(reconstructable == null)
                return false;

            if(reconstructable.building.Def.Invincible)
                return false;

            if(!reconstructable.deconstructable.allowDeconstruction)
                return false;

            if(hp == null || hp.destroyOnDamaged)
                return false;

            return true;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (reconstructable == null)
            {
                this.enabled = false;
                return;
            }
            if (CanRebuild())
                Subscribe((int)GameHashes.BuildingBroken, OnBuildingBroken);
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            if (CanRebuild())
                Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            if (!CanRebuild())
            {
                return;
            }
            Unsubscribe((int) GameHashes.CopySettings, OnCopySettingsDelegate);
            Unsubscribe((int) GameHashes.BuildingBroken, OnBuildingBroken);
        }

        public void OnBuildingBroken(object data)
        {
            if(RebuildOnBreaking && reconstructable !=null)
            {
                reconstructable.RequestReconstruct(primaryElement.Element.tag);
            }
        }

        public void OnCopySettings(object data)
        {
            if (data is GameObject sauceGameObject && sauceGameObject.TryGetComponent<AutomatedBrokenRebuild>(out var addon))
            {
                this.RebuildOnBreaking = addon.RebuildOnBreaking;
            }
        }
    }
}
