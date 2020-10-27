using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace FlappyBlooper
{
    public static class SpriteLibraryAssetCreator
    {
        public static void CompleteCharacters(ref SpriteLibraryAsset asset, IEnumerable<Character> characters, Sprite empty, Sprite deadFace)
        {
            foreach (var category in Enum.GetValues(typeof(BodyPartTag)))
            {
                asset.AddCategoryLabel(empty, category.ToString(), PlayerLook.NoneLabel);
            }

            asset.AddCategoryLabel(deadFace, BodyPartTag.Face.ToString(), PlayerLook.DeadFaceLabel);

            foreach (var character in characters)
            {
                foreach(var part in character.BodyParts)
                {
                    asset.AddCategoryLabel(part.Sprite, part.Tag.ToString(), character.TaggedName.ToString());
                }
            }
        }

        public static void CompleteAccessories(ref SpriteLibraryAsset asset, IEnumerable<Accessory> accessories, Sprite empty)
        {
            foreach (var category in typeof(AccessoryCategory).GetEnumValues())
            {
                asset.AddCategoryLabel(empty, category.ToString(), PlayerLook.NoneLabel);
            }

            foreach (var accessory in accessories)
            {
                asset.AddCategoryLabel(accessory.Icon, accessory.Category.ToString(), accessory.TaggedName.ToString());
            }
        }
    }
}