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

        public static void SetArtifactSkillType(ItemDrop ItemDrop)
        {
            ItemDrop.m_itemData.m_shared.m_skillType = ArtifactSkillType;
            Debug.LogError(ItemDrop.m_itemData.m_shared.m_skillType);
        }
    }
}
