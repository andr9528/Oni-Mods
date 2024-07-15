﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplosiveMaterials
{
    public class BombSideScreen : KMonoBehaviour, ISidescreenButtonControl
    {
        [MyCmpReq]
        private ExplosiveBomblet bomb; 

        public string SidescreenTitle => "Nuclear Bomblet";

        public string SidescreenStatusMessage => "";

        public void OnSidescreenButtonPressed()
        {
            bomb.Detonate();
        }

        public string SidescreenButtonText => "Detonate Bomb";

        public string SidescreenButtonTooltip => "boom";

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => true;

        public int ButtonSideScreenSortOrder() => 20;
        public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

        public int HorizontalGroupID() => -1;
    }
}
