using UnityEditor;
using UnityEngine;

namespace FlappyBlooper
{
    [CustomEditor(typeof(ProductForCurrency), true)]
    [CanEditMultipleObjects]
    public class ProductForCurrencyEditor : ProductEditor
    {
        private SerializedProperty _currencyTagProperty;
        private SerializedProperty _priceProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _currencyTagProperty = serializedObject.FindProperty("currencyTag");
            _priceProperty = serializedObject.FindProperty("price");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.PropertyField(_currencyTagProperty);
            EditorGUILayout.PropertyField(_priceProperty);
            EditorGUILayout.PropertyField(DiscountProperty);
            
            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}