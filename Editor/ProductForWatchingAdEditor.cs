using UnityEditor;
using UnityEngine;

namespace FlappyBlooper
{
    [CustomEditor(typeof(ProductForWatchingAd), true)]
    [CanEditMultipleObjects]
    public class ProductForWatchingAdEditor : ProductEditor
    {
        private SerializedProperty _adTagProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _adTagProperty = serializedObject.FindProperty("adTag");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (LimitedOffer.boolValue)
            {
                _adTagProperty.enumValueIndex = (int) VideoAdTag.Offer;
            }
            else
            {
                _adTagProperty.enumValueIndex = (int) VideoAdTag.Store;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_adTagProperty);
            EditorGUI.EndDisabledGroup();
            
            DiscountProperty.intValue = 0;
            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}