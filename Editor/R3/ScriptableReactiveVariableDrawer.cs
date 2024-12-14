#if R3_SUPPORT
using JHS.Library.ScriptableVariable.Runtime.R3;
using UnityEditor;
using UnityEngine;

namespace JHS.Library.ScriptableVariable.Editor.R3
{
    [CustomPropertyDrawer(typeof(ScriptableReactiveVariable<>), true)]
    public class ScriptableReactiveVariableDrawer : PropertyDrawer
    {
        SerializedObject cachedSerializedObject;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

            var so = new SerializedObject(property.objectReferenceValue);
            var valueProperty = so.FindProperty("value");
            float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, true);

            return EditorGUIUtility.singleLineHeight + valueHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(objectFieldRect, property, label, false);

            if (property.objectReferenceValue != null)
            {
                if (cachedSerializedObject?.targetObject != property.objectReferenceValue)
                {
                    cachedSerializedObject = new SerializedObject(property.objectReferenceValue);
                }

                cachedSerializedObject.Update();
                var valueProperty = cachedSerializedObject.FindProperty("value");

                var valueRect = new Rect(
                    position.x,
                    objectFieldRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUI.GetPropertyHeight(valueProperty, true)
                );

                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(valueRect, valueProperty, true);
                EditorGUI.indentLevel--;

                if (EditorGUI.EndChangeCheck())
                {
                    cachedSerializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                var buttonRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

                if (GUI.Button(buttonRect, "Create New Instance"))
                {
                    CreateNewInstance(property);
                }
            }
        }

        void CreateNewInstance(SerializedProperty property)
        {
            EditorApplication.delayCall += () =>
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create Scriptable Variable",
                    ObjectNames.NicifyVariableName(property.name),
                    "asset",
                    "Choose a location to save the new instance"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    var instance = ScriptableObject.CreateInstance(fieldInfo.FieldType);
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();

                    EditorApplication.delayCall += () =>
                    {
                        property.objectReferenceValue = instance;
                        property.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    };
                }
            };
        }
    }
}
#endif