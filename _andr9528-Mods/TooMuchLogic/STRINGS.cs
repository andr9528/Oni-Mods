using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooMuchLogic.Logic;

namespace TooMuchLogic
{
    internal class STRINGS
    {
        //UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN
        public class MODDED_UI
        {
            public class UISIDESCREENS
            {
                public class THRESHOLD_SWITCH_SIDESCREEN
                {
                    public static LocString MASS_TITLE = (LocString) "Mass Threshold";
                    public static LocString CONTENT_MASS = (LocString) "Kilogram";

                    public static LocString MASS_TOOLTIP_ABOVE = (LocString) ("Will send a " +
                                                                              UI.FormatAsAutomationState("Green Signal",
                                                                                  UI.AutomationState.Active) +
                                                                              " if the mass is above <b>{0}</b>");

                    public static LocString MASS_TOOLTIP_BELOW = (LocString) ("Will send a " +
                                                                              UI.FormatAsAutomationState("Green Signal",
                                                                                  UI.AutomationState.Active) +
                                                                              " if the mass is below <b>{0}</b>");

                    public static LocString MASS_UNITS = (LocString) "";
                }
            }
        }

        public class BUILDINGS
        {
            public class PREFABS
            {
                public class TMLCONNECTEDELEMENTTEMPERATURESENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Connected Element Thermo Sensor",
                        nameof(TmlConnectedElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Connected Element Thermo Sensors can disable buildings when average temperature of connected elements approach dangerous temperatures.";

                    public static LocString EFFECT = (LocString) ("Sends a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + " or a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) + " when average " +
                                                                  UI.FormatAsLink("Temperature", "HEAT") +
                                                                  " of matching elements enters the chosen range.");

                    public static LocString LOGIC_PORT =
                        (LocString) ("Ambient " + UI.FormatAsLink("Temperature", "HEAT"));

                    public static LocString LOGIC_PORT_ACTIVE = (LocString) ("Sends a " +
                                                                             UI.FormatAsAutomationState("Green Signal",
                                                                                 UI.AutomationState.Active) +
                                                                             " if average " +
                                                                             UI.FormatAsLink("Temperature", "HEAT") +
                                                                             " is within the selected range");

                    public static LocString LOGIC_PORT_INACTIVE = (LocString) ("Otherwise, sends a " +
                                                                               UI.FormatAsAutomationState("Red Signal",
                                                                                   UI.AutomationState.Standby));

                    public static LocString SIDESCREEN_TOOLTIP =
                        (LocString) "Radius that the sensor works in. 0 being infinite.";

                    public static LocString SIDESCREEN_TITTLE = (LocString) "Acting Radius";
                }

                public class TMLCONNECTEDELEMENTMASSSENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Connected Element Mass Sensor",
                        nameof(TmlConnectedElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Connected Element Mass Sensors can disable buildings when total mass of connected elements is at desired levels.";

                    public static LocString EFFECT = (LocString) ("Sends a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + " or a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) +
                                                                  " depending on the total mass of matching elements in the area.");

                    public static LocString LOGIC_PORT = (LocString) "Total Mass";

                    public static LocString LOGIC_PORT_ACTIVE = (LocString) ("Sends a " +
                                                                             UI.FormatAsAutomationState("Green Signal",
                                                                                 UI.AutomationState.Active) +
                                                                             " if total mass is above the selected level");

                    public static LocString LOGIC_PORT_INACTIVE = (LocString) ("Otherwise, sends a " +
                                                                               UI.FormatAsAutomationState("Red Signal",
                                                                                   UI.AutomationState.Standby));

                    public static LocString SIDESCREEN_TOOLTIP =
                        (LocString) "Radius that the sensor works in. 0 being infinite.";

                    public static LocString SIDESCREEN_TITTLE = (LocString) "Acting Radius";
                }

                public class TMLONEWAYGATE
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("One-Way Gate", nameof(TMLONEWAYGATE));

                    public static LocString DESC =
                        (LocString) "This gate splits a logic network in two, outputting the same signal as inputted.";

                    public static LocString EFFECT = (LocString) ("Outputs a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) +
                                                                  " if the Input is receiving a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + ".\n\nOutputs a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) +
                                                                  " when its Input is receiving a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) + ".");

                    public static LocString OUTPUT_NAME = (LocString) "OUTPUT";

                    public static LocString OUTPUT_ACTIVE = (LocString) ("Sends a " +
                                                                         UI.FormatAsAutomationState("Green Signal",
                                                                             UI.AutomationState.Active) +
                                                                         " if receiving " +
                                                                         UI.FormatAsAutomationState("Green",
                                                                             UI.AutomationState.Active));

                    public static LocString OUTPUT_INACTIVE = (LocString) ("Sends a " +
                                                                           UI.FormatAsAutomationState("Red Signal",
                                                                               UI.AutomationState.Standby) +
                                                                           " if receiving " +
                                                                           UI.FormatAsAutomationState("Red",
                                                                               UI.AutomationState.Standby));
                }

                public class TMLLIQUIDCONDUITMASSSENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Liquid Pipe Mass Sensor",
                        nameof(TmlConnectedElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Liquid Pipe Mass Sensors can output a logic signal when mass of content is withing desirable levels.";

                    public static LocString EFFECT = (LocString) ("Sends a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + " or a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) +
                                                                  " depending on the mass of the conduit content.");

                    public static LocString LOGIC_PORT = (LocString) "Mass";

                    public static LocString LOGIC_PORT_ACTIVE = (LocString) ("Sends a " +
                                                                             UI.FormatAsAutomationState("Green Signal",
                                                                                 UI.AutomationState.Active) +
                                                                             " if mass is above the selected level");

                    public static LocString LOGIC_PORT_INACTIVE = (LocString) ("Otherwise, sends a " +
                                                                               UI.FormatAsAutomationState("Red Signal",
                                                                                   UI.AutomationState.Standby));
                }

                public class TMLGASCONDUITMASSSENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Gas Pipe Mass Sensor",
                        nameof(TmlConnectedElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Gas Pipe Mass Sensors can output a logic signal when mass of content is withing desirable levels.";

                    public static LocString EFFECT = (LocString) ("Sends a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + " or a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) +
                                                                  " depending on the mass of the conduit content.");

                    public static LocString LOGIC_PORT = (LocString) "Mass";

                    public static LocString LOGIC_PORT_ACTIVE = (LocString) ("Sends a " +
                                                                             UI.FormatAsAutomationState("Green Signal",
                                                                                 UI.AutomationState.Active) +
                                                                             " if mass is above the selected level");

                    public static LocString LOGIC_PORT_INACTIVE = (LocString) ("Otherwise, sends a " +
                                                                               UI.FormatAsAutomationState("Red Signal",
                                                                                   UI.AutomationState.Standby));
                }

                public class TMLSOLIDCONDUITMASSSENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Conveyor Rail Mass Sensor",
                        nameof(TmlConnectedElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Conveyor Rail Mass Sensors can output a logic signal when mass of content is withing desirable levels.";

                    public static LocString EFFECT = (LocString) ("Sends a " +
                                                                  UI.FormatAsAutomationState("Green Signal",
                                                                      UI.AutomationState.Active) + " or a " +
                                                                  UI.FormatAsAutomationState("Red Signal",
                                                                      UI.AutomationState.Standby) +
                                                                  " depending on the mass of the conduit content.");

                    public static LocString LOGIC_PORT = (LocString) "Mass";

                    public static LocString LOGIC_PORT_ACTIVE = (LocString) ("Sends a " +
                                                                             UI.FormatAsAutomationState("Green Signal",
                                                                                 UI.AutomationState.Active) +
                                                                             " if mass is above the selected level");

                    public static LocString LOGIC_PORT_INACTIVE = (LocString) ("Otherwise, sends a " +
                                                                               UI.FormatAsAutomationState("Red Signal",
                                                                                   UI.AutomationState.Standby));
                }
            }
        }
    }
}