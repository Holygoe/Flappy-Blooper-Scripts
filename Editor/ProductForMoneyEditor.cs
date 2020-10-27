using UnityEditor;
using UnityEngine;

namespace FlappyBlooper
{
    [CustomEditor(typeof(ProductForMoney), true)]
    [CanEditMultipleObjects]
    public class ProductForMoneyEditor : ProductEditor
    {
        private SerializedProperty _inAppProductTagProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _inAppProductTagProperty = serializedObject.FindProperty("inAppProductTag");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.PropertyField(_inAppProductTagProperty, new GUIContent("In App Product"));
            EditorGUILayout.PropertyField(DiscountProperty);
            
            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}