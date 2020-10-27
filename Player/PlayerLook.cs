using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace FlappyBlooper
{
    public class PlayerLook : MonoBehaviour
    {
        public const string NoneLabel = "None";
        public const string DeadFaceLabel = "Dead";

        private List<SpriteResolver> _bodyParts;
        private List<SpriteResolver> _accessories;
        private SpriteLibraryAsset _asset;
        private SpriteResolver _face;

        private void Awake()
        {
            _asset = Assets.SpriteLibraryAsset;
            GetComponent<SpriteLibrary>().spriteLibraryAsset = _asset;
            _face = transform.Find(BodyPartTag.Face.ToString()).GetComponent<SpriteResolver>();
            _bodyParts = new List<SpriteResolver>();
            
            foreach (BodyPartTag bodyPartTag in typeof(BodyPartTag).GetEnumValues())
            {
                _bodyParts.Add(transform.Find(bodyPartTag.ToString()).GetComponent<SpriteResolver>());
            }

            _accessories = new List<SpriteResolver>();
            
            foreach (AccessoryCategory category in typeof(AccessoryCategory).GetEnumValues())
            {
                _accessories.Add(transform.Find(category.ToString()).GetComponent<SpriteResolver>());
            }
        }

        private void OnEnable()
        {
            UpdateLook();
        }

        public void UpdateLook()
        {
            UpdateBodyParts();
            UpdateAccessories();
        }

        private void UpdateBodyParts()
        {
            var playerLabel = Game.GameData.selectedCharacter;

            foreach (var resolver in _bodyParts)
            {
                SetLabel(resolver, playerLabel);
            }
        }

        private void UpdateAccessories()
        {
            foreach (var spriteResolver in _accessories)
            {
                var categoryName = spriteResolver.GetCategory();

                if (Enum.TryParse(categoryName, out AccessoryCategory category))
                {
                    if (Accessory.TryToGetSelectedItem(category, out var accessory))
                    {
                        SetLabel(spriteResolver, accessory.TaggedName.ToString());
                        continue;
                    }
                }

                spriteResolver.SetCategoryAndLabel(categoryName, NoneLabel);
            }
        }

        public void UpdateFace(bool dead)
        {
            if (dead)
            {
                SetLabel(_face, DeadFaceLabel);
            }
            else
            {
                var playerLabel = Game.GameData.selectedCharacter;
                SetLabel(_face, playerLabel);
            }
        }

        private void SetLabel(SpriteResolver spriteResolver, string label)
        {
            var category = spriteResolver.GetCategory();
            label = DoesLabelExistInCategory(label, category) ? label : NoneLabel;
            spriteResolver.SetCategoryAndLabel(category, label);
        }

        private bool DoesLabelExistInCategory(string label, string category)
        {
            var labelNames = new List<string>(_asset.GetCategoryLabelNames(category));
            return labelNames.Any(labelName => label == labelName);
        }
    }
}