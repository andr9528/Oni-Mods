﻿using STRINGS;

namespace Cryopod
{
	class STRINGS
	{

		public class CRYOPODRESEARCHTYPE
		{
			public static LocString NAME = "Cryopod Analysis";
			public static LocString RECIPEDESC = "Analyzing gravitas cryopods yields new knowledge.";
		}

		public class DUPLICANTS
		{
			public class STATUSITEMS
			{
				public class FORCETHAWED
				{
					public static LocString NAME = UI.FormatAsLink("Cryo Sickness", nameof(FORCETHAWED));
					public static LocString TOOLTIP = "Being forcefully thawed, this Duplicant does not have a good time.";
				}
			}
		}
		public class RESEARCH
		{
			public class TECHS
			{
				public class FROSTEDDUPERESEARCH
				{
					public static LocString NAME = UI.FormatAsLink("Cryogenics", nameof(FROSTEDDUPERESEARCH));
					public static LocString DESC = "Unlocked by investigating ancient artifacts left behind.\nEnables freezing your Duplicants in cryosleep.";

				}
			}
		}

		public class BUILDINGS
		{
			public class PREFABS
			{
				public class CRY_BUILDABLECRYOTANK
				{
					public static LocString NAME = UI.FormatAsLink("Cryotank 4000", nameof(CRY_BUILDABLECRYOTANK));
					public static LocString DESC = (LocString)"A design found in an ancient facility, your Duplicants have managed to almost perfectly replicate it.";
					public static LocString EFFECT = (LocString)"Can store 1 Duplicant.\n\nHeats up during freezing.\nCools down during thawing.";
					public static LocString DEFROSTBUTTON = (LocString)"Defrost stored Friend";
					public static LocString DEFROSTBUTTONCANCEL = (LocString)"Cancel Defrosting";
					public static LocString DEFROSTBUTTONTOOLTIP = (LocString)"Unfreeze the duplicant stored here.";

					public static LocString INPUT_LOGIC_PORT = "Thaw Duplicant";
					public static LocString INPUT_LOGIC_PORT_ACTIVE = (UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Triggers the thawing process.");
					public static LocString INPUT_LOGIC_PORT_INACTIVE = (UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Does nothing");
				}
				public class CRY_BUILDABLECRYOTANKLIQUID
				{
					public static LocString NAME = UI.FormatAsLink("Cryotank 5000", nameof(CRY_BUILDABLECRYOTANKLIQUID));
					public static LocString DESC = (LocString)"A design found in an ancient facility, your Duplicants have managed to almost perfectly replicate it.";
					public static LocString EFFECT = (LocString)"Can store 1 Duplicant.\n\nHeats up during freezing.\nCools down during thawing.\n\nExchanges temperature with the liquid pumped through.";
				}
			}
		}

		public class BUILDING
		{
			public class STATUSITEMS
			{
				public class CRY_DUPLICANTINTERNALTEMPERATURE
				{
					public static LocString NAME = "The Dupe is at {InternalTemperature}.";
					public static LocString TOOLTIP = "Cryogenic process cools down the body for preservation.";
				}

				public class CRY_DUPLICANTNAMESTATUS
				{
					public static LocString NAME = "Duplicant in cryogenic sleep: {DupeName}";
					public static LocString TOOLTIP = "{DupeName} takes a cool nap.";
				}
				public class CRY_DUPLICANTHEALTHSTATUS
				{
					public static LocString NAME = "Cryogenic Healthyness: {DupeHealthState}";
					public static LocString TOOLTIP = "{DupeHealthStateTooltip}";
					public static LocString HEALTHGOODNAME = "Healthy";
					public static LocString HEALTHSOMEDAMAGENAME = "Light Wounds";
					public static LocString HEALTHMAJORDAMAGENAME = "Severe Wounds";
					public static LocString HEALTHINCAPACITATENAME = "Critical";


					public static LocString HEALTHGOOD = "The Duplicant is in a healthy condition";
					public static LocString HEALTHSOMEDAMAGE = "The Duplicant has suffered minor damage due to malfunctions.";
					public static LocString HEALTHMAJORDAMAGE = "The Duplicant has suffered major damage due to malfunctions!";
					public static LocString HEALTHINCAPACITATE = "The Duplicant has suffered extreme amounts of cell damage and will die if thawed";
				}
				public class CRY_DUPLICANTCRYODAMAGE
				{
					public static LocString NAME = "Warning, Duplicant is thawing improperly";
					public static LocString TOOLTIP = "When this duplicant thaws fully, it won't have a good time.";
				}
				public class CRY_DUPLICANTATTEMPERATURE
				{
					public static LocString NAME = "Energy Saving Mode";
					public static LocString TOOLTIP = "Fully cooled down, this buiding has entered energy saving mode.";
				}
			}
		}
		public class DISEASES
		{
			public class CRYOSICKNESS
			{
				public static LocString NAME = "Cryo Sickness";
				public static LocString DESCRIPTIVE_SYMPTOMS = (LocString)"A lot of cells got pierced by a non standard cryopod thawing.";
				public static LocString DESCRIPTION = (LocString)"After a botched thawing process, this dupe will take time to feel normal again.";
			}
		}
	}
}
