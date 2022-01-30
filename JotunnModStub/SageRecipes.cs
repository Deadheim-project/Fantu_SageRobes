using Jotunn.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SageRobes
{
    public class SageRecipes
    {
        public static RequirementConfig[] RobeRequirements = new RequirementConfig[3]
                {
                    new RequirementConfig
                    {
                        Item = "LinenThread",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "TrollHide",
                        Amount = 30,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "GreydwarfEye",
                        Amount = 10,
                        AmountPerLevel = 5
                    },

                };
        public static RequirementConfig[] CrownRequirements = new RequirementConfig[3]
        {
                    new RequirementConfig
                    {
                        Item = "Silver",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Ruby",
                        Amount = 1,
                        AmountPerLevel = 1
                    },
                    new RequirementConfig
                    {
                        Item = "Crystal",
                        Amount = 5,
                        AmountPerLevel = 5
                    },

        };
        public static RequirementConfig[] ArtifactRequirements = new RequirementConfig[4]
        {
                    new RequirementConfig
                    {
                        Item = "DragonTear",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Silver",
                        Amount = 60,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "Crystal",
                        Amount = 30,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "ElderBark",
                        Amount = 20,
                        AmountPerLevel = 10
                    }
        };
        public static RequirementConfig[] TomeRequirements = new RequirementConfig[3]
        {
                    new RequirementConfig
                    {
                        Item = "LeatherScraps",
                        Amount = 10,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "LinenThread",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Silver",
                        Amount = 5,
                        AmountPerLevel = 1
                    },
        };
        public static RequirementConfig[] BladeRequirements = new RequirementConfig[3]
        {
                    new RequirementConfig
                    {
                        Item = "Needle",
                        Amount = 5,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "BlackMetal",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "SilverNecklace",
                        Amount = 1,
                        AmountPerLevel = 1
                    },
        };
        public static RequirementConfig[] headRequirements = new RequirementConfig[2]
        {
                    new RequirementConfig
                    {
                        Item = "TankardOdin",
                        Amount = 5,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "CapeTest",
                        Amount = 10,
                        AmountPerLevel = 5
                    }
        };
        public static RequirementConfig[] scholarstaffRequirements = new RequirementConfig[4]
        {
                    new RequirementConfig
                    {
                        Item = "GreydwarfEye",
                        Amount = 50,
                        AmountPerLevel = 25
                    },
                    new RequirementConfig
                    {
                        Item = "Bronze",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Ruby",
                        Amount = 5,
                        AmountPerLevel = 2
                    },
                    new RequirementConfig
                    {
                        Item = "FineWood",
                        Amount = 20,
                        AmountPerLevel = 10
                    }
        };

        public static RequirementConfig[] ElvenArmorRequirements = new RequirementConfig[3]
        {
                    new RequirementConfig
                    {
                        Item = "Silver",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Ruby",
                        Amount = 1,
                        AmountPerLevel = 1
                    },
                    new RequirementConfig
                    {
                        Item = "Crystal",
                        Amount = 5,
                        AmountPerLevel = 5
                    },
        };

        public static RequirementConfig[] ElvenWeaponRequirements = new RequirementConfig[3]
        {
                    new RequirementConfig
                    {
                        Item = "Silver",
                        Amount = 10,
                        AmountPerLevel = 5
                    },
                    new RequirementConfig
                    {
                        Item = "Ruby",
                        Amount = 1,
                        AmountPerLevel = 1
                    },
                    new RequirementConfig
                    {
                        Item = "Crystal",
                        Amount = 5,
                        AmountPerLevel = 5
                    },

        };

    }
}
