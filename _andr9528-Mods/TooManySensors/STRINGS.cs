using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoryOverload
{
    internal class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class LOGICELEMENTTEMPERATURESENSOR
                {
                    public static LocString NAME = (LocString) UI.FormatAsLink("Thermo Element Sensor",
                        nameof(LogicElementTemperatureSensor));

                    public static LocString DESC =
                        (LocString)
                        "Thermo Element sensors can disable buildings when average temperature of connected elements approach dangerous temperatures.";

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
                }
            }
        }
    }
}