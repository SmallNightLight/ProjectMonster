using ScriptableArchitecture.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    public class GameEventEditorHelper
    {
        public static void DrawListeners(IGameEvent gameEvent, ref bool showListeners)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            bool inPlaymode = EditorApplication.isPlaying;

            List<IListener> listeners = new List<IListener>();

            if (inPlaymode)
            {
                foreach (IListener listener in gameEvent.GetListeners())
                    listeners.Add(listener);
            }
            else
            {
                foreach (GameObject gameObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    foreach (IListener listener in gameObject.GetComponentsInChildren<IListener>())
                        if (listener.GetGameEvent() == gameEvent)
                            listeners.Add(listener);
            }

            string labelText = (inPlaymode ? "Active listeners" : "Listeners in active scene") + $" ({listeners.Count})";
            showListeners = EditorGUILayout.Foldout(showListeners, labelText, true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (showListeners)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Space();

                foreach (IListener listener in listeners)
                    EditorGUILayout.ObjectField(listener.GetListenerObject(), typeof(GameEventListenerBase), true);

                EditorGUILayout.Space();
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndVertical();
        }

        public static void DrawStackTrace(Stacktrace stacktrace, ref bool showStacktrace, ref Vector2 scrollPosition)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            List<string> messages = stacktrace.GetMessages();

            EditorGUILayout.BeginHorizontal();
            showStacktrace = EditorGUILayout.Foldout(showStacktrace, $"Stacktrace {stacktrace.GetStackType()} ({messages.Count})", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (showStacktrace)
            {
                GUILayout.FlexibleSpace();
                if (messages.Count > 0 && GUILayout.Button("Clear", GUILayout.Width(100)))
                    stacktrace.Clear();

                EditorGUILayout.EndHorizontal();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(Mathf.Min(300, messages.Count * (EditorGUIUtility.singleLineHeight + 2) + 2)));

                foreach (string message in messages)
                    EditorGUILayout.LabelField(message);

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}