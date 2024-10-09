﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLibs.UIcmp
{
	public class FMultiSelectDropdown : KMonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public System.Action RefreshUI;

		GameObject DropDownContent;

		FToggle entryPrefab;
		Image backgroundImage;

		public Color Inactive = UIUtils.rgb(62, 67, 87);
		public Color OnHover = UIUtils.rgb(88, 95, 122);


		public List<FDropDownEntry> DropDownEntries = null;

		public class FDropDownEntry
		{
			public FDropDownEntry(string title, System.Action<bool> onToggled, bool enabled = true, string tooltip = "")
			{
				Title = title;
				OnToggled = onToggled;
				Enabled = enabled;
				Description = tooltip;
			}

			public string Title;
			public string Description = "";
			public System.Action<bool> OnToggled;
			public bool Enabled = true;
		}


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			backgroundImage = GetComponent<Image>();
			backgroundImage.color = Inactive;
			DropDownContent = transform.Find("DropDownContent").gameObject;
			entryPrefab = transform.Find("DropDownContent/Item").gameObject.AddOrGet<FToggle>();
			entryPrefab.gameObject.SetActive(false);
			InitializeDropDown();


		}
		public void InitializeDropDown()
		{
			if (DropDownEntries != null)
			{
				DropDownContent.SetActive(true);
				foreach (var entry in DropDownEntries)
				{
					var toggle = Util.KInstantiateUI<FToggle>(entryPrefab.gameObject, DropDownContent, true);
					toggle.SetCheckmark("Background/Checkmark");
					toggle.SetOnFromCode(entry.Enabled);

					toggle.OnClick += entry.OnToggled;
					if (RefreshUI != null)
						toggle.OnClick += (_) => RefreshUI();
					toggle.transform.Find("Label").GetComponent<LocText>().text = entry.Title;
					if (entry.Description != null && entry.Description.Length > 0)
						UIUtils.AddSimpleTooltipToObject(toggle.transform, entry.Description);

				}
				DropDownContent.SetActive(false);
			}
		}



		public void OnPointerEnter(PointerEventData eventData)
		{
			backgroundImage.color = OnHover;
			DropDownContent?.SetActive(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			backgroundImage.color = Inactive;
			DropDownContent?.SetActive(false);
		}
	}
}
