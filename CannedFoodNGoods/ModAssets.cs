﻿using System;

namespace CannedFoods
{
	public class ModAssets
	{
		public class Tags
		{
			public static Tag CanTag = TagManager.Create("CF_Can");
			public static Tag DropCanOnEat = TagManager.Create("CF_Drop_Can_On_Eat");
		}

		public sealed class ExportSettings
		{
			private static readonly Lazy<ExportSettings> lazy =
			new Lazy<ExportSettings>(() => new ExportSettings());

			public static ExportSettings Instance { get { return lazy.Value; } }


			public static SimHashes GetMaterialHashForCans()
			{
				return Config.Instance.GetCanElement();
			}
		}
	}
}
