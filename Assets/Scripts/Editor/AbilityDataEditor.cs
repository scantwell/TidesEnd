using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using TidesEnd.Abilities;

namespace TidesEnd.Editor
{
    /// <summary>
    /// Custom inspector for AbilityData that provides a dropdown UI for adding concrete AbilityEffect types.
    /// This solves the issue where [SerializeReference] arrays show empty elements without type selection.
    /// </summary>
    [CustomEditor(typeof(AbilityData))]
    public class AbilityDataEditor : UnityEditor.Editor
    {
        private SerializedProperty effectsProperty;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty("effects");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw all default fields except effects array
            DrawPropertiesExcluding(serializedObject, "effects");

            // Draw custom effects array with type selection
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

            DrawEffectsArray();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEffectsArray()
        {
            // Draw array size field
            int newSize = EditorGUILayout.IntField("Size", effectsProperty.arraySize);
            if (newSize != effectsProperty.arraySize)
            {
                effectsProperty.arraySize = newSize;
            }

            EditorGUI.indentLevel++;

            // Draw each element with type dropdown
            for (int i = 0; i < effectsProperty.arraySize; i++)
            {
                SerializedProperty element = effectsProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();

                // Show element label
                string elementLabel = $"Element {i}";
                if (element.managedReferenceValue != null)
                {
                    elementLabel = $"Element {i} ({element.managedReferenceValue.GetType().Name})";
                }

                EditorGUILayout.LabelField(elementLabel, EditorStyles.boldLabel);

                // Type selection dropdown
                if (GUILayout.Button("Change Type", GUILayout.Width(100)))
                {
                    ShowEffectTypeMenu(element);
                }

                // Remove button
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    effectsProperty.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Draw properties if element has a value
                if (element.managedReferenceValue != null)
                {
                    EditorGUI.indentLevel++;
                    DrawEffectProperties(element);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox("Click 'Change Type' to select an effect type", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel--;

            // Add new effect button
            if (GUILayout.Button("+ Add Effect"))
            {
                effectsProperty.arraySize++;
                SerializedProperty newElement = effectsProperty.GetArrayElementAtIndex(effectsProperty.arraySize - 1);
                ShowEffectTypeMenu(newElement);
            }
        }

        private void ShowEffectTypeMenu(SerializedProperty property)
        {
            // Get all concrete AbilityEffect types
            var effectTypes = GetAllEffectTypes();

            GenericMenu menu = new GenericMenu();

            foreach (var type in effectTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private Type[] GetAllEffectTypes()
        {
            // Find all non-abstract classes that inherit from AbilityEffect
            return Assembly.GetAssembly(typeof(AbilityEffect))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AbilityEffect)))
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private void DrawEffectProperties(SerializedProperty property)
        {
            // Draw all child properties
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            iterator.NextVisible(true); // Enter the first child

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                EditorGUILayout.PropertyField(iterator, true);
                if (!iterator.NextVisible(false))
                    break;
            }
        }
    }
}