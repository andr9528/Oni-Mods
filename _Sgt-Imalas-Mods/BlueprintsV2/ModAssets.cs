﻿using BlueprintsV2.BlueprintData;
using HarmonyLib;
using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UtilLibs;
using YamlDotNet.Serialization;
using static BlueprintsV2.STRINGS;
using static FMODUnity.FMODEventPlayableBehavior;

namespace BlueprintsV2
{
    internal class ModAssets
    {

        public static Sprite BLUEPRINTS_CREATE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_CREATE_VISUALIZER_SPRITE;

        public static Sprite BLUEPRINTS_USE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_USE_VISUALIZER_SPRITE;

        public static Sprite BLUEPRINTS_SNAPSHOT_ICON_SPRITE;
        public static Sprite BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;

        public static Color BLUEPRINTS_COLOR_VALIDPLACEMENT = Color.white;
        public static Color BLUEPRINTS_COLOR_INVALIDPLACEMENT = Color.red;
        public static Color BLUEPRINTS_COLOR_NOTECH = new Color32(30, 144, 255, 255);
        public static Color BLUEPRINTS_COLOR_NOMATERIALS = UIUtils.rgb(255, 107, 8);
        public static Color BLUEPRINTS_COLOR_NOTALLOWEDINWORLD = UIUtils.rgb(135, 97, 79);
        public static Color BLUEPRINTS_COLOR_BLUEPRINT_DRAG = new Color32(0, 119, 145, 255);

        public static HashSet<char> BLUEPRINTS_FILE_DISALLOWEDCHARACTERS;
        public static HashSet<char> BLUEPRINTS_PATH_DISALLOWEDCHARACTERS;

        public static HashSet<string> BLUEPRINTS_AUTOFILE_IGNORE = new();
        public static FileSystemWatcher BLUEPRINTS_AUTOFILE_WATCHER;

        static ModAssets()
        {
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.UnionWith(System.IO.Path.GetInvalidFileNameChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidFileNameChars());
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidPathChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('/');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('\\');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.DirectorySeparatorChar);
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.AltDirectorySeparatorChar);

        }

        public static GameObject BlueprintSelectionScreen;
        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("blueprints_ui", platformSpecific: true);
            BlueprintSelectionScreen = bundle.LoadAsset<GameObject>("Assets/UIs/blueprintSelector.prefab");

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(BlueprintSelectionScreen);
        }


        public static BlueprintFolder SelectedFolder;
        public static Blueprint SelectedBlueprint;
        public static Dictionary<BlueprintSelectedMaterial, Tag> DynamicReplacementTags = new();

        public static void RemoveReplacementTag(BlueprintSelectedMaterial tag)
        {
            DynamicReplacementTags.Remove(tag);
        }
        public static void AddOrSetReplacementTag(BlueprintSelectedMaterial tag, Tag replacement)
        {
            if(!DynamicReplacementTags.ContainsKey(tag) && replacement != tag.SelectedTag) 
                DynamicReplacementTags.Add(tag, replacement);
            else
            {
                if (replacement == tag.SelectedTag)
                    DynamicReplacementTags.Remove(tag);
                else
                    DynamicReplacementTags[tag] = replacement;
            }
        }
        public static bool HasReplacementTag(BlueprintSelectedMaterial mat) => DynamicReplacementTags.ContainsKey(mat);
        public static void ClearReplacementTags()
        {
            DynamicReplacementTags.Clear();
        }
        public static bool TryGetReplacementTag(BlueprintSelectedMaterial tag, out Tag replacement)
        {
            replacement = null;
            if (DynamicReplacementTags.ContainsKey(tag))
            {
                replacement = DynamicReplacementTags[tag];
                return true;
            }
            return false;
        }

        public static GameObject ParentScreen => PauseScreen.Instance.transform.parent.gameObject;// GameScreenManager.Instance.transform.Find("ScreenSpaceOverlayCanvas/MiddleCenter - InFrontOfEverything").gameObject;

        public static class BlueprintFileHandling
        {
            public static BlueprintFolder RootFolder;
            public static HashSet<BlueprintFolder> BlueprintFolders = new();
            public static HashSet<Blueprint> Blueprints = new();


            public static string GetBlueprintDirectory()
            {
                string folderLocation = Path.Combine(Util.RootFolder(), "blueprints");
                if (!Directory.Exists(folderLocation))
                {
                    Directory.CreateDirectory(folderLocation);
                }

                return folderLocation;
            }

            public static bool AttachFileWatcher()
            {
                string blueprintDirectory = GetBlueprintDirectory();

                ModAssets.BLUEPRINTS_AUTOFILE_WATCHER = new FileSystemWatcher
                {
                    Path = blueprintDirectory,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                    Filter = "*.*"
                };

                ModAssets.BLUEPRINTS_AUTOFILE_WATCHER.Created += (sender, eventArgs) =>
                {
                    SgtLogger.l("file watcher triggered on "+eventArgs.Name+", "+eventArgs.FullPath, "BP FileWatcher");
                    if (ModAssets.BLUEPRINTS_AUTOFILE_IGNORE.Contains(eventArgs.FullPath))
                    {
                        ModAssets.BLUEPRINTS_AUTOFILE_IGNORE.Remove(eventArgs.FullPath);
                        return;
                    }

                    if (eventArgs.FullPath.EndsWith(".blueprint") || eventArgs.FullPath.EndsWith(".json"))
                    {
                        HandleBlueprintLoading(eventArgs.FullPath);
                    }
                };

                ModAssets.BLUEPRINTS_AUTOFILE_WATCHER.EnableRaisingEvents = true;
                return false;
            }

            public static void ReloadBlueprints(bool ingame)
            {
                RootFolder = null;
                BlueprintFolders.Clear();
                Blueprints.Clear();
                LoadFolder(GetBlueprintDirectory());

                if (ingame)
                {
                    BlueprintState.ClearVisuals();
                    if(HasBlueprints())
                        BlueprintState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), SelectedBlueprint);
                }
            }

            public static void DeleteBlueprint(Blueprint bp)
            {
                bp.RemoveFromFolder();
                Blueprints.Remove(bp);
                if(SelectedBlueprint == bp)
                    SelectedBlueprint = null;
                bp.DeleteFile();
            }

            public static bool TryGetFolder(Blueprint bp, out BlueprintFolder folder)
            {
                if(RootFolder.ContainsBlueprint(bp))
                {
                    folder = RootFolder;
                    return true;
                }
                folder = BlueprintFolders.FirstOrDefault(x => x.ContainsBlueprint(bp));
                return folder != null;
            }
            public static bool TryGetFolder(string folderName, out BlueprintFolder folder)
            {
                if (folderName == null || folderName == "")
                {
                    folder = RootFolder;
                    return true;
                }
                folder = BlueprintFolders.FirstOrDefault(x => x.Name == folderName);
                return folder != null;
            }

            public static bool HasBlueprints()
            {
                if (BlueprintFolders.Count == 0 && !RootFolder.HasBlueprints)
                {
                    return false;
                }

                if (RootFolder.HasBlueprints)
                    return true;

                foreach (var blueprintFolder in BlueprintFolders)
                {
                    if (blueprintFolder.HasBlueprints)
                    {
                        return true;
                    }
                }

                return false;
            }

            public static void LoadFolder(string folder, string ParentFolder = null)
            {
                BlueprintFolder CurrentFolder;
                //root
                if (ParentFolder == null)
                {
                    CurrentFolder = new BlueprintFolder("");
                }
                else
                {
                    CurrentFolder = new BlueprintFolder(Path.GetFileName(folder));
                }
                
                string parentName = Path.GetFileName(Path.GetDirectoryName(folder));

                string[] files = Directory.GetFiles(folder);
                string[] subfolders = Directory.GetDirectories(folder);

                foreach (string file in files)
                {
                    if (file.EndsWith(".blueprint") || file.EndsWith(".json"))
                    {
                        if (LoadBlueprint(file, out Blueprint blueprint))
                        {
                            CurrentFolder.AddBlueprint(blueprint);
                        }
                    }
                }

                foreach (string subfolder in subfolders)
                {
                    LoadFolder(subfolder, folder);
                }

                if (ParentFolder == null)
                {
                    RootFolder = CurrentFolder;
                }
                else if(CurrentFolder.HasBlueprints)
                {
                    BlueprintFolders.Add(CurrentFolder);
                }
            }
            public static BlueprintFolder AddOrGetFolder(string folderName)
            {
                if(TryGetBlueprintFolder(folderName, out var folder))
                {
                    return folder;
                }
                return CreateFolder(folderName);
            }
            public static bool TryGetBlueprintFolder(string folderName, out BlueprintFolder folder)
            {
                folder = null;
                if (folderName.IsNullOrWhiteSpace())
                {
                    folder = RootFolder;
                    return true;
                }
                foreach (var f in BlueprintFolders)
                {
                    if (f.Name == folderName)
                    {
                        folder = f;
                        return true;
                    }
                }
                return false;
            }

            private static BlueprintFolder CreateFolder(string folderName)
            {
                var folder = new BlueprintFolder(folderName);
                BlueprintFolders.Add(folder);
                return folder;  
            }
            public static void HandleBlueprintLoading(string filePath)
            {
                if (LoadBlueprint(filePath, out Blueprint blueprint))
                {
                    if (blueprint.Folder == Path.GetDirectoryName(GetBlueprintDirectory()) || blueprint.Folder == string.Empty)
                    {
                        SgtLogger.l("adding to root folder", blueprint.FriendlyName);
                        RootFolder.AddBlueprint(blueprint);
                    }
                    else
                    {
                        SgtLogger.l("putting in folder", blueprint.FriendlyName);
                        var folder = BlueprintFolders.FirstOrDefault(f => f.Name == blueprint.Folder);
                        if (folder == null)
                        {
                            folder = CreateFolder(blueprint.Folder);
                        }
                        folder.AddBlueprint(blueprint);
                    }
                    BlueprintsV2.UnityUI.BlueprintSelectionScreen.RefreshOnBpAdded();
                }
                else
                    SgtLogger.warning("not a blueprint");
            }

            public static bool LoadBlueprint(string blueprintLocation, out Blueprint blueprint)
            {
                blueprint = new Blueprint(blueprintLocation);
                if (!blueprint.ReadBinary())
                {
                    blueprint.ReadJson();
                }
                return !blueprint.IsEmpty();
            }
        }

        internal static void RegisterActions()
        {
            Actions.BlueprintsCreateAction = new PActionManager().CreateAction(ActionKeys.ACTION_CREATE_KEY,
                STRINGS.UI.ACTIONS.CREATE_TITLE ,new PKeyBinding(KKeyCode.C, Modifier.Ctrl));
            Actions.BlueprintsUseAction = new PActionManager().CreateAction(ActionKeys.ACTION_USE_KEY,
                STRINGS.UI.ACTIONS.USE_TITLE, new PKeyBinding(KKeyCode.V, Modifier.Ctrl));
            Actions.BlueprintsSnapshotAction = new PActionManager().CreateAction(ActionKeys.ACTION_SNAPSHOT_KEY,
                STRINGS.UI.ACTIONS.SNAPSHOT_TITLE, new PKeyBinding(KKeyCode.X, Modifier.Ctrl));
            Actions.BlueprintsReopenSelectionAction = new PActionManager().CreateAction(ActionKeys.ACTION_RESELECT_KEY,
                STRINGS.UI.ACTIONS.SELECT_DIFFERENT_TITLE, new PKeyBinding(KKeyCode.E, Modifier.Shift));
            Actions.BlueprintsSwapAnchorAction = new PActionManager().CreateAction(ActionKeys.ACTION_SWAP_ANCHOR_KEY,
                STRINGS.UI.ACTIONS.CHANGE_ANCHOR_TITLE, new PKeyBinding(KKeyCode.R, Modifier.Shift));
            Actions.BlueprintsSwapAnchorAction = new PActionManager().CreateAction(ActionKeys.ACTION_SWAP_ANCHOR_KEY,
                STRINGS.UI.ACTIONS.ROTATE_BLUEPRINT_TITLE, new PKeyBinding(KKeyCode.E, Modifier.Shift));
        }

        internal static bool IsStaticTag(BlueprintSelectedMaterial tagMaterial, out string name, out string desc, out Sprite icon)
        {
            name = tagMaterial.CategoryTag.Name;
            desc = string.Empty;
            icon = Assets.GetSprite("unknown");
            var possibleItems = GetValidMaterials(tagMaterial.CategoryTag, false);
            
            if (possibleItems.Count<2)
            {
                //SgtLogger.l(possibleItems.Count + "", "possibruh");
                if(possibleItems.Count == 0)
                    return true;
                var tag = possibleItems.First();
                var prefab = Assets.GetPrefab(tag);
                name = prefab.GetProperName();
                desc = GameUtil.GetMaterialTooltips(tag);
                icon = Def.GetUISprite(prefab).first; 
                //SgtLogger.l($"{name}: {desc}","DESC");

                return true;
            }
            
            return false;
        }

        public static Tag GetFirstAvailableMaterial(Tag materialType, float mass)
        {
            var mats = GetValidMaterials(materialType);
            foreach (var mat in mats)
            {
                if(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(mat, true) >= mass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || MaterialSelector.AllowInsufficientMaterialBuild())
                    return mat;
            }
            SgtLogger.error("could not find viable replacementTag for materialType " + materialType);
            return materialType;
        }

        public static List<Tag> GetValidMaterials(Tag materialTypeTag, bool omitDisabledElements = true)
        {
            List<Tag> list = new List<Tag>();
            foreach (Element element in ElementLoader.elements)
            {
                if (!(element.disabled && omitDisabledElements) 
                    && (element.IsSolid || BlueprintsV2.ModAPI.API_Methods.AllowNonSolids(materialTypeTag))
                    && (element.tag == materialTypeTag || element.HasTag(materialTypeTag)))
                {
                    list.Add(element.tag);
                }
            }

            foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
            {
                if (!(materialBuildingElement == materialTypeTag))
                {
                    continue;
                }

                foreach (GameObject item in Assets.GetPrefabsWithTag(materialBuildingElement))
                {
                    KPrefabID component = item.GetComponent<KPrefabID>();
                    if (component != null && !list.Contains(component.PrefabTag))
                    {
                        list.Add(component.PrefabTag);
                    }
                }
            }

            return list;
        }

        public static class ActionKeys
        {
            public static string ACTION_CREATE_KEY = "Blueprints.create.opentool";
            public static string ACTION_USE_KEY = "Blueprints.use.opentool";
            public static string ACTION_SNAPSHOT_KEY = "Blueprints.snapshot.opentool";
            public static string ACTION_RESELECT_KEY = "Blueprints.reselect";
            public static string ACTION_SWAP_ANCHOR_KEY = "Blueprints.anchorswap";
            public static string ACTION_ROTATE_BLUEPRINT_KEY = "Blueprints.rotate";
        }
        public static class Actions
        {
            public static PAction BlueprintsCreateAction { get; set; }
            public static PAction BlueprintsUseAction { get; set; }
            public static PAction BlueprintsSnapshotAction { get; set; }
            public static PAction BlueprintsReopenSelectionAction { get; set; }
            public static PAction BlueprintsSwapAnchorAction { get; set; }
        }
    }
}
