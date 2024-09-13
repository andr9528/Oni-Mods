using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace TooMuchLogic.Services
{
    public class ConjoinedSensorLogicService<TSensor> where TSensor : Switch
    {
        internal bool buildingRoom = false;
        internal HashSet<int> roomCells;
        [CanBeNull] internal string currentRoomId;

        internal int radius = 0;
        private static Dictionary<string, HashSet<int>> rooms;
        private static Dictionary<int, TSensor> knownSensors;
        private readonly int motherCell;

        public ConjoinedSensorLogicService(
            int startingRadius, ref Dictionary<string, HashSet<int>> refRooms,
            ref Dictionary<int, TSensor> refKnownSensors, int motherCell)
        {
            radius = startingRadius;
            rooms = refRooms;
            knownSensors = refKnownSensors;
            this.motherCell = motherCell;
            roomCells = new HashSet<int>();
        }

        internal void CollectConnectedElementCells(
            System.Action updateSensorValue, Func<bool> setSensorValueFromOtherSensor)
        {
            if (buildingRoom)
                return;
            buildingRoom = true;

            if ((double) Grid.Mass[motherCell] <= 0.0)
            {
                buildingRoom = false;
                return;
            }

            roomCells.Clear();

            Element startElement = Grid.Element[motherCell];

            var grapper = new CellGrapperService();
            roomCells = radius == 0
                ? grapper.GrabCellsIterative(motherCell, startElement, setSensorValueFromOtherSensor, EarlyBreak)
                : grapper.GrabCellsRadius(motherCell, startElement, radius);

            if (radius == 0 && string.IsNullOrWhiteSpace(currentRoomId))
            {
                currentRoomId = Guid.NewGuid().ToString();
                rooms.Add(currentRoomId, new HashSet<int>());
                rooms[currentRoomId].Add(motherCell);
                //SgtLogger.log($"Cell {motherCell} is inside a newly created room ({currentRoomId})");
            }

            buildingRoom = false;

            updateSensorValue();
        }

        internal void RemoveFromRoom()
        {
            if (string.IsNullOrWhiteSpace(currentRoomId))
                return;

            rooms[currentRoomId].Remove(motherCell);
            currentRoomId = "";
        }

        private bool ShouldUpdateSensorValueFromOtherSensor(int source, int cell)
        {
            if (source == cell)
                return false;

            var existingRoom = rooms.FirstOrDefault(x => x.Value.Contains(cell));
            if (existingRoom.Equals(default(KeyValuePair<string, HashSet<int>>)))
                return false;

            existingRoom.Value.Add(source);
            currentRoomId = existingRoom.Key;

            return true;
        }

        private bool EarlyBreak(int source, int cell)
        {
            return radius == 0 && knownSensors.ContainsKey(cell) &&
                   ShouldUpdateSensorValueFromOtherSensor(source, cell);
        }

        internal bool ShouldCellsBeRecollected()
        {
            if (string.IsNullOrEmpty(currentRoomId))
                return true;

            var room = rooms.FirstOrDefault(x => x.Key == currentRoomId);

            return !string.IsNullOrEmpty(currentRoomId) && room.Value.First() == motherCell;
        }
    }
}