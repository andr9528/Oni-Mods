﻿using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SaveGameModLoader.ModFilter
{
	internal class OnHoverReveal : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public List<Image> Images = new List<Image>();
		public bool ShouldToggle = true;


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Images.ForEach((img) => { img.SetAlpha(ShouldToggle ? 0f : 1f); });
			//Target.SetActive(!ShouldToggle);
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (ShouldToggle && Images.Count > 0)
				Images.ForEach((img) => { img.SetAlpha(1f); });
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (ShouldToggle && Images.Count > 0)
				Images.ForEach((img) => { img.SetAlpha(0f); });
		}
	}
}
