using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace chsxf
{
    [Serializable]
    public class AutoSceneSettings
    {
        public const string VERSION = "1.2.2";

        [SerializeField] private string version = VERSION;
        public string Version { get { return version; } }

        [SerializeField] private bool enabled = true;
        public bool Enabled {
            get { return enabled; }
            set {
                if (enabled != value) {
                    enabled = value;
                    SaveSettings();
                }
            }
        }

        [SerializeField] private string loadedScene = "auto";
        public string LoadedScene {
            get { return loadedScene; }
            set {
                if (loadedScene != value) {
                    loadedScene = value;
                    SaveSettings();
                }
            }
        }

        private static string ProjectPath {
            get {
                string assetsPath = Application.dataPath;
                return Directory.GetParent(assetsPath).FullName;
            }
        }

        private static string PrefsKey {
            get { return ProjectPath + ".AutoScene"; }
        }

        private static string LegacyPrefsKeyEnabled {
            get { return PrefsKey + ".enabled"; }
        }

        private void SaveSettings() {
            string prefs = JsonUtility.ToJson(this);
            EditorPrefs.SetString(PrefsKey, prefs);
        }

        public static AutoSceneSettings LoadSettings() {
            string prefs = EditorPrefs.GetString(PrefsKey, null);
            bool hasKey = !string.IsNullOrEmpty(prefs);
            bool isLegacy = (hasKey && !prefs.StartsWith("{"));
            if (!hasKey || isLegacy) {
                AutoSceneSettings settings = new AutoSceneSettings();
                if (isLegacy) {
                    settings.enabled = EditorPrefs.GetBool(LegacyPrefsKeyEnabled, true);
                    settings.loadedScene = prefs;
                }
                settings.SaveSettings();
                if (isLegacy) {
                    EditorPrefs.DeleteKey(LegacyPrefsKeyEnabled);
                }
                return settings;
            }
            return JsonUtility.FromJson<AutoSceneSettings>(prefs);
        }
    }
}