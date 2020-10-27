using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBlooper
{
    public class AchievementPanel : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private AchievementCard achievementCardPrefab;
        [SerializeField] private AchievementCard achievementCardCompletePrefab;
#pragma warning restore CS0649
        
        private void Start()
        {
            UpdatePanel();
        }

        private void OnEnable()
        {
            Achievement.OnAnyAchievementCompleted += Achievement_OnCompleted;
        }

        private void OnDisable()
        {
            Achievement.OnAnyAchievementCompleted -= Achievement_OnCompleted;
        }

        private void Achievement_OnCompleted(object sender, EventArgs e)
        {
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            foreach (var achievement in GetAchievements())
            {
                var prefab = achievement.Status == AchievementStatus.Completed
                    ? achievementCardCompletePrefab
                    : achievementCardPrefab;
                prefab.Achievement = achievement;
                Instantiate(prefab, transform);
            }
        }

        private static IEnumerable<Achievement> GetAchievements()
        {
            var list = new List<Achievement>();
            var asset = AssetTag.Achievements.ToAsset();
            var order = Game.GameData.achievementsOrder;

            for (var i = 0; i < order.Count; i++)
            {
                if (Enum.TryParse(order[i], out AchievementTag tag) &&
                    asset.GetItem<Achievement>(tag).Status != AchievementStatus.Empty)
                    list.Add(asset.GetItem<Achievement>(tag));
                else
                {
                    order.RemoveAt(i);
                    i--;
                }
            }

            var saveGame = false;

            foreach (var achievement in asset.GetItems<Achievement>())
            {
                if (!achievement || achievement.Status == AchievementStatus.Empty ||
                    list.Contains(achievement)) continue;

                list.Add(achievement);
                order.Add(achievement.TaggedName.ToString());
                saveGame = true;
            }

            if (saveGame) DataManager.SaveData();
            list.Reverse();
            return list;
        }
    }
}