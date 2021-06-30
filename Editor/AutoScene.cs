using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;

namespace chsxf
{
    [InitializeOnLoad]
    public static class AutoScene
    {
        private const string VERSION = "1.1.0";

        private static string ProjectPath {
            get {
                string assetsPath = Application.dataPath;
                return Directory.GetParent(assetsPath).FullName;
            }
        }

        private static string PrefsKey {
            get { return ProjectPath + ".AutoScene"; }
        }

        private static string PrefsKeyEnabled {
            get { return PrefsKey + ".enabled"; }
        }

        private static bool Enabled {
            get { return EditorPrefs.GetBool(PrefsKeyEnabled, true); }
        }

        static AutoScene() {
            EditorBuildSettings.sceneListChanged += UpdatePlayModeStartScene;
            UpdatePlayModeStartScene();
        }

        private static void UpdatePlayModeStartScene() {
            SceneAsset sceneAsset = null;

            if (Enabled) {
                string value = EditorPrefs.GetString(PrefsKey, "none");
                if (value == "auto") {
                    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                        if (scene.enabled) {
                            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                            break;
                        }
                    }
                }
                else if (value != "none") {
                    sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(value);
                }
            }

            EditorSceneManager.playModeStartScene = sceneAsset;
        }

        [MenuItem("Tools/AutoScene/Disable")]
        private static void DisableAutoScene() {
            EditorPrefs.SetBool(PrefsKeyEnabled, false);
            UpdatePlayModeStartScene();
        }

        [MenuItem("Tools/AutoScene/Disable", true)]
        private static bool CanDisableAutoScene() {
            return Enabled;
        }

        [MenuItem("Tools/AutoScene/Enable")]
        private static void EnableAutoScene() {
            EditorPrefs.SetBool(PrefsKeyEnabled, true);
            UpdatePlayModeStartScene();
        }

        [MenuItem("Tools/AutoScene/Enable", true)]
        private static bool CanEnableAutoScene() {
            return !Enabled;
        }

        [PreferenceItem("AutoScene")]
        public static void AutoScenePreferences() {
            string prefsValue = EditorPrefs.GetString(PrefsKey, "none");

            EditorGUILayout.LabelField("Version", VERSION, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Build scene list
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            string[] scenePathes = new string[sceneGuids.Length];
            for (int i = 0; i < sceneGuids.Length; i++) {
                scenePathes[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            }
            Array.Sort(scenePathes, string.Compare);

            // Finding selected index
            int selectedIndex = 0;
            if (prefsValue == "auto") {
                selectedIndex = 1;
            }
            else {
                int arrayIndex = Array.IndexOf(scenePathes, prefsValue);
                if (arrayIndex >= 0) {
                    selectedIndex = arrayIndex + 2;
                }
            }

            string[] menuEntries = new string[scenePathes.Length + 2];
            menuEntries[0] = "None";
            menuEntries[1] = "Auto";
            Array.Copy(scenePathes, 0, menuEntries, 2, scenePathes.Length);

            EditorGUI.BeginChangeCheck();
            
            bool enabled = Enabled;
            enabled = EditorGUILayout.Toggle("Enable AutoScene", enabled);
            EditorGUILayout.Space();

            selectedIndex = EditorGUILayout.Popup("Scene to load on Play", selectedIndex, menuEntries);

            if (EditorGUI.EndChangeCheck()) {
                if (selectedIndex == 0) {
                    prefsValue = "none";
                }
                else if (selectedIndex == 1) {
                    prefsValue = "auto";
                }
                else {
                    prefsValue = menuEntries[selectedIndex];
                }

                EditorPrefs.SetString(PrefsKey, prefsValue);
                EditorPrefs.SetBool(PrefsKeyEnabled, enabled);
                UpdatePlayModeStartScene();
            }

            string helpBoxMessage;
            if (selectedIndex == 0) {
                helpBoxMessage = "The scenes currently loaded in the editor will be maintained when entering Play mode.\n\nThis is the default Unity behaviour.";
            }
            else if (selectedIndex == 1) {
                helpBoxMessage = "The first enabled scene in the Build Settings box will be loaded when entering Play mode. If no such scene exists, falls back to 'None'.";
            }
            else {
                helpBoxMessage = string.Format("The scene '{0}' will be loaded when entring Play mode. If the scene does not exist anymore, falls back to 'None'.", prefsValue);
            }
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(helpBoxMessage, MessageType.Info, true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Made with ❤️ by chsxf");
        }
    }
}