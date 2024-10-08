﻿using KSerialization;

namespace RebuildPreserve
{
	internal class ConduitDirectionInfo : KMonoBehaviour
	{
		[Serialize] public UtilityConnections connectedDirections;
		[Serialize] int targetCell;
		[Serialize] IUtilityNetworkMgr manager;
		[Serialize] public bool initialized = false;


		public void StoreConduitConnections(
			int cell,
			IUtilityNetworkMgr mgr)
		{
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell == null)
				return;
			manager = mgr;
			targetCell = cell;
			connectedDirections = mgr.GetConnections(cell, true);
			initialized = true;
			//SgtLogger.l($"connected: {connectedDirections}");
		}
		public void ApplyConduitConnections(bool notPlannedBuilding)
		{
			if (initialized)
			{
				manager.SetConnections(connectedDirections, targetCell, notPlannedBuilding);
			}
		}
	}
}
