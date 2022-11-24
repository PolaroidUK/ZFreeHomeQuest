using System.Collections.Generic;
using System.Reflection;
using Runtime;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    using Object = UnityEngine.Object;
    [CustomEditor(typeof(Object),true)]
    [CanEditMultipleObjects]
    public class ObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawMethodButtons(targets);
        }
        private void DrawMethodButtons(Object[] objectTargets)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public ;
            var methods = target.GetType().GetMethods(flags);
            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null)
                    continue;
                Dictionary<System.Object,MethodInfo> methodInfos = new Dictionary<System.Object,MethodInfo>();
                foreach (var t in objectTargets)
                {
                    methodInfos.Add(t,t.GetType().GetMethod(method.Name));
                }
                string eventName = buttonAttribute.Name ?? method.Name;
                bool shouldDisable =(!Application.isPlaying && buttonAttribute.ButtonMode == ButtonMode.EnabledInPlayMode) || (Application.isPlaying && buttonAttribute.ButtonMode == ButtonMode.DisabledInPlayMode);
                EditorGUI.BeginDisabledGroup(shouldDisable);
                DrawButton(eventName,methodInfos);
                EditorGUI.EndDisabledGroup();
            }
        }
        private void DrawButton(string buttonName, Dictionary<System.Object,MethodInfo> raiseMethods)
        {
            if (!GUILayout.Button(buttonName)) return;
            foreach (var raiseMethod in raiseMethods)
            {
                raiseMethod.Value?.Invoke(raiseMethod.Key,null);
            }
        }
    }
}

