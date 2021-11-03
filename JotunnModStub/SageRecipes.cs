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
        public static RequirementConfig[] ArtifactRequirements = new RequirementConfig[3]
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
    }
}
