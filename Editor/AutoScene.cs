using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace chsxf
{
    [InitializeOnLoad]
    public static class AutoScene
    {
        private const string MENUITEM_PREFIX = "Tools/AutoScene (" + AutoSceneSettings.VERSION + ")/";
        private const string SETTINGS_PROVIDER_PATH = "AutoScene";

        private static AutoSceneSettings settings = null;

        static AutoScene() {
            settings = AutoSceneSettings.LoadSettings();

            EditorBuildSettings.sceneListChanged += UpdatePlayModeStartScene;
            UpdatePlayModeStartScene();
        }

        private static void UpdatePlayModeStartScene() {
            SceneAsset sceneAsset = null;

            if (settings.Enabled) {
                if (settings.LoadedScene == "auto") {
                    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                        if (scene.enabled) {
                            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                            break;
                        }
                    }
                }
                else if (settings.LoadedScene != "none") {
                    sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(settings.LoadedScene);
                }
            }

            EditorSceneManager.playModeStartScene = sceneAsset;
        }

        [MenuItem(MENUITEM_PREFIX + "Open Settings...")]
        private static void OpenSettings() {
            SettingsService.OpenUserPreferences(SETTINGS_PROVIDER_PATH);
        }

        [MenuItem(MENUITEM_PREFIX + "Disable")]
        private static void DisableAutoScene() {
            settings.Enabled = false;
            UpdatePlayModeStartScene();
        }

        [MenuItem(MENUITEM_PREFIX + "Disable", true)]
        private static bool CanDisableAutoScene() {
            return settings.Enabled;
        }

        [MenuItem(MENUITEM_PREFIX + "Enable")]
        private static void EnableAutoScene() {
            settings.Enabled = true;
            UpdatePlayModeStartScene();
        }

        [MenuItem(MENUITEM_PREFIX + "Enable", true)]
        private static bool CanEnableAutoScene() {
            return !settings.Enabled;
        }

        [SettingsProvider]
        public static SettingsProvider AutoSceneSettingsProvider() {
            SettingsProvider provider = new SettingsProvider(SETTINGS_PROVIDER_PATH, SettingsScope.User) {
                keywords = new HashSet<string>(new[] { "scene", "autoscene", "play mode" }),

                guiHandler = (searchContext) => {
                    EditorGUILayout.LabelField("Version", AutoSceneSettings.VERSION, EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    // Build scene list
                    string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
                    string[] scenePathes = new string[sceneGuids.Length];
                    for (int i = 0; i < sceneGuids.Length; i++) {
                        scenePathes[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                    }
                    Array.Sort(scenePathes, string.Compare);

                    // Finding selected index
                    string prefsValue = settings.LoadedScene;
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

                    bool enabled = settings.Enabled;
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

                        settings.LoadedScene = prefsValue;
                        settings.Enabled = enabled;
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
                    EditorGUILayout.HelpBox(helpBoxMessage, MessageType.Info, wide: false);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Made with ❤️ by chsxf");
                }
            };
            return provider;
        }
    }
}