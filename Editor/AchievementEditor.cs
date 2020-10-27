using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlappyBlooper.Editors
{
    [CustomEditor(typeof(Achievement))]
    public class AchievementEditor : Editor
    {
        private SerializedProperty _tagProp;
        private SerializedProperty _descriptionKeyProp;
        private SerializedProperty _iconProp;
        private SerializedProperty _nameKeyProp;
        private SerializedProperty _parentProp;
        private SerializedProperty _rewardItemProp;
        private SerializedProperty _thresholdsProp;

        private void OnEnable()
        {
            _tagProp = serializedObject.FindProperty("tag");
            _descriptionKeyProp = serializedObject.FindProperty("descriptionKey");
            _iconProp = serializedObject.FindProperty("icon");
            _nameKeyProp = serializedObject.FindProperty("nameKey");
            _parentProp = serializedObject.FindProperty("parentAchievement");
            _rewardItemProp = serializedObject.FindProperty("rewardItem");
            _thresholdsProp = serializedObject.FindProperty("thresholds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_tagProp);
            EditorGUILayout.PropertyField(_nameKeyProp);
            EditorGUILayout.PropertyField(_descriptionKeyProp);
            EditorGUILayout.ObjectField(_iconProp, typeof(Sprite));
            EditorGUILayout.ObjectField(_parentProp, typeof(Achievement));
            
            if (_parentProp.objectReferenceValue is Achievement parent)
            {
                EditorGUI.BeginDisabledGroup(true);
                var serializedParent = new SerializedObject(parent);
                ThresholdsLayout(
                    serializedParent.FindProperty("rewardItem"),
                    serializedParent.FindProperty("thresholds"));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                ThresholdsLayout(_rewardItemProp, _thresholdsProp);
            }
            
            serializedObject.ApplyModifiedProperties();
            if(GUI.changed) EditorUtility.SetDirty(target);
        }

        private static void ThresholdsLayout(SerializedProperty rewardItemProp, SerializedProperty thresholdsProp)
        {
            EditorGUILayout.ObjectField(rewardItemProp, typeof(CountableItem));
            
            GUILayout.Space(10);
            GUILayout.Label($"Thresholds ({thresholdsProp.arraySize})");
            var lastThresholdValue = 0;
            
            for (var i = 0; i < thresholdsProp.arraySize; i++)
            {
                GUILayout.Space(5);
                var thresholdProp = thresholdsProp.GetArrayElementAtIndex(i);
                GUILayout.Label($"Threshold {i + 1}");
                var valueProp = thresholdProp.FindPropertyRelative("value");
                var itemsAmountProp = thresholdProp.FindPropertyRelative("itemsAmount");

                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(valueProp);
                if (valueProp.intValue <= lastThresholdValue) valueProp.intValue = lastThresholdValue + 1;
                lastThresholdValue = valueProp.intValue;
                EditorGUILayout.PropertyField(itemsAmountProp, new GUIContent("Amount"));
                if (itemsAmountProp.intValue <= 0) itemsAmountProp.intValue = 1;
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = labelWidth;
            }
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add new")) thresholdsProp.arraySize++;
            if (GUILayout.Button("Remove last")) thresholdsProp.arraySize--;
            GUILayout.EndHorizontal();
        }
    }
}