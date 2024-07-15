﻿using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SetStartDupes.STRINGS.UI.DSS_OPTIONS;
using static STRINGS.CODEX.MYLOG.BODY;

namespace SetStartDupes
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Duplicant Stat Selector")]
    class Config : SingletonOptions<Config>
    {


        [Option("STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
        [Limit(1, 100)]
        [JsonProperty]
        public int DuplicantStartAmount { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.NAME", "STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
        [JsonProperty]
        public bool StartupResources { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.NAME", "STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
        [JsonProperty]
        [Limit(0, 10)]
        public int SupportedDays { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME.NAME", "STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public bool ModifyDuringGame { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public bool RerollDuringGame { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.MORECAREPACKAGES.NAME", "STRINGS.UI.DSS_OPTIONS.MORECAREPACKAGES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public bool AddAdditionalCarePackages { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.NAME", "STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public float PrintingPodRechargeTime { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIMEFIRST.NAME", "STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIMEFIRST.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public float PrintingPodRechargeTimeFirst { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.PAUSEONREADYTOPRING.NAME", "STRINGS.UI.DSS_OPTIONS.PAUSEONREADYTOPRING.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public bool PauseOnReadyToPrint { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        public bool CarePackagesOnly { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        [Limit(1, 200)]
        public int CarePackagesOnlyDupeCap { get; set; }


        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
        [JsonProperty]
        [Limit(1, 5)]
        public int CarePackagesOnlyPackageCount { get; set; }


        [Option("STRINGS.UI.DSS_OPTIONS.LIVEDUPESKINCHANGE.NAME", "STRINGS.UI.DSS_OPTIONS.LIVEDUPESKINCHANGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
        [JsonProperty]
        public bool LiveDupeSkins { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.NAME", "STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
        [JsonProperty]
        public bool SkinsDoReactions { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.LIVEDUPESTATCHANGE.NAME", "STRINGS.UI.DSS_OPTIONS.LIVEDUPESTATCHANGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.E_UTIL")]
        [JsonProperty]
        public bool DuplicityDupeEditor { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.REROLLCRYOPODANDJORGE.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLCRYOPODANDJORGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.E_UTIL")]
        [JsonProperty]
        public bool JorgeAndCryopodDupes { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.HERMITSKIN.NAME", "STRINGS.UI.DSS_OPTIONS.HERMITSKIN.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
        [JsonProperty]
        public bool HermitSkin { get; set; }


        [Option("STRINGS.UI.DSS_OPTIONS.ADDANDREMOVE.NAME", "STRINGS.UI.DSS_OPTIONS.ADDANDREMOVE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool AddAndRemoveTraitsAndInterests { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.ADDVACCILATORTRAITS.NAME", "STRINGS.UI.DSS_OPTIONS.ADDVACCILATORTRAITS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool AddVaccilatorTraits { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.INTERESTPOINTSBALANCING.NAME", "STRINGS.UI.DSS_OPTIONS.INTERESTPOINTSBALANCING.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool BalanceAddRemove { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.PRESETSOVERRIDENAME.NAME", "STRINGS.UI.DSS_OPTIONS.PRESETSOVERRIDENAME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool PresetsDoNames { get; set; } = false;
        [Option("STRINGS.UI.DSS_OPTIONS.PRESETSOVERRIDEREACTIONS.NAME", "STRINGS.UI.DSS_OPTIONS.PRESETSOVERRIDEREACTIONS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool PresetsDoReactions { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.NOJOYREACTION.NAME", "STRINGS.UI.DSS_OPTIONS.NOJOYREACTION.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool NoJoyReactions { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.NOSTRESSREACTION.NAME", "STRINGS.UI.DSS_OPTIONS.NOSTRESSREACTION.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
        [JsonProperty]
        public bool NoStressReactions { get; set; }


        public Config()
        {
            DuplicantStartAmount = 3;
            PrintingPodRechargeTime = 3;
            PrintingPodRechargeTimeFirst = 2.5f;
            ModifyDuringGame = true;
            RerollDuringGame = true;
            PauseOnReadyToPrint = false;

            StartupResources = false;
            SupportedDays = 5;

            CarePackagesOnly = false;
            CarePackagesOnlyDupeCap = 16;
            CarePackagesOnlyPackageCount = 3;

            SkinsDoReactions = true;
            JorgeAndCryopodDupes = true;
            HermitSkin = true;

            AddAndRemoveTraitsAndInterests = true;
            AddVaccilatorTraits = false;
            BalanceAddRemove = true;
            PresetsDoReactions = false;
            NoJoyReactions = false;
            NoStressReactions = false;

            AddAdditionalCarePackages = true;
            LiveDupeSkins = false;
            DuplicityDupeEditor = false;
        }
    }
}
