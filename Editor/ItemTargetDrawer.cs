using UnityEditor;
using UnityEngine;

namespace FlappyBlooper.Editors
{
    [CustomPropertyDrawer(typeof(ItemTarget))]
    public class ItemTargetDrawer : PropertyDrawer
    {
        private const string Item = "item";
        private const string Count = "count";
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var itemProp = property.FindPropertyRelative(Item);
            var height = EditorGUI.GetPropertyHeight(itemProp, label) + 3;

            if (!(itemProp.objectReferenceValue is CountableItem)) return height;
            
            var countProp = property.FindPropertyRelative("count");
            height += EditorGUI.GetPropertyHeight(countProp, label) + 2;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
            
            var itemProp = property.FindPropertyRelative("item");
            var height = EditorGUI.GetPropertyHeight(itemProp, label);
            var y = position.y;
            var itemRect = new Rect(position.x, y, position.width, height);
            EditorGUI.ObjectField(itemRect, itemProp, typeof(Item), label);

            if (itemProp.objectReferenceValue is CountableItem)
            {
                var countProp = property.FindPropertyRelative("count");
                y += height + 3;
                height = EditorGUI.GetPropertyHeight(countProp, label);
                var countRect = new Rect(position.x, y, position.width, height);
                EditorGUI.PropertyField(countRect, countProp, label);
            }
            
            EditorGUI.EndProperty();
        }
    }
}