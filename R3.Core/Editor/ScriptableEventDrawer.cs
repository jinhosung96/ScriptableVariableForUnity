#if R3_SUPPORT
using UnityEditor;
using UnityEngine;

namespace JHS.ScriptableVariable.R3
{
    [CustomPropertyDrawer(typeof(ScriptableEvent<>), true)]
    public class ScriptableEventDrawer : PropertyDrawer
    {
        SerializedObject cachedSerializedObject;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(objectFieldRect, property, label, false);

            if (property.objectReferenceValue == null)
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