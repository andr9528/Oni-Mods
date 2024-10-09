﻿using System.Collections.Generic;
using UtilLibs;

namespace Dupery
{
	class AccessoryManager
	{
		public AccessoryPool Pool { get { return this.accessoryPool; } }

		private AccessoryPool accessoryPool;
		public static Dictionary<string, string> HeadOverrideAnims = new(); //key: headshape, value: animName
		public static Dictionary<HashedString, string> PersonalityHeadOverrideAnims = new(); //key: personalityId, value: animName

		public AccessoryManager()
		{
			accessoryPool = new AccessoryPool();
		}

		public bool TryGetAccessoryId(string slotId, string accessoryName, out string accessoryId)
		{
			return accessoryPool.TryGetId(slotId, accessoryName, out accessoryId);
		}
		public bool TryGetHeadAnimOverride(MinionIdentity identity, out string headAnimOverride)
		{
			return PersonalityHeadOverrideAnims.TryGetValue(identity.personalityResourceId, out headAnimOverride);
		}
		public bool RegisterPersonalityForCustomCheeks(HashedString personalityID, string headshape)
		{
			if (headshape == null) return false;

			if (HeadOverrideAnims.TryGetValue(headshape, out var anim))
			{
				SgtLogger.l("Registered custom headshape for " + personalityID + ": " + headshape + " -> " + anim);
				PersonalityHeadOverrideAnims[personalityID] = anim;
				return true;
			}
			return false;
		}


		//[HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.AddGroup))]
		//public class KAnimGroupFileGroup_TargetMethod_Patch
		//{
		//    public static void Postfix(KAnimGroupFile.GroupFile gf,KAnimFile file)
		//    {
		//            SgtLogger.l(gf.groupID  , "GroupDumping");
		//    }
		//}

		public int LoadAccessories(string animName, bool saveToCache = false)
		{
			ResourceSet accessories = Db.Get().Accessories;

			KAnimFile anim = Assets.GetAnim(animName);
			if (anim == null)
			{
				Debug.LogWarning("[Dupery]: no anim with the name " + animName + " found");
				return 0;
			}
			KAnim.Build build = anim.GetData().build;

			int numLoaded = 0;
			int numCached = 0;
			var accessorySlots = Db.Get().AccessorySlots;
			var resourceTable = Db.Get().ResourceTable;

			for (int index = 0; index < build.symbols.Length; ++index)
			{
				string id = HashCache.Get().Get(build.symbols[index].hash);
				AccessorySlot slot = null;
				string lowerinvid = id.ToLowerInvariant();
				Debug.Log("[Dupery]: trying to load accessory " + id);
				foreach (var _slot in accessorySlots.resources)
				{
					string slotID = _slot.Id.ToLowerInvariant();
					if (lowerinvid.Contains(slotID))
						slot = _slot;
				}

				if (slot == null)
					continue;

				bool cachable = true;
				if (slot.Id == accessorySlots.HatHair.Id)
				{
					cachable = false;
				}

				if (slot.Id == accessorySlots.HeadShape.Id)
				{
					HeadOverrideAnims.Add(id, animName);
					Debug.Log("[Dupery]: setting custom cheek override anim for headshape: " + id);
				}


				Accessory accessory = new Accessory(id, accessories, slot, anim.batchTag, build.symbols[index], anim);
				slot.accessories.Add(accessory);
				resourceTable.Add(accessory);

				if (cachable && saveToCache)
				{
					accessoryPool.AddId(slot.Id, id, id);
					numCached++;
				}
				numLoaded++;
				Debug.Log("[Dupery]: accessory successfully loaded as " + slot.Name);
			}

			if (numCached > 0)
				Logger.Log($"Added {numCached} new accessories IDs to the cache.");

			return numLoaded;
		}
	}
}
