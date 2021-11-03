using Jotunn.Configs;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SageRobes
{
    public class Skill
    {
        public static Skills.SkillType ArtifactSkillType = 0;

        public static void CreateSkill()
        {
            ArtifactSkillType = SkillManager.Instance.AddSkill(new SkillConfig
            {
                Identifier = "Artifact",
                Name = "Artifact",
                Description = "Increase magic damage!",
                IncreaseStep = 1f
            });
        }

        public static void SetArtifactSkillType()
        {
            foreach (string artifact in SageRobes.MyBundleName.artifactList) 
            {
                string prefab = artifact.Replace(" ", "");
                ItemDrop item = ObjectDB.instance.GetItemPrefab(prefab).GetComponent<ItemDrop>();
                item.m_itemData.m_shared.m_skillType = ArtifactSkillType;
                Debug.LogError(item.m_itemData.m_shared.m_skillType);
            }
        }
    }
}
