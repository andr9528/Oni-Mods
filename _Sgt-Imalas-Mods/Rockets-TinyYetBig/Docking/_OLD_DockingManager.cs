﻿//using Klei;
//using KSerialization;
//using Rockets_TinyYetBig.Behaviours;
//using Rockets_TinyYetBig.SpaceStations;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UtilLibs;
//using static STRINGS.BUILDINGS.PREFABS;
//using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN.BUTTONS;
//using static UnityEngine.GraphicsBuffer;

//namespace Rockets_TinyYetBig.Docking
//{
//    [SerializationConfig(MemberSerialization.Invalid)]
//    public class DockingManagerOLD : KMonoBehaviour, IListableOption, ISim1000ms
//    {
//        [MyCmpGet]
//        public Clustercraft clustercraft;

//        public void StartupID(int world)
//        {
//            SgtLogger.l(world.ToString(), "RocketDockingManager Initialize World Id");
//            MyWorldId = world;
//        }

//        bool hasInterior = true;

//        /// <summary>
//        /// My door + connectedDoor
//        /// </summary>
//        int MyWorldId = -1;

//        public Dictionary<IDockable, int> IDockables = new Dictionary<IDockable, int>();
//        public int WorldId => MyWorldId;

//        DockableType Type = DockableType.Rocket;

//        public DockableType GetCraftType => Type;
//        public bool DockableInterior => hasInterior;

//        public AssignmentGroupController GetAssignmentGroupControllerIfExisting()
//        {
//            var PassengerModule = clustercraft.ModuleInterface.GetPassengerModule();
//            if(PassengerModule != null && PassengerModule.TryGetComponent<AssignmentGroupController>(out var controller))
//            {
//                return controller;
//            }
//            if(this.gameObject.TryGetComponent<AssignmentGroupController>(out var controller2))
//                return controller2;

//            return null;
//        }

//        public List<int> GetConnectedWorlds()
//        {
//            var list = new List<int>();
//            foreach (var door in IDockables)
//            {
//                if (door.Value != -1)
//                    list.Add(door.Value);

//            }
//            return list;
//        }
//        public List<int> GetConnectedRockets()
//        {
//            var list = new List<int>();
//            foreach (var door in IDockables)
//            {
//                if (door.Value != -1 && SpaceStationManager.WorldIsRocketInterior(door.Value))
//                    list.Add(door.Value);

//            }
//            return list;
//        }



//        public void SetManagerType(int overrideType = -1)
//        {
//            if (MyWorldId != -1)
//            {
//                if (SpaceStationManager.WorldIsSpaceStationInterior(MyWorldId))
//                {
//                    Type = DockableType.SpaceStation;
//                }
//                else if (SpaceStationManager.WorldIsRocketInterior(MyWorldId))
//                {
//                    Type = DockableType.Rocket;
//                }
//            }
//#if DEBUG
//            SgtLogger.debuglog("CraftType set to: "+ Type);
//#endif
//        }


//        public void SetCurrentlyLoadingStuff(bool IsLoading)
//        {
//            SgtLogger.l("IsNowLoading? " + IsLoading);
//            isLoading = IsLoading;

//            if (!IsLoading && OnFinishedLoading != null)
//                OnFinishedLoading.Invoke();
//        }
//        bool isLoading = false;

//        public System.Action OnFinishedLoading = null;

//        public bool IsLoading => isLoading;

//        List<int> PendingDocks = new List<int>();

//        public void Sim1000ms(float dt)
//        {
//            List<DockingDoor> ToRemove = new List<DockingDoor>();
//            foreach (var undockingProcess in PendingUndocks)
//            {
//                List<MinionIdentity> WrongWorldDupesHERE = new List<MinionIdentity>();
//                List<MinionIdentity> WrongWorldDupesTHERE = new List<MinionIdentity>();
//                if (undockingProcess.Key.IsConnected && IDockables.ContainsKey(undockingProcess.Key))
//                {
//                    //PassengerRocketModule passengerOwn = undockingProcess.Key.GetCraftModuleInterface().GetPassengerModule();
//                    var assignmentGroupControllerOWN = undockingProcess.Key.dManager.GetAssignmentGroupControllerIfExisting();
//                    var assignmentGroupControllerDOCKED = undockingProcess.Key.GetConnec().dManager.GetAssignmentGroupControllerIfExisting();

//                    if (assignmentGroupControllerOWN != null)
//                    {
//                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.dManager.MyWorldId))
//                        {

//#if DEBUG
//                            SgtLogger.l(minion.name, "minion");
//#endif
//                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                            {
//#if DEBUG
//                                SgtLogger.l(minion.name, "wrong here");
//#endif
//                                WrongWorldDupesHERE.Add(minion);
//                            }
//                        }
//                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.GetConnec().dManager.MyWorldId))
//                        {

//#if DEBUG
//                            SgtLogger.l(minion.name, "minion there");
//#endif
//                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerOWN.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                            {

//#if DEBUG
//                                SgtLogger.l(minion.name, "wrong there;");
//#endif
//                                WrongWorldDupesTHERE.Add(minion);
//                            }
//                        }
//                    }
//                    else if (assignmentGroupControllerDOCKED != null)
//                    {
//                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.dManager.MyWorldId))
//                        {

//#if DEBUG
//                            SgtLogger.l(minion.name, "minion 2");
//#endif

//                            if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                            {

//#if DEBUG
//                                SgtLogger.l(minion.name, "wrong here 2");
//#endif
//                                WrongWorldDupesHERE.Add(minion);
//                            }
//                        }
//                        foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(undockingProcess.Key.GetConnec().dManager.MyWorldId))
//                        {

//#if DEBUG
//                            SgtLogger.l(minion.name, "minion there 2");
//#endif
//                            if (!Game.Instance.assignmentManager.assignment_groups[assignmentGroupControllerDOCKED.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                            {

//#if DEBUG
//                                SgtLogger.l(minion.name, "wrong there 2");
//#endif
//                                WrongWorldDupesTHERE.Add(minion);
//                            }
//                        }
//                    }
//                    //SgtLogger.l("GONE");

//                    DockingDoor OwnDoor = (undockingProcess.Key);
//                    DockingDoor ConnectedDoor = (undockingProcess.Key.GetConnec() as DockingDoor);

//                    foreach (var minion in WrongWorldDupesHERE)
//                    {
//                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();                      
//                        smi.SetMoveTarget(ConnectedDoor.GetPorterCell());
//                        OwnDoor.RefreshAccessStatus(minion, false);
//                        ConnectedDoor.RefreshAccessStatus(minion, true);

//                    }
//                    foreach (var minion in WrongWorldDupesTHERE)
//                    {
//                        var smi = minion.GetSMI<RocketPassengerMonitor.Instance>();
//                        smi.SetMoveTarget((undockingProcess.Key).GetPorterCell());

//                        OwnDoor.RefreshAccessStatus(minion, true);
//                        ConnectedDoor.RefreshAccessStatus(minion, false);
//                    }

//                    if (WrongWorldDupesHERE.Count == 0 && WrongWorldDupesTHERE.Count == 0)
//                    {
//                        SgtLogger.log(string.Format("Undocking world {0} from {1}, now that all dupes are moved", undockingProcess.Key.dManager.WorldId, undockingProcess.Key.GetConnec().dManager.WorldId));
//                        UndockDoor(undockingProcess.Key, false, undockingProcess.Value);
//                        ToRemove.Add(undockingProcess.Key);
//                    }
//                }
//            }
//            foreach (var done in ToRemove)
//            {
//                PendingUndocks.Remove(done);
//            }

//            List<int> ToDock = new List<int>();
//            foreach (var worldToDockTo in PendingDocks)
//            {
//                var target = ModAssets.Dockables.Items.Find(mng => mng.MyWorldId == worldToDockTo);
//                if (target != null && target.TryGetComponent<ClusterGridEntity>(out var targetEntity))
//                {
//                    if (targetEntity.Location == clustercraft.Location && CanDock() && target.CanDock())
//                    {
//                        ToDock.Add(worldToDockTo);
//                    }
//                }
//            }
//            foreach (var doDock in ToDock)
//            {
//                DockToTargetWorld(doDock);
//            }
//        }

//        public void CleanupWorldAssignmentsTargetWorldDoor(IDockable targetDoor)
//        {
//            if (targetDoor == null)
//                return;
//            if (targetDoor.dManager != null && targetDoor.dManager.WorldId != this.WorldId) 
//            {
//                this.CleanupWorldAssignments(targetDoor.dManager.WorldId);
//            }
//        }

//        /// <summary>
//        /// Clean up world assignments if there was an unexpected decoupling (by blow up or deconstruction)
//        /// </summary>
//        /// <param name="targetWorldId"></param>
//        public void CleanupWorldAssignments(int targetWorldId)
//        {
//            var OwnWorld = ClusterManager.Instance.GetWorld(MyWorldId);
//            var DockedWorld = ClusterManager.Instance.GetWorld(targetWorldId);

//            PassengerRocketModule OwnPassengerModule = null;
//            if (OwnWorld != null)
//                OwnPassengerModule = OwnWorld.GetComponent<CraftModuleInterface>().GetPassengerModule();


//            PassengerRocketModule ConnectedPassengerModule = null;
//            if (DockedWorld != null)
//                ConnectedPassengerModule = DockedWorld.GetComponent<CraftModuleInterface>().GetPassengerModule();


//            if (OwnPassengerModule != null && OwnPassengerModule.TryGetComponent(out AssignmentGroupController OwnWorldAssignmentController))
//            {
//                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(MyWorldId))
//                {
//                    if (!Game.Instance.assignmentManager.assignment_groups[OwnWorldAssignmentController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                    {
//                        Game.Instance.assignmentManager.assignment_groups[OwnWorldAssignmentController.AssignmentGroupID].AddMember(minion.assignableProxy.Get());
//                    }
//                }

//            }
//            if (ConnectedPassengerModule != null && ConnectedPassengerModule.TryGetComponent(out AssignmentGroupController TargetWorldController))
//            {
//                foreach (var minion in Components.LiveMinionIdentities.GetWorldItems(MyWorldId))
//                {
//                    if (Game.Instance.assignmentManager.assignment_groups[TargetWorldController.AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
//                    {
//                        Game.Instance.assignmentManager.assignment_groups[TargetWorldController.AssignmentGroupID].RemoveMember(minion.assignableProxy.Get());
//                    }
//                }
//            }
//        }


//        public void AddPendingDock(int worldID)
//        {
//            if (PendingDocks.Contains(worldID))
//                return;
//            PendingDocks.Add(worldID);
//        }

//        public Sprite GetDockingIcon()
//        {
//            Sprite returnVal = null;
//            switch (Type)
//            {
//                case DockableType.Rocket:
//                case DockableType.SpaceStation:
//                    returnVal = clustercraft.GetUISprite();
//                    break;
//                case DockableType.Derelict:
//                   // break;

//                default:
//                    returnVal = Assets.GetSprite("unknown");
//                    break;
//            }
//            return returnVal;

//        }

//        public void InitializeWorldId()
//        {
//            if (this.gameObject == null)
//            {
//                SgtLogger.warning($"Gameobject of dockingmanager {this} was null?!");
//                return;
//            }


//            if (!this.gameObject.TryGetComponent<WorldContainer>(out var container))
//            {
//                SgtLogger.logwarning("no worldContainer found:" + gameObject.name);
//                MyWorldId = -1;
//                hasInterior = false;
//                return;
//            }

//            if (MyWorldId != -1)
//                return;

//            StartupID(container.id);
//            if (container.Width < 5 || container.Height < 5)
//            {
//                SgtLogger.log("World has no proper interior and will be handled as AI rocket: " + gameObject.name);
//                hasInterior = false;
//            }
//            else 
//                hasInterior = true;


//        }
//        public override void OnSpawn()
//        {
//            SgtLogger.l("Added docking manager to "+this.gameObject.name);
//            base.OnSpawn();
//            //ModAssets.Dockables.Add(this);
//            InitializeWorldId();

//            ClusterManager.Instance.Trigger(ModAssets.Hashes.DockingManagerAdded, this);
//            //Subscribe((int)GameHashes.RocketModuleChanged, (module) =>InitializeWorldId());

//#if DEBUG
//            SgtLogger.debuglog("AddedDockable");
//#endif
//        }
//        public override void OnCleanUp()
//        {
//            SgtLogger.l("removed docking manager with " + this.gameObject.name);

//            ClusterManager.Instance.Trigger(ModAssets.Hashes.DockingManagerRemoved, this);

//            //ModAssets.Dockables.Remove(this);
//            //Unsubscribe((int)GameHashes.RocketModuleChanged, (module) => InitializeWorldId());
//            base.OnCleanUp();
//        }

//        //public string GetUiDoorInfo()
//        //{
//        //    if (PendingUndocks.Count > 0)
//        //        return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.UNDOCKINGPENDING;
//        //    else if (PendingDocks.Count > 0)
//        //        return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.DOCKINGPENDING;

//        //    int count = AvailableConnections();
//        //    if (count == 1)
//        //        return STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.ONECONNECTION;
//        //    else
//        //        return string.Format(STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.MORECONNECTIONS, count);
//        //}
//        public int AvailableConnections()
//        {
//            int count = IDockables.Keys.ToList().FindAll(k => k.GetConnec() == null).Count();
//            return count;
//        }
//        public int TotalConnections()
//        {
//            int count = IDockables.Keys.Count();
//            return count;
//        }

//        public void AddDockable(IDockable door)
//        {
//            if (MyWorldId == -1)
//                InitializeWorldId();
//            if (!IDockables.ContainsKey(door))
//            {
//                int target = -1;
//                if (door.GetConnec() != null)
//                    target = door.GetConnec().GetWorldObject().GetComponent<WorldContainer>().id;//  dManager.WorldId;
//                IDockables.Add(door, target);
//                SgtLogger.debuglog("Added new Docking Door!, ID: " + MyWorldId + ", Doorcount: " + IDockables.Count()+", Connected? "+target);
//            }
//            else
//            {
//                SgtLogger.debuglog("Docking Door already existant!, ID: " + MyWorldId + ", Doorcount: " + IDockables.Count() );
//            }

//            //UpdateDeconstructionStates();

//        }

//        public void RemoveDockable(IDockable door)
//        {
//            if (IDockables.ContainsKey(door))
//            {
//                SgtLogger.debuglog(door + "- IDockable removed from  " + door.GetMyWorldId());
//                UnDockFromTargetWorld(door.GetConnectedTargetWorldId(), true);
//                ///Disconecc;
//                //door.DisconnecDoor();
//                IDockables.Remove(door);
//                //UpdateDeconstructionStates();
//            }

//            if (this.IDockables.Count == 0)
//            {
//                SgtLogger.l("removing docking manager");
//                UnityEngine.Object.Destroy(this);
//            }
//        }

//        public bool CanDock()
//        {
//            bool cando = IDockables.Any(k => k.Key.GetConnec() == null)
//                && HasDoors()
//                && (clustercraft.status == Clustercraft.CraftStatus.InFlight);
//            return cando;
//        }
//        public bool IsDockedToAny()
//        {
//            return IDockables.Any(k => k.Key.GetConnec() != null);
//        }

//        public bool HasDoors()
//        {
//            //SgtLogger.debuglog("HAs Doors: " + IDockables.Count);
//            return IDockables.Count > 0;
//        }

//        public bool IsDockedTo(int WorldID)
//        {
//            return IDockables.ContainsValue(WorldID);
//        }
//        public IDockable DockedToDoor(int targetWorldID)
//        {
//            if (IDockables.ContainsValue(targetWorldID))
//            {
//                return IDockables.First(kvp => kvp.Value == targetWorldID).Key;
//            }
//            return null;
//        }


//        public bool GetActiveUIState(int worldId) => IsDockedTo(worldId) || HasPendingUndocks(worldId);

//        public bool HasPendingUndocks(int WorldID)
//        {
//            if(PendingUndocks == null || PendingUndocks.Keys == null || PendingUndocks.Keys.Count == 0)
//            {
//                return false; 
//            }
//            return PendingUndocks.Keys.Any(door => door.dManager!=null &&  door.dManager.WorldId == WorldID);
//        }

//        public void HandleUiDocking(int prevDockingState, int targetWorld, IDockable door = null, System.Action onFinished = null)
//        {
//            //SgtLogger.debuglog(prevDockingState == 0 ? "Trying to dock " + MyWorldId + " with dedicated door to " + targetWorld : "Trying To Undock " + MyWorldId + " from " + targetWorld);

//            if (prevDockingState == 0)
//                DockToTargetWorld(targetWorld, door);
//            else
//                UnDockFromTargetWorld(targetWorld, OnFinishedUndock: onFinished);
//        }

//        public void DockToTargetWorld(int targetWorldId, IDockable OwnDoor = null)
//        {
//            SgtLogger.l("Can Dock? " + this.CanDock());
//            if (!this.CanDock())
//                return;

//            var target = ModAssets.Dockables.Items.Find(mng => mng.MyWorldId == targetWorldId);


//            if (target == null || target.IDockables.Count == 0 || this.IDockables.Count == 0 || !target.CanDock())
//            {
//                SgtLogger.warning("No doors found");
//                return;
//            }
//            if (IsDockedTo(targetWorldId))
//            {
//                SgtLogger.warning("Already Docked");
//                return;
//            }

//            if (HasPendingUndocks(targetWorldId))
//            {
//                var worldDoor = PendingUndocks.FirstOrDefault(door => door.Key.GetMyWorldId() == targetWorldId).Key;
//                SgtLogger.l("canceled pending undocking");
//                if (worldDoor != null)
//                    PendingUndocks.Remove(worldDoor);
//                return;
//            }

//            ConnectTwo(this, target, OwnDoor);

//            if (SpaceStationManager.WorldIsSpaceStationInterior(MyWorldId))
//            {
//                ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(MyWorldId);
//            }
//            else if (SpaceStationManager.WorldIsSpaceStationInterior(targetWorldId))
//            {
//                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(targetWorldId);
//            }

//            else
//                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(targetWorldId);

//            if (PendingDocks.Contains(targetWorldId))
//                PendingDocks.Remove(targetWorldId);

//            SgtLogger.l(MyWorldId + " docked to world: " + targetWorldId);
//        }
//        public void UndockAll(bool force = false)
//        {
//            if (GetConnectedWorlds().Count > 0)
//                SgtLogger.debuglog("Undocking all");

//            foreach (int id in GetConnectedWorlds())
//            {
//                SgtLogger.debuglog("from World: " + id);
//                UnDockFromTargetWorld(id, force);
//            }
//        }

//        public static void ConnectTwo(DockingManager door1mng, DockingManager door2mng, IDockable OverwriteOwn = null)
//        {
//            if (OverwriteOwn != null && OverwriteOwn.GetConnec() != null)
//                OverwriteOwn = null;

//            var door1 = OverwriteOwn != null ? OverwriteOwn : door1mng.IDockables.First(k => k.Value == -1).Key;
//            var door2 = door2mng.IDockables.First(k => k.Value == -1).Key;
//            if (door1 == null || door2 == null)
//                return;


//            door1mng.IDockables[door1] = door2mng.MyWorldId;
//            door2mng.IDockables[door2] = door1mng.MyWorldId;

//            door1.ConnecDockable(door2);
//            door2.ConnecDockable(door1);
//            if (door1.HasDupeTeleporter && door2.HasDupeTeleporter)
//                door1.Teleporter.EnableTwoWayTarget(true);
//            //DetailsScreen.Instance.Refresh(door1.gameObject);
//        }


//        //public void StartupConnect(IDockable door1, IDockable door2)
//        //{
//        //    door1.ConnecDoor(door2);
//        //    door2.ConnecDoor(door1);
//        //    door1.Teleporter.EnableTwoWayTarget(true);
//        //}

//        void InitPendingUndock(IDockable door, bool cleanup = false, System.Action OnFinishedUndock = null)
//        {
//            SgtLogger.l("Pending undock: " + door.GetMyWorldId() + " force: " + cleanup);

//            if (cleanup)
//            {
//                CleanupWorldAssignmentsTargetWorldDoor(door.GetConnec());
//                UndockDoor(door, true, OnFinishedUndock);
//            }
//            else
//            {
//                if (!PendingUndocks.Keys.Contains(door) && !PendingUndocks.Keys.Contains(door.GetConnec()))
//                    PendingUndocks.Add(door as DockingDoor, OnFinishedUndock);
//            }
//        }

//        public static Dictionary<DockingDoor, System.Action> PendingUndocks = new Dictionary<DockingDoor, System.Action>();


//        void UndockDoor(IDockable door, bool cleanup = false, System.Action OnFinishedUndock = null)
//        {
//            var door2 = door.GetConnec();

//            if (door.HasDupeTeleporter && door2.HasDupeTeleporter)
//                door.Teleporter.EnableTwoWayTarget(false);

//            door2.dManager.IDockables[door2] = -1;
//            door.dManager.IDockables[door] = -1;

//            door.DisconnecDoor(cleanup);
//            door2.DisconnecDoor(cleanup);

//            int targetWorldId = door2.GetMyWorldId();

//            if (OnFinishedUndock != null)
//                OnFinishedUndock.Invoke();

//            if (ClusterManager.Instance.GetWorld(targetWorldId) != null)
//                ClusterManager.Instance.GetWorld(targetWorldId).SetParentIdx(targetWorldId);

//            if (ClusterManager.Instance.GetWorld(MyWorldId) != null)
//                ClusterManager.Instance.GetWorld(MyWorldId).SetParentIdx(MyWorldId);

//            door.gameObject.Trigger((int)GameHashes.RocketLaunched);
//            door2.gameObject.Trigger((int)GameHashes.RocketLaunched);


//            //DetailsScreen.Instance.Refresh(door.gameObject);
//        }

//        public void UnDockFromTargetWorld(int targetWorldId, bool cleanup = false, System.Action OnFinishedUndock = null)
//        {
//            SgtLogger.debuglog(WorldId + "<- ownId , TargetWorldToUndock: " + targetWorldId);

//            if (targetWorldId == -1)
//                return;

//            var door = IDockables.FirstOrDefault(d => d.Value == targetWorldId).Key;
//            if (door == null)
//            {
//                Debug.LogWarning("No connection to undock from found");
//                return;
//            }
//            if (door.HasDupeTeleporter && door.GetConnec().HasDupeTeleporter)
//            {
//                InitPendingUndock(door, cleanup, OnFinishedUndock);
//            }
//            else
//            {
//                SgtLogger.l("undocking...");
//                UndockDoor(door, false, OnFinishedUndock);
//            }
//        }

//        public string GetProperName() => this.GetComponent<Clustercraft>().name;

//    }
//    public enum DockableType
//    {
//        Rocket = 0,
//        SpaceStation = 1,
//        Derelict = 2
//    }
//}
