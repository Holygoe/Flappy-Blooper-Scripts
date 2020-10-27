using UnityEditor;
using UnityEditor.UI;

namespace FlappyBlooper
{
    [CustomEditor(typeof(RudeButton), true)]
    [CanEditMultipleObjects]
    public class RudeButtonEditor : ButtonEditor
    {
        private SerializedProperty _disabledSpriteProperty;
        private SerializedProperty _graphicsProperty;
        private SerializedProperty _disabledColorProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _disabledSpriteProperty = serializedObject.FindProperty("disabledSprite");
            _disabledColorProperty = serializedObject.FindProperty("disabledColor");
            _graphicsProperty = serializedObject.FindProperty("graphics");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_disabledSpriteProperty);
            EditorGUILayout.PropertyField(_disabledColorProperty);
            EditorGUILayout.PropertyField(_graphicsProperty, true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}