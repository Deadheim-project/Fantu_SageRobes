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

        public const string PluginGUID = "fantu.sagesvault";
        public const string PluginName = "SagesVault";
        public const string PluginVersion = "1.1.3";
        private Harmony _harmony;
        public static AssetBundle ArmorBundle;
        public static List<string> robeList = new List<string> { "Sage Robe Black", "Sage Robe Blue", "Sage Robe Brown", "Sage Robe Gray", "Sage Robe Green", "Sage Robe Red", "Sage Robe White", "Sage Tunic Black", "Sage Tunic Blue", "Sage Tunic Brown", "Sage Tunic Gray", "Sage Tunic Green", "Sage Tunic Red", "Sage Tunic White", "Sage Hood Black", "Sage Hood Blue", "Sage Hood Brown", "Sage Hood Gray", "Sage Hood Green", "Sage Hood Red", "Sage Hood White", "Sage Pants" };
        public static List<string> crownList = new List<string> { "Sage Crown Gold 01", "Sage Crown Gold 02", "Sage Crown Gold 03", "Sage Crown Silver 01", "Sage Crown Silver 02", "Sage Crown Silver 03", "Sage Crown Obsidian 01", "Sage Crown Obsidian 02", "Sage Crown Obsidian 03" };
        public static List<string> tomeList = new List<string> { "Sage Book 01", "Sage Book 02", "Sage Book 03", "Sage Book 04" };
        public static List<string> bladeList = new List<string> { "Needle Blade", "Death Blade" };
        public static List<string> headList = new List<string> { "Head Elf" };
        public static List<string> scholarstaffList = new List<string> { "Scholar Fire Staff" };
        public static List<string> artifactList = new List<string> { "Sage Fire Staff", "Sage Holy Staff", "Sage Ice Staff", "Sage Lightning Staff", "Sage Dark Staff" };
        public static List<string> elvenarmorList = new List<string> { "Elven Greaves Emerald", "Elven Plate Emerald", "Elven Helmet Emerald" };
        public static List<string> elvenweaponList = new List<string> { "Elven Battleaxe Emerald", "Elven Warhammer Emerald", "Elven Dagger Emerald", "Elven Lance Emerald", "Elven Shield Emerald", "Elven Staff Emerald", "Elven Sword Emerald", "Elven Bow Emerald" };

        private void Awake()
        {
            Skill.CreateSkill();
//            LoadSagesVaultAssets();
            PrefabManager.OnVanillaPrefabsAvailable += LoadSageRobes;

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
        }

        private void LoadSageRobes()
        {
            ArmorBundle = GetAssetBundleFromResources("sagerobes");
            Debug.Log("Load robeList");
            robeList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.RobeRequirements));
            Debug.Log("Load crownList");
            crownList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.CrownRequirements));
            Debug.Log("Load tomeList");
            tomeList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.TomeRequirements));
            Debug.Log("Load bladeList");
            bladeList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.BladeRequirements));
            Debug.Log("Load headList");
            headList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.headRequirements));
            Debug.Log("Load scholarList");
            scholarstaffList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.scholarstaffRequirements));
            Debug.Log("Load artifactList");
            artifactList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.ArtifactRequirements, true));
            Debug.Log("Load elvenarmorList");
            elvenarmorList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.ElvenArmorRequirements, true));
            Debug.Log("Load elvenweaponList");
            elvenweaponList.ForEach(x => AddItemsWithRenderedIcons(x, SageRecipes.ElvenWeaponRequirements, true));
            Debug.Log("Load Projectiles");
            GameObject IceProjectile = ArmorBundle.LoadAsset<GameObject>("sage_cold_projectile");
            GameObject FireProjectile = ArmorBundle.LoadAsset<GameObject>("sage_fireball_projectile");
            GameObject LightningProjectile = ArmorBundle.LoadAsset<GameObject>("sage_lightning_projectile");
            GameObject SkullProjectile = ArmorBundle.LoadAsset<GameObject>("sage_skull_projectile");
            GameObject SpiritProjectile = ArmorBundle.LoadAsset<GameObject>("sage_spirit_projectile");
            PrefabManager.Instance.AddPrefab(IceProjectile);
            PrefabManager.Instance.AddPrefab(FireProjectile);
            PrefabManager.Instance.AddPrefab(LightningProjectile);
            PrefabManager.Instance.AddPrefab(SkullProjectile);
            PrefabManager.Instance.AddPrefab(SpiritProjectile);

            Debug.Log("Load VFX");
            Debug.Log("Load vfx_sagespell_hit");
            GameObject VfxSageSpell = ArmorBundle.LoadAsset<GameObject>("vfx_sagespell_hit");
            PrefabManager.Instance.AddPrefab(VfxSageSpell);
            Debug.Log("Load vfx_SageFire_AoE1");
            GameObject VfxSageFireAoE1 = ArmorBundle.LoadAsset<GameObject>("vfx_SageFire_AoE1");
            PrefabManager.Instance.AddPrefab(VfxSageFireAoE1);
            Debug.Log("Load vfx_SageFire_AoE2");
            GameObject VfxSageFireAoE2 = ArmorBundle.LoadAsset<GameObject>("vfx_SageFire_AoE2");
            PrefabManager.Instance.AddPrefab(VfxSageFireAoE2);
            Debug.Log("Load vfx_SageFireballHit");
            GameObject VfxSageFireballHit = ArmorBundle.LoadAsset<GameObject>("vfx_SageFireballHit");
            PrefabManager.Instance.AddPrefab(VfxSageFireballHit);
            Debug.Log("Load vfx_SageGhost_death");
            GameObject VfxSageGhostDeath = ArmorBundle.LoadAsset<GameObject>("vfx_SageGhost_death");
            PrefabManager.Instance.AddPrefab(VfxSageGhostDeath);
            Debug.Log("Load vfx_SageGhost_hit");
            GameObject VfxSageGhostHit = ArmorBundle.LoadAsset<GameObject>("vfx_SageGhost_hit");
            PrefabManager.Instance.AddPrefab(VfxSageGhostHit);
            Debug.Log("Load vfx_sageice_aoe");
            GameObject VfxSageIceAoe = ArmorBundle.LoadAsset<GameObject>("vfx_sageice_aoe");
            PrefabManager.Instance.AddPrefab(VfxSageIceAoe);
            Debug.Log("Load vfx_SageSpirit_attack");
            GameObject VfxSageSpiritAttack = ArmorBundle.LoadAsset<GameObject>("vfx_SageSpirit_attack");
            PrefabManager.Instance.AddPrefab(VfxSageSpiritAttack);
            Debug.Log("Load vfx_SageTerra_hit");
            GameObject VfxSageTerraHit = ArmorBundle.LoadAsset<GameObject>("vfx_SageTerra_hit");
            PrefabManager.Instance.AddPrefab(VfxSageTerraHit);
        }
        //        private void LoadSagesVaultAssets()
        //        {
        //            ProjectileAssets = GetAssetBundleFromResources("sagerobes");
        //            GameObject IceProjectile = ArmorBundle.LoadAsset<GameObject>("sage_cold_projectile");
        //            GameObject FireProjectile = ArmorBundle.LoadAsset<GameObject>("sage_fireball_projectile");
        //            GameObject LightningProjectile = ArmorBundle.LoadAsset<GameObject>("sage_lightning_projectile");
        //            GameObject SkullProjectile = ArmorBundle.LoadAsset<GameObject>("sage_skull_projectile");
        //            GameObject SpiritProjectile = ArmorBundle.LoadAsset<GameObject>("sage_spirit_projectile");
        //            PrefabManager.Instance.AddPrefab(IceProjectile);
        //            PrefabManager.Instance.AddPrefab(FireProjectile);
        //            PrefabManager.Instance.AddPrefab(LightningProjectile);
        //            PrefabManager.Instance.AddPrefab(SkullProjectile);
        //            PrefabManager.Instance.AddPrefab(SpiritProjectile);
        //        }

        public void AddItemsWithRenderedIcons(string itemName, RequirementConfig[] requirements, bool setSkill = false)
        {
            try
            {
                GameObject sagerobes = ArmorBundle.LoadAsset<GameObject>(itemName.Replace(" ", ""));
//                void CreateArmorRecipe(Sprite sprite)
                {
                    CustomItem customItem = new CustomItem(sagerobes, fixReference: false, new ItemConfig
                    {
                        Name = itemName,
                        Amount = 1,
                        //                        CraftingStation = itemName == "Scholar Fire Staff" ? "piece_workbench" : "piece_artisanstation",
                        CraftingStation = itemName == "Scholar Fire Staff" ? "piece_workbench" : "forge",
                        MinStationLevel = 3,
                        Icons = new[]
                            {
                            RenderManager.Instance.Render(new RenderManager.RenderRequest(sagerobes)
                            {
                                Rotation = RenderManager.IsometricRotation
                            }),
                        },
                        Requirements = requirements
                    });

                    ItemManager.Instance.AddItem(customItem);
                    if (setSkill) Skill.SetArtifactSkillType(customItem.ItemDrop);
                }
//                var SageIcons = new RenderManager.RenderRequest(sagerobes)
//                {
//                    Rotation = RenderManager.IsometricRotation,
//                };
//                RenderManager.Instance.EnqueueRender(rr, CreateBooks);
//                RenderManager.Instance.EnqueueRender(sagerobes, CreateArmorRecipe);
//                RenderManager.Instance.EnqueueRender(SageIcons, CreateArmorRecipe);
            }
            catch (Exception ex)
            {
                Jotunn.Logger.LogError($"Error while adding item with rendering: {ex}");
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

        [HarmonyPatch]

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}