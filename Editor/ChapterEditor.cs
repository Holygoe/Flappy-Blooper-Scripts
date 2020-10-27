using UnityEditor;
using UnityEngine;

namespace FlappyBlooper.Editors
{
    [CustomEditor(typeof(Chapter))]
    public class ChapterEditor : Editor
    {
        private MonoScript _script;
        private SerializedProperty _tag;
        private SerializedProperty _nameKey;
        private SerializedProperty _stages;

        private void OnEnable()
        {
            _script = MonoScript.FromScriptableObject((Chapter)target);
            _tag = serializedObject.FindProperty("tag");
            _nameKey = serializedObject.FindProperty("nameKey");
            _stages = serializedObject.FindProperty("stages");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(_tag);
            EditorGUILayout.PropertyField(_nameKey);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Stages count", _stages.arraySize);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(5);
            for (var i = 0; i < _stages.arraySize; i++)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal("box");
                GUILayout.Label($"Stage {i + 1}", EditorStyles.boldLabel);
                
                if (GUILayout.Button("↑") && i > 0)
                {
                    _stages.MoveArrayElement(i, i - 1);
                    break;
                }
                
                if (GUILayout.Button("↓") && i < _stages.arraySize - 1)
                {
                    _stages.MoveArrayElement(i, i + 1);
                    break;
                }

                if (GUILayout.Button("Add"))
                {
                    _stages.InsertArrayElementAtIndex(i);
                    break;
                }
                
                if (GUILayout.Button("Remove"))
                {
                    _stages.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                GUILayout.EndHorizontal();
                
                var stageProp = _stages.GetArrayElementAtIndex(i);
                GUILayout.Space(10);
                var lengthProp = stageProp.FindPropertyRelative("length");
                EditorGUILayout.PropertyField(lengthProp);
                if (lengthProp.intValue < 10) lengthProp.intValue = 10;
                var difficultyProp = stageProp.FindPropertyRelative("difficulty");
                EditorGUILayout.IntSlider(difficultyProp, 0, Stage.MaxDifficulty);
                var rewardsProp = stageProp.FindPropertyRelative("rewards");
                rewardsProp.arraySize = 3;
                GUILayout.Label($"Rewards");
                
                for (var j = 0; j < 3; j++)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label($"Start {j + 1}");
                    var rewardProp = rewardsProp.GetArrayElementAtIndex(j);
                    EditorGUILayout.PropertyField(rewardProp);
                    GUILayout.EndVertical();
                }
                
                GUILayout.Space(5);
            }
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add stage")) _stages.arraySize++;
            if (GUILayout.Button("Remove last")) _stages.arraySize--;
            GUILayout.EndHorizontal();
            
            GUILayout.Space(40);
            
            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}