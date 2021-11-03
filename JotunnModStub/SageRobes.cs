using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;


namespace SageRobes
{

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    [BepInDependency("com.jotunn.jotunn", BepInDependency.DependencyFlags.HardDependency)]

    internal class MyBundleName : BaseUnityPlugin
    {

        public const string PluginGUID = "fantu.sagerobes";
        public const string PluginName = "SageRobes";
        public const string PluginVersion = "0.0.1";
        private Harmony _harmony;
        public static AssetBundle ArmorBundle;
        List<string> robeList = new List<string> { "Sage Robe Black", "Sage Robe Blue", "Sage Robe Brown", "Sage Robe Gray", "Sage Robe Green", "Sage Robe Red", "Sage Robe White", "Sage Tunic Black", "Sage Tunic Blue", "Sage Tunic Brown", "Sage Tunic Gray", "Sage Tunic Green", "Sage Tunic Red", "Sage Tunic White", "Sage Hood Black", "Sage Hood Blue", "Sage Hood Brown", "Sage Hood Gray", "Sage Hood Green", "Sage Hood Red", "Sage Hood White" };
        List<string> crownList = new List<string> { "Sage Crown Gold 01", "Sage Crown Gold 02", "Sage Crown Gold 03", "Sage Crown Silver 01", "Sage Crown Silver 02", "Sage Crown Silver 03", "Sage Crown Obsidian 01", "Sage Crown Obsidian 02", "Sage Crown Obsidian 03" };
        public static List<string> artifactList = new List<string> { "Sage Fire Staff", "Sage Holy Staff", "Sage Ice Staff", "Sage Lightning Staff", "Sage Dark Staff", "Death Blade", "Sage Book 01", "Sage Book 02", "Sage Book 03", "Sage Book 04" };

        private void Awake()
        {
            Skill.CreateSkill();

            PrefabManager.OnVanillaPrefabsAvailable += LoadSageRobes;

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
        }

        private void LoadSageRobes()
        {
            ArmorBundle = GetAssetBundleFromResources("sagerobes");
            robeList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.RobeRequirements));
            crownList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.CrownRequirements));
            artifactList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.ArtifactRequirements, true));
        }

        public void AddItemsWithRenderedIcons(string itemName, RequirementConfig[] requirements, bool setSkill = false)
        {
            try
            {
                GameObject sagerobes = ArmorBundle.LoadAsset<GameObject>(itemName.Replace(" ", ""));
                void CreateArmorRecipe(Sprite sprite)
                {
                    CustomItem customItem = new CustomItem(sagerobes, fixReference: false, new ItemConfig
                    {
                        Name = itemName,
                        Amount = 1,
                        CraftingStation = "piece_workbench",
                        MinStationLevel = 1,
                        Icons = new[] { sprite },
                        Requirements = requirements
                    });

                    ItemManager.Instance.AddItem(customItem);
                    if (setSkill) Skill.SetArtifactSkillType(customItem.ItemDrop);
                }

                RenderManager.Instance.EnqueueRender(sagerobes, CreateArmorRecipe);
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Error while adding item with rsndering: {ex}");
            }
            finally
            {
                PrefabManager.OnVanillaPrefabsAvailable -= LoadSageRobes;
            }
        }


        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string text = executingAssembly.GetManifestResourceNames().Single((string str) => str.EndsWith(fileName));
            using Stream stream = executingAssembly.GetManifestResourceStream(text);
            return AssetBundle.LoadFromStream(stream);
        }


        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}