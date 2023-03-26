using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace chsxf
{
    [InitializeOnLoad]
    public static class AutoSceneTestFrameworkConnector
    {
        private static readonly object[] emptyObjectArray = new object[0];

        static AutoSceneTestFrameworkConnector() {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _change) {
            if (_change == PlayModeStateChange.ExitingEditMode) {
                AutoScene.IsRunningTests = AreTestsRunning();
            }
            else if (_change == PlayModeStateChange.EnteredEditMode) {
                AutoScene.IsRunningTests = false;
            }
        }

        private static bool AreTestsRunning() {
            Type type = typeof(TestRunnerApi);
            MethodInfo mi = type.GetMethod("IsRunActive", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Log(mi);
            bool result = (bool) mi.Invoke(null, emptyObjectArray);
            Debug.Log($"Tests running: {result}");
            return result;
        }
    }
}
