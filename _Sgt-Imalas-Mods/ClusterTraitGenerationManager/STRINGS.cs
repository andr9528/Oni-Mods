﻿namespace ClusterTraitGenerationManager
{
    internal class STRINGS
    {
        public class MODCONFIG
        {
            public class MANUALCLUSTERPRESETS
            {
                public static LocString NAME = (LocString)"Automated Cluster Preset";
                public static LocString TOOLTIP = (LocString)"Each time you start a new CGM cluster, a cluster preset will be created automatically.\nif deactivated, there will be a button for manual creation on the start screen instead.";
            }
        }
        public class WORLD_TRAITS
        {
            public class CGM_RANDOMTRAIT
            {
                public static LocString NAME = (LocString)"Randomized Traits";
                public static LocString DESCRIPTION = (LocString)"Chooses between 1 and 3 Traits at random.\n(Between 0 and 2 for random planets)\nMutually exclusive with other selectable Traits.";
            }
        }
        public class UI
        {
            public class INFOTOOLTIPS
            {
                public static LocString INFO_ONLY = "These are for info only and are not configurable.";
            }
            public class GENERATIONWARNING
            {
                public static LocString WINDOWNAME = (LocString)"Potential Generation Errors detected!";
                public static LocString DESCRIPTION = (LocString)"You have selected more than 6 outer planets, which can lead to placement failures.\n Automatically adjust cluster size and placements?";
                public static LocString YES = (LocString)"Yes";
                public static LocString NOMANUAL = (LocString)"No, let me do it manually.";
            }

            public class CGM_MAINSCREENEXPORT
            {
                public class CATEGORIES
                {
                    public class HEADER
                    {
                        public static LocString LABEL = (LocString)"Starmap Item Categories";

                    }
                    public class FOOTERCONTENT
                    {
                        public class TITLE
                        {
                            public static LocString LABEL = (LocString)"World Settings";
                        }
                        public class STORYTRAITS
                        {
                            public static LocString LABEL = global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER;
                            public static LocString TOOLTIP = (LocString)"Open the game settings screen.";
                        }
                        public class GAMESETTINGS
                        {
                            public static LocString LABEL = global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.CUSTOMIZE;
                            public static LocString TOOLTIP = (LocString)"Open the game settings.";
                        }
                    }
                }
                public class ITEMSELECTION
                {
                    public class HEADER
                    {
                        public static LocString LABEL = (LocString)"[STARMAPITEMTYPEPL] in this category:";
                    }
                    public class VANILLASTARMAPCONTENT
                    {
                        public class VANILLASTARMAPCONTAINER
                        {
                            public class ADDMISSINGPOI
                            {
                                public static LocString LABEL = (LocString)"Add another POI group";
                                public static LocString LABEL_BASEGAME = (LocString)"Add missing POIs ({0} missing)";
                                public static LocString TOOLTIP = (LocString)"The following POI types are currently missing:{0}";
                            }
                            public class VANILLASTARMAPENTRYPREFAB
                            {
                                public class MININGWORLDSCONTAINER
                                {
                                    public class SCROLLAREA
                                    {
                                        public class CONTENT
                                        {
                                            public class ADDPOI
                                            {
                                                public static LocString LABEL = (LocString)"Add new POI";
                                            }
                                        }
                                    }
                                }
                            }

                            public class ADDNEWDISTANCEBUTTONCONTAINER
                            {
                                public class ADDDISTANCEROW
                                {
                                    public static LocString LABEL = (LocString)"Increase max. Distance";
                                }
                                public class REMOVEDISTANCEROW
                                {
                                    public static LocString LABEL = (LocString)"Reduce max. Distance";
                                }
                            }
                        }
                    }
                    public class FOOTER
                    {
                        public class TOOLBOX
                        {
                            public class TRASHCANCONTAINER
                            {
                                public class SHOWPERMALABELS
                                {
                                    public static LocString LABEL = (LocString)"Show Names";
                                }
                                public static LocString LABEL = (LocString)"Drag POI here to delete.";
                                public class INPUT
                                {
                                    public static LocString TEXT = (LocString)"";
                                }
                            }
                            public class BOXOFPOI
                            {
                                public static LocString LABEL = (LocString)"Drag POI here to delete.";
                                public static LocString POINOTINSTARMAP = (LocString)"This POI is currently not present on the starmap";
                                public class INPUT
                                {
                                    public class TEXTAREA
                                    {
                                        public static LocString TEXT = (LocString)"";
                                        public static LocString PLACEHOLDER = (LocString)"Enter Text to filter POIs";
                                    }
                                }
                            }
                        }
                    }
                }
                public class DETAILS
                {
                    public class HEADER
                    {
                        public static LocString LABEL = (LocString)"Currently selected [STARMAPITEMTYPE]: {0}";
                        public static LocString LABEL_LOCATION = (LocString)"Current [STARMAPITEMTYPE]: {0} at {1}";
                    }


                    public class CONTENT
                    {
                        public class SCROLLRECTCONTAINER
                        {
                            public class SO_POIGROUP_CONTAINER
                            {
                                public class GROUPHEADER
                                {
                                    public static LocString LABEL = (LocString)"POIs in this Group:";
                                }
                                public class POICONTAINER
                                {
                                    public class SCROLLAREA
                                    {
                                        public class CONTENT
                                        {
                                            public class NOPOIS
                                            {
                                                public static LocString LABEL = (LocString)"No POIs in this group...";
                                            }
                                        }
                                    }
                                }
                                public class ADDPOIBUTTON
                                {
                                    public static LocString LABEL = (LocString)"Add new POI to group";
                                }
                            }
                            public class POI_ALLOWDUPLICATES
                            {
                                public static LocString LABEL = (LocString)"Allow Spawning Duplicates:";
                                public static LocString TOOLTIP = (LocString)"When enabled, a POI from the POI pool can generate multiple times from this POI group.";
                            }
                            public class POI_AVOIDCLUMPING
                            {
                                public static LocString LABEL = (LocString)"Avoid Clumping:";
                                public static LocString TOOLTIP = (LocString)"When enabled, POIs generated from this group cannot generate adjacent to other POIs.";
                            }
                            public class SO_POIGROUP_REMOVE
                            {
                                public static LocString LABEL = (LocString)"Delete this POI Group";
                                public static LocString TOOLTIP = (LocString)"Delete the currently selected POI group from the custom cluster.";

                            }
                            public class STORYTRAIT
                            {
                                public class STORYTRAITENABLED
                                {
                                    public static LocString LABEL = (LocString)"Generate Story Trait:";
                                    public static LocString TOOLTIP = (LocString)"Should this Story Trait be generated?";
                                }
                            }
                            public class VANILLAPOI_RESOURCES
                            {
                                public static LocString NONESELECTED = (LocString)"None";
                                public static LocString SELECTEDDISTANCE = (LocString)"{0} at {1} {2}";
                                public static LocString SELECTEDDISTANCE_SO = (LocString)"{0} in Group {1}";
                                public static LocString DISTANCELABEL_DLC = (LocString)"POI-Group {0}\nSpawns {1} of these POIs at Distance {2} to {3}:";
                                public class RESOURCEHEADER
                                {
                                    public static LocString LABEL = (LocString)"Resources:";
                                }
                                public class CONTENT
                                {
                                    public class RESOURCECONTAINER
                                    {
                                        public class SCROLLAREA
                                        {
                                            public class CONTENT
                                            {
                                                public class NORESOURCES
                                                {
                                                    public static LocString LABEL = (LocString)"No Resources here..";
                                                }
                                                public class LISTVIEWENTRYPREFAB
                                                {
                                                    public class BIOLABEL
                                                    {
                                                        public static LocString LABEL = (LocString)"Bio-Resource";
                                                        public static LocString TOOLTIP = (LocString)"This resource requires a biological cargo bay to collect.";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                public class VANILLAPOI_REMOVE
                                {
                                    public class DELETEPOI
                                    {
                                        public static LocString TEXT = (LocString)"Remove this POI";
                                    }
                                }

                                public class MODIFYPOIBTN
                                {
                                    public class MODIFYPOI
                                    {
                                        public static LocString TEXT = (LocString)"Modify this POI Type";
                                    }
                                }
                                public class CAPACITY
                                {
                                    public static LocString LABEL = (LocString)"Mineable Mass:";
                                }
                                public class REPLENISMENT
                                {
                                    public static LocString LABEL = (LocString)"Replenishment per cycle:";
                                }
                                public class VANILLAPOI_ARTIFACT
                                {
                                    public static LocString LABEL = (LocString)"Artifact Rarity:";
                                    public class ARTIFACTRATES
                                    {
                                        public static LocString NONE = "None";
                                        public static LocString BAD = "Bad";
                                        public static LocString MEDIOCRE = "Mediocre";
                                        public static LocString GOOD = "Good";
                                        public static LocString GREAT = "Great";
                                        public static LocString AMAZING = "Amazing";
                                        public static LocString PERFECT = "Perfect";
                                        public static LocString DLC_NO = "No Artifacts";
                                        public static LocString DLC_YES = "Produces Artifacts";
                                    }
                                }
                            }

                            public class STARMAPITEMENABLED
                            {
                                public static LocString LABEL = (LocString)"Generate [STARMAPITEMTYPE]:";
                                public static LocString TOOLTIP = (LocString)"Should this starmap item be generated at all?";
                            }
                            public class AMOUNTSLIDER
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Number to generate:";
                                    public static LocString TOOLTIP = (LocString)"How many instances of this starmap item should be generated.\nValues that aren't full numbers represent a chance to generate for POIs.\n(f.e. 0.8 = 80% chance to generate this POI)";
                                    public static LocString OUTPUT = (LocString)"0";
                                    public class INPUT
                                    {
                                        public static LocString TEXT = (LocString)"";

                                    }
                                }
                            }
                            public class AMOUNTOFCLASSICPLANETS
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Max. Number of Classic planets:";
                                    public static LocString TOOLTIP = (LocString)"How many of the random planets are allowed to be \"classic\" size.\nA large number of classic size asteroids can impact performance.\nThis check includes start and warp planet in its calculcations.";
                                    public static LocString OUTPUT = (LocString)"0";
                                    public class INPUT
                                    {
                                        public static LocString TEXT = (LocString)"";

                                    }
                                }
                            }
                            public class MINMAXDISTANCE
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Distance to Center:";
                                    public static LocString TOOLTIP = (LocString)"The minimum and maximum distance to the center of the starmap the starmap item can generate with.\nSetting the range to 0 - 0 will always spawn the starmap item the center of the map.\nSetting the range to 3 - 8 will spawn the starmap item randomly between 3 and 8 hexes away from the center of the starmap.";

                                    public static LocString FORMAT = "Between {0} and {1}";
                                    public static LocString OUTPUT = (LocString)"0";
                                    public class INPUT
                                    {
                                        public static LocString TEXT = (LocString)"";
                                    }
                                }
                            }
                            public class BUFFERSLIDER
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Buffer Distance:";
                                    public static LocString TOOLTIP = (LocString)"The minimum distance this asteroid has to other asteroids.";
                                    public class INPUT
                                    {
                                        public static LocString TEXT = (LocString)"";

                                    }
                                    public static LocString OUTPUT = (LocString)"0";
                                }
                            }


                            public class ASTEROIDSIZE
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Asteroid Size:";
                                    public static LocString TOOLTIP = (LocString)"The dimensions of this asteroid.";
                                }
                                public class CONTENT
                                {
                                    public class INFO
                                    {
                                        public class HEIGHTLABEL
                                        {
                                            public static LocString LABEL = (LocString)"Height:";
                                        }
                                        public class WIDTHLABEL
                                        {
                                            public static LocString LABEL = (LocString)"Width:";
                                        }
                                    }
                                }
                                public static LocString SIZEWARNING = (LocString)"Warning!\nThe planet size you have selected has {0}% more area than a normal vanilla size asteroid.\nThis might lead to low game performance!";
                                public static LocString BIOMEMISSINGWARNING = (LocString)"Warning!\nThe selected planet dimensions are too small, making it very likely to fail worldgen.\nIncrease them to avoid that!";


                                public class SIZESELECTOR
                                {

                                    public static LocString NEGSIZE0 = (LocString)"Tiny";
                                    public static LocString NEGSIZE0TOOLTIP = (LocString)"The asteroid is at 30% of its usual size.";
                                    public static LocString NEGSIZE1 = (LocString)"Smaller";
                                    public static LocString NEGSIZE1TOOLTIP = (LocString)"The asteroid is at 45% of its usual size.";
                                    public static LocString NEGSIZE2 = (LocString)"Small";
                                    public static LocString NEGSIZE2TOOLTIP = (LocString)"The asteroid is at 60% of its usual size.";
                                    public static LocString NEGSIZE3 = (LocString)"Slightly Smaller";
                                    public static LocString NEGSIZE3TOOLTIP = (LocString)"The asteroid is at 80% of its usual size.";

                                    public static LocString SIZE0 = (LocString)"Normal";
                                    public static LocString SIZE0TOOLTIP = (LocString)"The asteroid is at its usual size.";
                                    public static LocString SIZE1 = (LocString)"Slightly Larger";
                                    public static LocString SIZE1TOOLTIP = (LocString)"The asteroid is 25% larger than normal.";
                                    public static LocString SIZE2 = (LocString)"Large";
                                    public static LocString SIZE2TOOLTIP = (LocString)"The asteroid is 50% larger than normal.";
                                    public static LocString SIZE3 = (LocString)"Huge";
                                    public static LocString SIZE3TOOLTIP = (LocString)"The asteroid has twice its usual size.";
                                    public static LocString SIZE4 = (LocString)"Massive";
                                    public static LocString SIZE4TOOLTIP = (LocString)"The asteroid has three times its usual size.";
                                    public static LocString SIZE5 = (LocString)"Enormous";
                                    public static LocString SIZE5TOOLTIP = (LocString)"The asteroid has four times its usual size.";
                                }
                                public class RATIOSELECTOR
                                {

                                    public static LocString NORMAL = (LocString)"Normal Shape";
                                    public static LocString NORMALTOOLTIP = (LocString)"The asteroid has its usual shape.";
                                    public static LocString WIDE1 = (LocString)"Slightly Wider";
                                    public static LocString WIDE1TOOLTIP = (LocString)"The asteroid is a bit wider than normal.";
                                    public static LocString WIDE2 = (LocString)"Wider";
                                    public static LocString WIDE2TOOLTIP = (LocString)"The asteroid is wider than normal.";
                                    public static LocString WIDE3 = (LocString)"Much Wider";
                                    public static LocString WIDE3TOOLTIP = (LocString)"The asteroid is a lot wider than normal.";

                                    public static LocString HEIGHT1 = (LocString)"Slightly Taller";
                                    public static LocString HEIGHT1TOOLTIP = (LocString)"The asteroid is a bit taller than normal.";

                                    public static LocString HEIGHT2 = (LocString)"Taller";
                                    public static LocString HEIGHT2TOOLTIP = (LocString)"The asteroid is taller than normal.";

                                    public static LocString HEIGHT3 = (LocString)"Much Taller";
                                    public static LocString HEIGHT3TOOLTIP = (LocString)"The asteroid is a lot taller than normal.";

                                }


                                public class INPUT
                                {
                                    public static LocString TEXT = (LocString)"";

                                }
                            }

                            public class ASTEROIDTRAITS
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Asteroid Traits:";
                                }
                                public class CONTENT
                                {
                                    public class TRAITCONTAINER
                                    {
                                        public class SCROLLAREA
                                        {
                                            public class CONTENT
                                            {
                                                public class NOTRAITS
                                                {
                                                    public static LocString LABEL = (LocString)"No Traits";
                                                }
                                                public class LISTVIEWENTRYPREFAB
                                                {
                                                    public class AWAILABLERANDOMTRAITS
                                                    {
                                                        public static LocString LABEL = (LocString)"Blacklist Traits";
                                                        public static LocString TOOLTIP = (LocString)"Disable Traits you want to not show up as random traits.";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    public class ADDSEASONBUTTON
                                    {
                                        public static LocString TEXT = (LocString)"Add Trait";
                                    }
                                }
                                public class ADDTRAITBUTTON
                                {
                                    public static LocString TEXT = (LocString)"Add Trait";
                                }
                            }


                            public class METEORSEASONCYCLE
                            {
                                public class DESCRIPTOR
                                {
                                    public static LocString LABEL = (LocString)"Meteors:";
                                    public static LocString TOOLTIP = (LocString)"What kind of meteors should come down on this asteroid?";
                                }

                                public static LocString NOSEASONSSELECTED = "No season types selected";
                                public static LocString ADDNEWSEASON = (LocString)"Add additional meteor season";
                                public static LocString ADDNEWSEASONTOOLTIP = (LocString)"Add another type of meteor season type to this asteroid.\nWarning: Meteor season types are all active at the same time,\nleading to a high volume of meteors at the same time if multiple are added.\nNormal asteroids have usually one season type.";
                                public static LocString ACTIVESEASONSELECTORLABEL = (LocString)"Active Meteor Seasons:";
                                public static LocString ACTIVEMETEORSLABEL = (LocString)"Active Meteor Showers:";

                                public class CONTENT
                                {
                                    public class SEASONS
                                    {
                                        public class SEASONSCROLLAREA
                                        {
                                            public class CONTENT
                                            {
                                                public class NOSEASONSELECTED
                                                {
                                                    //public static LocString LABEL = (LocString)"No Seasons selected";
                                                    public static LocString LABEL = (LocString)"";
                                                }
                                                public class ADDSEASONBUTTON
                                                {
                                                    public static LocString TEXT = (LocString)"Add new season type";
                                                }
                                            }
                                        }
                                    }
                                    public class ASTEROIDS
                                    {
                                        public class SCROLLAREA
                                        {
                                            public class CONTENT
                                            {
                                                public class NOMETEORSAVAILABLE
                                                {
                                                    public static LocString LABEL = (LocString)"No Meteor Showers";
                                                }
                                            }
                                        }
                                    }

                                    public static LocString TITLE = (LocString)"Available Meteor Season Types:";
                                    public static LocString NOSEASONTYPESAVAILABLE = (LocString)"No more available Season Types";
                                    public static LocString SEASONTYPETOOLTIP = (LocString)"This season type contains the following shower types:";
                                    public static LocString SEASONTYPENOMETEORSTOOLTIP = (LocString)"This season type does not contain any meteor shower types.\nThis might change in the future if Klei decides to add some showers to this season type.\nFor this reason the season type is listed here.";
                                }

                                public static LocString SWITCHTOOTHERSEASONTOOLTIP = (LocString)"Swap this season to a different type";
                                public static LocString REMOVESEASONTOOLTIP = (LocString)"Remove this season type";

                                public static LocString SHOWERTOOLTIP = (LocString)"Meteor type present in these shower types:";
                                public static LocString VANILLASEASON = (LocString)"Vanilla Meteor Showers";
                                public static LocString FULLERENETOOLTIP = (LocString)"One time shower event when opening the tear.";
                            }

                        }

                    }

                    public class FOOTER
                    {
                        public class CLUSTERSIZESLIDER
                        {
                            public class DESCRIPTOR
                            {
                                public static LocString TOOLTIP = (LocString)"The radius of the starmap.";
                                public static LocString LABEL = (LocString)"Cluster Size (Radius):";
                                public static LocString OUTPUT = (LocString)"0";
                                public class INPUT
                                {
                                    public static LocString TEXT = (LocString)"";

                                }
                            }
                        }

                        public class BUTTONS
                        {
                            public class RESETCLUSTERBUTTON
                            {
                                public static LocString TEXT = (LocString)"Reset Everything";
                                public static LocString TOOLTIP = (LocString)"Undo all changes you have made by reloading the cluster preset.";
                                public static LocString TOOLTIP_VANILLA = (LocString)"Undo all changes you have made by reloading the world preset.";
                            }
                            public class RESETSELECTIONBUTTON
                            {
                                public static LocString TEXT = (LocString)"Reset Selected Item";
                                public static LocString TOOLTIP = (LocString)"Undo all changes you have made to the currently selected item.";
                            }
                            public class STARMAPBUTTON
                            {
                                public static LocString TEXT = (LocString)"Reset Starmap";
                                public static LocString TOOLTIP = (LocString)"Undo all changes you have made to the starmap.";
                            }
                            public class RETURNBUTTON
                            {
                                public static LocString TEXT = (LocString)"Return";
                                public static LocString TOOLTIP = (LocString)"Return to the previous screen.";
                            }
                            public class PRESETBUTTON
                            {
                                public static LocString TEXT = (LocString)"Cluster Presets";
                                public static LocString TOOLTIP = (LocString)"Create new or load your existing cluster presets";
                            }
                            public class GENERATECLUSTERBUTTON
                            {
                                public static LocString TEXT = (LocString)"Start modified Game";
                                public static LocString TOOLTIP = (LocString)"Start generating a modified Cluster based on selected parameters.\nModified Cluster Generation is only activated if this button here is used.";
                                public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED = "The current asteroid placement rules do not allow a spot for all asteroids on the starmap.\nPlease adjust your asteroid placement rules or reduce the total amount of asteroids!";
                                public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_ASTEROID = "The current asteroid placement rules for the {0} currently fail to place the asteroid on the starmap.\nPlease adjust its placements or reduce the total amount of asteroids!";
                                public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_COMETS = "The current asteroid placement rules do not allow meteors to reach the {0}.\nThis would cause a crash on the spawn of the first meteor shower that tries to target this unreachable asteroid.\nPlease adjust your starmap placements!";
                                public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_TEAR = "There is currently no temporal tear on the starmap.\nPlease add a temporal tear to the starmap!";
                            }
                        }
                    }
                }
            }


            public class CGMEXPORT_SIDEMENUS
            {
                public class PRESETWINDOWCGM
                {
                    public class DELETEWINDOW
                    {
                        public static LocString TITLE = "Delete {0}";
                        public static LocString DESC = "You are about to delete the preset \"{0}\".\nDo you want to continue?";
                        public static LocString YES = "Confirm Deletion";
                        public static LocString CANCEL = "Cancel";

                    }

                    public static LocString TITLE = "Cluster Presets";

                    public class HORIZONTALLAYOUT
                    {
                        public class OBJECTLIST
                        {
                            public class SCROLLAREA
                            {
                                public class CONTENT
                                {
                                    public class NOPRESETSAVAILABLE
                                    {
                                        public static LocString LABEL = "No presets available";
                                    }
                                    public class PRESETENTRYPREFAB
                                    {
                                        public class ADDTHISTRAITBUTTON
                                        {
                                            public static LocString TEXT = "Load Preset";
                                            public static LocString TOOLTIP = "Load this preset to the preview";

                                        }

                                        public static LocString RENAMEPRESETTOOLTIP = "Rename Preset";
                                        public static LocString DELETEPRESETTOOLTIP = "Delete Preset";

                                    }
                                }
                            }

                            internal class SEARCHBAR
                            {
                                public static LocString CLEARTOOLTIP = "Clear search bar";
                                public static LocString OPENFOLDERTOOLTIP = "Open the folder where the presets are stored.";
                                internal class INPUT
                                {
                                    public class TEXTAREA
                                    {
                                        public static LocString PLACEHOLDER = "Enter text to filter presets...";
                                        public static LocString TEXT = "";
                                    }
                                }
                            }
                        }
                        public class ITEMINFO
                        {
                            public class BUTTONS
                            {
                                public class CLOSEBUTTON
                                {
                                    public static LocString TEXT = "Return";
                                    public static LocString TOOLTIP = "Close this preset window";
                                }
                                public class GENERATEFROMCURRENT
                                {
                                    public static LocString TEXT = "Generate new Preset";
                                    public static LocString TEXT_STARTSCREEN = "Cluster Preset";
                                    public static LocString TOOLTIP = "Save the currently loaded cluster configuration to a new preset.";
                                }
                                public class APPLYPRESETBUTTON
                                {
                                    public static LocString TEXT = "Apply Preset";
                                    public static LocString TOOLTIP = "Apply the preset thats currently displayed in the preview to the custom cluster.";
                                }
                            }
                        }
                    }
                }
                public class TRAITPOPUP
                {
                    public static LocString TEXT = (LocString)"available Traits:";
                    public class SCROLLAREA
                    {
                        public class CONTENT
                        {
                            public class NOTRAITAVAILABLE
                            {
                                public static LocString LABEL = (LocString)"No Traits available";

                            }
                            public class LISTVIEWENTRYPREFAB
                            {
                                public static LocString LABEL = (LocString)"trait label";
                                public class ADDTHISTRAITBUTTON
                                {
                                    public static LocString TEXT = (LocString)"Add this trait";

                                }
                                public class TOGGLETRAITBUTTON
                                {
                                    public static LocString ADDTOBLACKLIST = "Disable as Random";
                                    public static LocString ADDTOBLACKLISTTOOLTIP = "Prevent the Trait from generating as a random trait";
                                    public static LocString REMOVEFROMBLACKLIST = "Enable as Random";
                                    public static LocString REMOVEFROMBLACKLISTTOOLTIP = "Allows the Trait generating as a random trait";

                                }
                            }
                        }
                    }
                    public class CANCELBUTTON
                    {
                        public static LocString TEXT = (LocString)"Close";
                    }
                }
            }


            public class CGMBUTTON
            {
                public static LocString DESC = (LocString)"Start customizing the currently selected cluster.";
            }
            public static class CATEGORYENUM
            {
                public static LocString START = (LocString)"Start Asteroid";
                public static LocString WARP = (LocString)"Teleport Asteroid";
                public static LocString OUTER = (LocString)"Outer Asteroids";
                public static LocString POI = (LocString)"Points of Interest";
                public static LocString VANILLASTARMAP = (LocString)"Starmap";
            }
            public static class STARMAPITEMDESCRIPTOR
            {
                public static LocString ASTEROID = (LocString)"Asteroid";
                public static LocString ASTEROIDPLURAL = (LocString)"Asteroids";

                public static LocString POI = (LocString)"Point of Interest";
                public static LocString POI_GROUP = (LocString)"POI Group";
                public static LocString POI_GROUP_PLURAL = (LocString)"POI Groups";
                public static LocString POIPLURAL = (LocString)"Points of Interest";

                public static LocString STORYTRAIT = (LocString)"Story Trait";
                public static LocString STORYTRAITPLURAL = (LocString)"Story Traits";


                public static LocString NOPOISAVAILABLE = (LocString)"No more available POI types.";
            }

            public class SEEDLOCK
            {
                public static LocString NAME = (LocString)"Seed rerolling affects traits";
                public static LocString NAME_SHORT = (LocString)"rerolling Traits:";
                public static LocString NAME_STARMAP = (LocString)"rerolling Starmap:";
                public static LocString SEED_PLACEHOLDER = (LocString)"Enter Seed...";
                public static LocString TOOLTIP = (LocString)"When enabled, rerolling the seed will also reroll the planet traits to those of the new seed.\nDisable to reroll the seed without affecting the traits.\nOnly blocks trait rerolling for the seed setting above.";
                public static LocString TOOLTIP_STARMAP = (LocString)"When enabled, rerolling the seed will also reroll the starmap to the new seed.\nDisable to reroll the seed without affecting the starmap.";
            }

            public class SPACEDESTINATIONS
            {
                public static LocString MODDEDPLANET = "(Modded)";
                public static LocString MODDEDPLANETDESC = "Added by the mod \"{0}\"";
                public static class CGM_RANDOM_STARTER
                {
                    public static LocString NAME = (LocString)"Random Start Asteroid";
                    public static LocString DESCRIPTION = (LocString)"The starting asteroid will be picked at random";
                }
                public static class CGM_RANDOM_WARP
                {
                    public static LocString NAME = (LocString)"Random Teleporter Asteroid";
                    public static LocString DESCRIPTION = (LocString)"The teleporter asteroid will be picked at random";
                }
                public static class CGM_RANDOM_OUTER
                {
                    public static LocString NAME = (LocString)"Random Outer Asteroid(s)";
                    public static LocString DESCRIPTION = (LocString)"Choose an amount of random outer asteroids.\n\nEach asteroid can only generate once";
                }
                public class CGM_RANDOM_POI
                {
                    public static LocString NAME = "Random POI";
                    public static LocString DESCRIPTION = "Chooses a random POI during worldgen.\n\nDoes not roll unique POIs\n(Temporal Tear, Russel's Teapot)";
                }
            }
        }
        public class CLUSTER_NAMES
        {
            public class CGM
            {
                public static LocString NAME = "CGM Custom Cluster";
                public static LocString DESCRIPTION = "This cluster has been handcrafted in the Cluster Generation Manager";
            }
        }
        public class ERRORMESSAGES
        {
            public static LocString PLANETPLACEMENTERROR = "Starmap Generation Error!\n{0} could not be placed on the star map. Increase the maximum distance of of this asteroid to fix this issue.";
            public static LocString MISSINGWORLD = "Missing Worlds!\nThe preset cannot be loaded since its start world is missing.";
        }
    }
}

