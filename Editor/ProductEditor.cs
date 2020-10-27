using UnityEditor;
using UnityEngine;

namespace FlappyBlooper
{
    [CustomEditor(typeof(Product), true)]
    [CanEditMultipleObjects]
    public class ProductEditor : Editor
    {
        private SerializedProperty _itemTarget;
        private SerializedProperty _offersAvailability;
        protected SerializedProperty LimitedOffer;
        protected SerializedProperty DiscountProperty;

        protected virtual void OnEnable()
        {
            _itemTarget = serializedObject.FindProperty("itemTarget");
            _offersAvailability = serializedObject.FindProperty("stockLimitedOffer");
            LimitedOffer = serializedObject.FindProperty("limitedOffer");
            DiscountProperty = serializedObject.FindProperty("discount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_itemTarget);
            EditorGUILayout.PropertyField(LimitedOffer);

            if (LimitedOffer.boolValue)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_offersAvailability);
                
                if (GUILayout.Button("+"))
                {
                    _offersAvailability.intValue++;
                }
                
                if (GUILayout.Button("-"))
                {
                    _offersAvailability.intValue--;
                }
                
                if (_offersAvailability.intValue < 1)
                {
                    _offersAvailability.intValue = 1;
                }
                
                GUILayout.EndHorizontal();
            }
            else
            {
                if (_offersAvailability.intValue > 0)
                {
                    _offersAvailability.intValue = 0;
                }
            }

            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}