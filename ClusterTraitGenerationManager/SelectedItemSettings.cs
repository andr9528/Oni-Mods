﻿using Database;
using Epic.OnlineServices.Sessions;
using Klei.AI;
using KMod;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.CATEGORIES.FOOTERCONTENT;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER.BUTTONS;
using static STRINGS.UI.BUILDCATEGORIES;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN;

namespace ClusterTraitGenerationManager
{
    [Obsolete]
    internal class SelectedItemSettingsOLD : FScreen
    {
        private FToggle2 StarmapItemEnabled;

        private FSlider NumberToGenerate;

        private FSlider MinimumDistance;

        private FSlider MaximumDistance;

        private FSlider BufferDistance;

        private FSlider ClusterSize;

        private GameObject AsteroidSize;
        private LocText AsteroidSizeLabel;
        private ToolTip AsteroidSizeTooltip;

        private FInputField2 PlanetSizeWidth;
        private FInputField2 PlanetSizeHeight;

        private FCycle PlanetSizeCycle;
        private FCycle PlanetRazioCycle;

        private GameObject MeteorSelector;
        //private FCycle PlanetMeteorTypes;
        private GameObject ActiveMeteorsContainer;
        private GameObject MeteorPrefab;
        private GameObject ActiveSeasonsContainer;
        private GameObject SeasonPrefab;
        public FButton AddSeasonButton;


        private GameObject AsteroidTraits;
        private GameObject ActiveTraitsContainer;
        private GameObject TraitPrefab;
        public FButton AddTraitButton;


        public static FButton ResetButton;
        public static FButton ResetAllButton;
        public FButton ReturnButton;
        public FButton PresetsButton;
        public FButton SettingsButton;
        public FButton GenerateClusterButton;

        public System.Action OnClose;

        private StarmapItem lastSelected;



        public void UpdateForSelected(StarmapItem SelectedPlanet)
        {
            if (!init)
            {
                Init();
            }
            if (SelectedPlanet == null) return;
            if(lastSelected!=SelectedPlanet)
            {
                overrideWholeNumbers = false; 
            }


            lastSelected = SelectedPlanet;
            bool isPoi = SelectedPlanet.category == StarmapItemCategory.POI;


            bool IsPartOfCluster = CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current);

            StarmapItemEnabled.SetOn(IsPartOfCluster);

            bool isRandom = current.id.Contains(CGSMClusterManager.RandomKey);

            bool canGenerateMultiple = current.MaxNumberOfInstances > 1;
            NumberToGenerate.transform.parent.gameObject.SetActive(canGenerateMultiple);///Amount, only on poi / random planets
            if (canGenerateMultiple)
            {
                NumberToGenerate.SetWholeNumbers(!isPoi || overrideWholeNumbers);
                NumberToGenerate.SetMinMaxCurrent(0, current.MaxNumberOfInstances, current.InstancesToSpawn);
                NumberToGenerate.SetInteractable(IsPartOfCluster);     
                current.SetSpawnNumber(NumberToGenerate.Value);
            }

            MinimumDistance.SetMinMaxCurrent(0, CustomCluster.Rings, current.minRing);
            MinimumDistance.SetInteractable(IsPartOfCluster);

            MaximumDistance.SetMinMaxCurrent(0, CustomCluster.Rings, current.maxRing);
            MaximumDistance.SetInteractable(IsPartOfCluster);

            BufferDistance.SetMinMaxCurrent(0, CustomCluster.Rings, SelectedPlanet.buffer);
            BufferDistance.transform.parent.gameObject.SetActive(!isPoi);
            BufferDistance.SetInteractable(IsPartOfCluster);

            ClusterSize.SetMinMaxCurrent(ringMin, ringMax, CustomCluster.Rings);

            AddTraitButton.SetInteractable(IsPartOfCluster && !isRandom);
            AddSeasonButton.SetInteractable(IsPartOfCluster && !isRandom);

            AsteroidSize.SetActive(!isPoi && !isRandom);
            MeteorSelector.SetActive(!isPoi && !isRandom);
            AsteroidTraits.SetActive(!isPoi && !isRandom);

            UpdateSizeLabels(current);
            PlanetSizeCycle.Value = current.CurrentSizePreset.ToString();
            PlanetRazioCycle.Value = current.CurrentRatioPreset.ToString();

            if (isPoi) return;

            foreach (var traitContainer in Traits.Values)
            {
                traitContainer.SetActive(false);
            }
            foreach (var activeTrait in lastSelected.CurrentTraits)
            {
                Traits[activeTrait].SetActive(true);
            }

            foreach (var showerHolder in ShowerTypes.Values)
            {
                showerHolder.SetActive(false);
            }
            foreach (var activeShower in lastSelected.CurrentMeteorShowerTypes)
            {
                if(ShowerTypes.ContainsKey(activeShower.Id))
                    ShowerTypes[activeShower.Id].SetActive(true);
            }

            foreach (var seasonHolder in SeasonTypes.Values)
            {
                seasonHolder.SetActive(false);
            }
            foreach (var activeSeason in lastSelected.CurrentMeteorSeasons)
            {
                if (SeasonTypes.ContainsKey(activeSeason.Id))
                    SeasonTypes[activeSeason.Id].SetActive(true);
            }
        }

        string Warning3 = "EC1802";
        string Warning2 = ("ff8102");
        string Warning1 = ("F6D42A");

        public void UpdateSizeLabels(StarmapItem current)
        {
            PlanetSizeWidth.EditTextFromData(current.CustomPlanetDimensions.X.ToString());
            PlanetSizeHeight.EditTextFromData(current.CustomPlanetDimensions.Y.ToString());
            PercentageLargerThanTerra(current, out var Percentage);
            if (Percentage > 200)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning3);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning3));
            }
            else if (Percentage > 100)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning2);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning2));
            }
            else if (Percentage > 33)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning1);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning1));
            }
            else
            {
                AsteroidSizeLabel.text = ASTEROIDSIZE.DESCRIPTOR.LABEL;
                AsteroidSizeTooltip.SetSimpleTooltip(ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);
            }
        }
        void PercentageLargerThanTerra(StarmapItem current, out int dimensions)
        {
            float TerraArea = 240 * 380;
            float CustomSize = current.CustomPlanetDimensions.X * current.CustomPlanetDimensions.Y;

            dimensions = Mathf.RoundToInt((CustomSize / TerraArea) * 100f);
            dimensions -= 100;
        }

        public void UpdateUI()
        {
            if (lastSelected != null)
            {
                UpdateForSelected(lastSelected);
            }

            UiRefresh.Invoke();
        }

        public static void SetResetButtonStates()
        {
            if(ResetButton!=null)
                ResetButton.SetInteractable(!PresetApplied);
            if(ResetAllButton != null)
                ResetAllButton.SetInteractable(!PresetApplied);
        }

        private static bool _presetApplied = false;
        public static bool PresetApplied 
        {
            get 
            {
                return _presetApplied;
            }
            set
            {
                _presetApplied = value;
                SetResetButtonStates();
            }
        }

        public System.Action UiRefresh;


        private bool init;


        public override void OnKeyDown(KButtonEvent e)
        {

            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                //SgtLogger.l("CONSUMING1");
                if (TraitSelectorScreen.Instance != null ? !TraitSelectorScreen.Instance.IsCurrentlyActive : true && SeasonSelectorScreen.Instance != null ? !SeasonSelectorScreen.Instance.IsCurrentlyActive : true)
                    OnClose.Invoke();
            }
            if (e.TryConsume(Action.DragStraight))
            {
                if(lastSelected!=null)
                {
                    overrideWholeNumbers = !overrideWholeNumbers;
                    UpdateUI();
                }
            }


            base.OnKeyDown(e);
        }
        bool overrideWholeNumbers = false;


        private void Init()
        {


            StarmapItemEnabled = transform.Find("StarmapItemEnabled").FindOrAddComponent<FToggle2>();
            StarmapItemEnabled.SetCheckmark("Background/Checkmark");
            StarmapItemEnabled.OnClick += () =>
                {
                    if (lastSelected != null)
                    {
                        CGSMClusterManager.TogglePlanetoid(lastSelected);
                        UpdateUI();
                    }
                };

            UIUtils.AddSimpleTooltipToObject(StarmapItemEnabled.transform, STARMAPITEMENABLED.TOOLTIP);

            NumberToGenerate = transform.Find("AmountSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(NumberToGenerate.transform, "Descriptor/Input");
            //UIUtils.ListAllChildrenWithComponents(NumberToGenerate.transform);
            NumberToGenerate.SetWholeNumbers(true);
            NumberToGenerate.AttachOutputField(transform.Find("AmountSlider/Descriptor/Output").GetComponent<LocText>());
            NumberToGenerate.OnChange += (value) =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetSpawnNumber(value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(NumberToGenerate.transform.parent.Find("Descriptor"), AMOUNTSLIDER.DESCRIPTOR.TOOLTIP);

            MinimumDistance = transform.Find("MinDistanceSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(MinimumDistance.transform, "Descriptor/Input");
            MinimumDistance.SetWholeNumbers(true);
            MinimumDistance.AttachOutputField(transform.Find("MinDistanceSlider/Descriptor/Output").GetComponent<LocText>());
            MinimumDistance.OnChange += (value) =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetInnerRing((int)value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(MinimumDistance.transform.parent.Find("Descriptor"), MINMAXDISTANCE.DESCRIPTOR.TOOLTIP);


            MaximumDistance = transform.Find("MaxDistanceSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(MaximumDistance.transform, "Descriptor/Input");
            MaximumDistance.SetWholeNumbers(true);
            MaximumDistance.AttachOutputField(transform.Find("MaxDistanceSlider/Descriptor/Output").GetComponent<LocText>());
            MaximumDistance.OnChange += (value) =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetOuterRing((int)value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(MaximumDistance.transform.parent.Find("Descriptor"), MINMAXDISTANCE.DESCRIPTOR.TOOLTIP);


            BufferDistance = transform.Find("BufferSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(BufferDistance.transform, "Descriptor/Input");
            BufferDistance.SetWholeNumbers(true);
            BufferDistance.AttachOutputField(transform.Find("BufferSlider/Descriptor/Output").GetComponent<LocText>());
            BufferDistance.OnChange += (value) =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetBuffer((int)value);
                }
            };
            UIUtils.AddSimpleTooltipToObject(BufferDistance.transform.parent.Find("Descriptor"), BUFFERSLIDER.DESCRIPTOR.TOOLTIP);

            ClusterSize = transform.Find("ClusterSize/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(ClusterSize.transform, "Descriptor/Input");
            ClusterSize.SetWholeNumbers(true);
            ClusterSize.AttachOutputField(transform.Find("ClusterSize/Descriptor/Output").GetComponent<LocText>());
            ClusterSize.OnChange += (value) =>
            {
                CustomCluster.SetRings((int)value);
                UpdateUI();
            };
            //UIUtils.AddSimpleTooltipToObject(ClusterSize.transform.parent.Find("Descriptor"), CLUSTERSIZE.DESCRIPTOR.TOOLTIP);


            AsteroidSize = transform.Find("AsteroidSizeCycle").gameObject;
            AsteroidSizeLabel = AsteroidSize.transform.Find("Label").GetComponent<LocText>();
            AsteroidSizeTooltip = UIUtils.AddSimpleTooltipToObject(AsteroidSize.transform.Find("Label"), ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);
           // UIUtils.TryChangeText(AsteroidSize.transform, "Info/WidthLabel/Label", ASTEROIDSIZEINFO.WIDTH);
            //UIUtils.TryChangeText(AsteroidSize.transform, "Info/HeightLabel/Label", ASTEROIDSIZEINFO.HEIGHT);

            PlanetSizeWidth = AsteroidSize.transform.Find("Info/WidthLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeWidth.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, false));

            PlanetSizeHeight = AsteroidSize.transform.Find("Info/HeightLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeHeight.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, true));

            PlanetSizeCycle = transform.Find("AsteroidSizeCycle/SizeCycle").gameObject.AddOrGet<FCycle>();
            PlanetSizeCycle.Initialize(
                PlanetSizeCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());



            PlanetSizeCycle.Options = new List<FCycle.Option>()
            {
                new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3TOOLTIP),

                new FCycle.Option(WorldSizePresets.Normal.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE0, ASTEROIDSIZE.SIZESELECTOR.SIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlyLarger.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE1, ASTEROIDSIZE.SIZESELECTOR.SIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Large.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE2, ASTEROIDSIZE.SIZESELECTOR.SIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.Huge.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE3, ASTEROIDSIZE.SIZESELECTOR.SIZE3TOOLTIP),
                new FCycle.Option(WorldSizePresets.Massive.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE4, ASTEROIDSIZE.SIZESELECTOR.SIZE4TOOLTIP),
                new FCycle.Option(WorldSizePresets.Enormous.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE5, ASTEROIDSIZE.SIZESELECTOR.SIZE5TOOLTIP),
            };

            PlanetSizeCycle.OnChange += () =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                    {
                        WorldSizePresets setTo = Enum.TryParse<WorldSizePresets>(PlanetSizeCycle.Value, out var result) ? result : WorldSizePresets.Normal;
                        current.SetPlanetSizeToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            PlanetRazioCycle = transform.Find("AsteroidSizeCycle/RazioCycle").gameObject.AddOrGet<FCycle>();
            PlanetRazioCycle.Initialize(
                PlanetRazioCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetRazioCycle.Options = new List<FCycle.Option>()
            {
                //new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2TOOLTIP),
                //new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3TOOLTIP),
                
                new FCycle.Option(WorldRatioPresets.LotWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE3, ASTEROIDSIZE.RATIOSELECTOR.WIDE3TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Wider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE2, ASTEROIDSIZE.RATIOSELECTOR.WIDE2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE1, ASTEROIDSIZE.RATIOSELECTOR.WIDE1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Normal.ToString(), ASTEROIDSIZE.RATIOSELECTOR.NORMAL, ASTEROIDSIZE.RATIOSELECTOR.NORMALTOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Taller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.LotTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3TOOLTIP),
            };
            PlanetRazioCycle.Value = WorldRatioPresets.Normal.ToString();

            PlanetRazioCycle.OnChange += () =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                    {
                        WorldRatioPresets setTo = Enum.TryParse<WorldRatioPresets>(PlanetRazioCycle.Value, out var result) ? result : WorldRatioPresets.Normal;
                        current.SetPlanetRatioToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            MeteorSelector = transform.Find("MeteorSeasonCycle").gameObject;
            ActiveMeteorsContainer = transform.Find("MeteorSeasonCycle/Asteroids/ScrollArea/Content").gameObject;
            MeteorPrefab = transform.Find("MeteorSeasonCycle/Asteroids/ScrollArea/Content/ListViewEntryPrefab").gameObject;

            ActiveSeasonsContainer = transform.Find("MeteorSeasonCycle/Seasons/SeasonScrollArea/Content").gameObject;
            SeasonPrefab = transform.Find("MeteorSeasonCycle/Seasons/SeasonScrollArea/Content/ListViewEntryPrefab").gameObject;

            AddSeasonButton = transform.Find("MeteorSeasonCycle/Seasons/SeasonScrollArea/Content/AddSeasonButton").FindOrAddComponent<FButton>();
            ///UIUtils.AddSimpleTooltipToObject(AddSeasonButton.transform, METEORSEASONCYCLE.DESCRIPTOR.ADDNEWSEASONTOOLTIP);

            AddSeasonButton.OnClick += () =>
            {
                SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
            };

            AddTraitButton = transform.Find("AsteroidTraits/AddTraitButton").FindOrAddComponent<FButton>();

            AddTraitButton.OnClick += () =>
            {
                TraitSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
            };

            UIUtils.AddSimpleTooltipToObject(MeteorSelector.transform.Find("Title"), METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);

            AsteroidTraits = transform.Find("AsteroidTraits").gameObject;
            ActiveTraitsContainer = transform.Find("AsteroidTraits/ListView/Content").gameObject;
            TraitPrefab = transform.Find("AsteroidTraits/ListView/Content/ListViewEntryPrefab").gameObject;





            ReturnButton = transform.Find("Buttons/ReturnButton").FindOrAddComponent<FButton>();
            ReturnButton.OnClick += OnClose;

            GenerateClusterButton = transform.Find("Buttons/GenerateClusterButton").FindOrAddComponent<FButton>();
            GenerateClusterButton.OnClick += () => CGSMClusterManager.InitializeGeneration();

            ResetButton = transform.Find("Buttons/ResetSelectionButton").FindOrAddComponent<FButton>();
            ResetButton.OnClick += () =>
            {
                CGSMClusterManager.ResetPlanetFromPreset(lastSelected.id);
                UpdateUI();
            };

            ResetAllButton = transform.Find("Buttons/ResetClusterButton").FindOrAddComponent<FButton>();
            ResetAllButton.OnClick += () =>
            {
                CGSMClusterManager.ResetToLastPreset();
                UpdateUI();
            };

            PresetsButton = transform.Find("Buttons/PresetButton").FindOrAddComponent<FButton>();
            PresetsButton.OnClick += () =>
            {
                CGSMClusterManager.OpenPresetWindow(()=> UpdateUI());
            };

            SettingsButton = transform.Find("Buttons/SettingsButton").FindOrAddComponent<FButton>();
            SettingsButton.OnClick += () =>
            {
                CustomSettingsController.ShowWindow(() => UpdateUI());
            };





            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, RESETCLUSTERBUTTON.TOOLTIP, true, onBottom:true);
            UIUtils.AddSimpleTooltipToObject(ResetButton.transform, RESETSELECTIONBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(GenerateClusterButton.transform, GENERATECLUSTERBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(ReturnButton.transform, RETURNBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(SettingsButton.transform, GAMESETTINGS.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(PresetsButton.transform, PRESETBUTTON.TOOLTIP, true, onBottom: true);

            SgtLogger.Assert("AsteroidSize", AsteroidSize);
            //SgtLogger.Assert("AsteroidSizeLabel", AsteroidSizeLabel);
            SgtLogger.Assert("AsteroidTraits", AsteroidTraits);
            SgtLogger.Assert("ActiveTraitsContainer", ActiveTraitsContainer);
            SgtLogger.Assert("TraitPrefab", TraitPrefab);

            InitializeTraitContainer();
            InitializeMeteorShowerContainers();
            init = true;
        }

        void TryApplyingCoordinates(string msg, bool Height)
        {
            if (int.TryParse(msg, out var size))
            {
                if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                {
                    if (size == (Height ? current.CustomPlanetDimensions.Y : current.CustomPlanetDimensions.X))
                        return;

                    current.ApplyCustomDimension(size, Height);
                    UpdateSizeLabels(current);
                    //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                }
            }

        }

        void InitializeMeteorShowerContainers()
        {
            //foreach(var planet in SettingsCache.worlds.worldCache)
            //{
            //    SgtLogger.l("", "-");
            //    foreach (var season in planet.Value.seasons)
            //    {
            //        SgtLogger.l(season, planet.Key);
            //    }
            //}

            ///SeasonContainer
            foreach (var gameplaySeason in Db.Get().GameplaySeasons.resources)
            {
                if (!(gameplaySeason is MeteorShowerSeason) || gameplaySeason.Id.Contains("Fullerene") || gameplaySeason.Id.Contains("TemporalTear") || gameplaySeason.dlcId != DlcManager.EXPANSION1_ID)
                    continue;
                var meteorSeason = gameplaySeason as MeteorShowerSeason;

                var seasonInstanceHolder = Util.KInstantiateUI(SeasonPrefab, ActiveSeasonsContainer, true);


                string name = meteorSeason.Name.Replace("MeteorShowers", string.Empty);
                ///string description = meteorSeason.events.Count == 0 ? METEORSEASONCYCLE.DESCRIPTOR.SEASONSELECTOR.SEASONTYPENOMETEORSTOOLTIP : METEORSEASON.SEASONSELECTOR.SEASONTYPETOOLTIP;
                
                
                // var icon = showerInstanceHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                // icon.sprite = Def.GetUISprite(Assets.GetPrefab(ClusterEventID)).first;

                foreach (var meteorShower in meteorSeason.events)
                {
                    ///description += "\n • ";
                    ///description += Assets.GetPrefab((meteorShower as MeteorShowerEvent).clusterMapMeteorShowerID).GetProperName();// Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                ///UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform, description);
                var LocTextName = seasonInstanceHolder.transform.Find("Label").GetComponent<LocText>();
                LocTextName.fontSizeMax = LocTextName.fontSize;
                LocTextName.fontSizeMin = LocTextName.fontSize-6f;
                LocTextName.enableAutoSizing= true;
                UIUtils.TryChangeText(seasonInstanceHolder.transform, "Label", name);
                ///UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform.Find("Label"), description);


                var RemoveButton = seasonInstanceHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                var SwitchButton = seasonInstanceHolder.transform.Find("SwitchButton").gameObject.FindOrAddComponent<FButton>();
                ///UIUtils.AddSimpleTooltipToObject(SwitchButton.transform, METEORSEASON.SWITCHTOOTHERSEASONTOOLTIP);
                ///UIUtils.AddSimpleTooltipToObject(RemoveButton.transform, METEORSEASON.REMOVESEASONTOOLTIP);


                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var item))
                    {
                        item.RemoveMeteorSeason(meteorSeason.Id); //SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
                    }
                    UpdateUI();
                };
                SwitchButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var item))
                    {
                        SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI(), meteorSeason.Id);
                    }
                };
                SeasonTypes[gameplaySeason.Id] = seasonInstanceHolder;
            }

            ///Shower Container
            foreach (var gameplayEvent in Db.Get().GameplayEvents.resources)
            {
                if (!(gameplayEvent is MeteorShowerEvent) || gameplayEvent.Id.Contains("Fullerene") )
                    continue;
                var meteorEvent = gameplayEvent as MeteorShowerEvent;
                string ClusterEventID = meteorEvent.clusterMapMeteorShowerID;

                ///for those pesky vanilla meteors without starmap entity
                if (ClusterEventID == null || ClusterEventID == string.Empty)
                {
                    continue;
                    string TypeOfEvent = meteorEvent.Id.Replace("MeteorShower", string.Empty).Replace("Event", string.Empty);
                    ClusterEventID = ClusterMapMeteorShowerConfig.GetFullID(TypeOfEvent);
                }

                var ClusterMapShower = Assets.GetPrefab(ClusterEventID);
                var showerInstanceHolder = Util.KInstantiateUI(MeteorPrefab, ActiveMeteorsContainer, true);


                string name = ClusterMapShower.GetProperName();
                ///string description = METEORSEASON.SHOWERTOOLTIP;
                var icon = showerInstanceHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = Def.GetUISprite(Assets.GetPrefab(ClusterEventID)).first;

                var meteortypes = meteorEvent.GetMeteorsInfo();
                foreach (var meteor in meteortypes)
                {
                    ///description += "\n • ";
                    /// description += Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                //icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                //if (kvp.Key.Contains(SpritePatch.randomTraitsTraitIcon))
                //{
                //    name = UIUtils.RainbowColorText(name.ToString());
                //}
                UIUtils.TryChangeText(showerInstanceHolder.transform, "Label", name);
                /// UIUtils.AddSimpleTooltipToObject(showerInstanceHolder.transform, description);


                ShowerTypes[gameplayEvent.Id] = showerInstanceHolder;
            }
            UpdateUI();
        }
        Dictionary<string, GameObject> SeasonTypes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> ShowerTypes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
        void InitializeTraitContainer()
        {
            foreach (var kvp in ModAssets.AllTraitsWithRandom)
            {
                var TraitHolder = Util.KInstantiateUI(TraitPrefab, ActiveTraitsContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var RemoveButton = TraitHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);
                var combined = UIUtils.ColorText(name.ToString(), kvp.Value.colorHex);

                //string associatedIcon = kvp.Value.filePath.Substring(kvp.Value.filePath.LastIndexOf("/") + 1);

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                if (kvp.Key.Contains(SpritePatch.randomTraitsTraitIcon))
                {
                    combined = UIUtils.RainbowColorText(name.ToString());
                }

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                //
                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var item))
                    {
                        item.RemoveWorldTrait(kvp.Value);
                    }
                    UpdateUI();
                };
                Traits[kvp.Value.filePath] = TraitHolder;
            }
            UpdateUI();
        }


        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
        }
    }
}
