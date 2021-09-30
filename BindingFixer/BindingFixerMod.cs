using MelonLoader;
using SG.XR.InputHandling;
using System;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(BindingFixer.BindingFixerMod), "Binding Fixer", "1.0.0", "nicoco007")]
[assembly: MelonGame("Schell Games", "UntilYouFall")]
namespace BindingFixer
{
    public class BindingFixerMod : MelonMod
    {
        private static readonly string[] kShouldBeAnyInputSourceButtons =
        {
            "/actions/default/in/RotateLeft",
            "/actions/default/in/RotateRight",
            "/actions/default/in/DashForward",
            "/actions/default/in/DashBack",
        };

        private static readonly string[] kShouldBeAnyInputSourceAxes =
        {
            "/actions/default/in/Movement",
        };

        private readonly UnityAction<Scene, LoadSceneMode> _sceneLoadedHandler;

        private bool _processed;

        public BindingFixerMod()
        {
            _sceneLoadedHandler = new Action<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        public override void OnApplicationStart()
        {
            SceneManager.add_sceneLoaded(_sceneLoadedHandler);
            base.OnApplicationStart();
        }

        public override void OnApplicationQuit()
        {
            SceneManager.remove_sceneLoaded(_sceneLoadedHandler);
            base.OnApplicationQuit();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "TitleScreen" || _processed)
            {
                return;
            }

            foreach (OpenVRButton openVrButton in Resources.FindObjectsOfTypeAll(Il2CppType.Of<OpenVRButton>()).Select(b => b.Cast<OpenVRButton>()))
            {
                string fullPath = openVrButton.button.axis.fullPath;

                if (kShouldBeAnyInputSourceButtons.Contains(fullPath))
                {
                    MelonLogger.Msg($"Fixing '{fullPath}'");
                    openVrButton.button.node = Valve.VR.SteamVR_Input_Sources.Any;
                }
            }

            foreach (OpenVRAxis openVrAxis in Resources.FindObjectsOfTypeAll(Il2CppType.Of<OpenVRAxis>()).Select(b => b.Cast<OpenVRAxis>()))
            {
                string fullPath = openVrAxis.axis.fullPath;

                if (kShouldBeAnyInputSourceAxes.Contains(fullPath))
                {
                    MelonLogger.Msg($"Fixing '{fullPath}'");
                    openVrAxis.node = Valve.VR.SteamVR_Input_Sources.Any;
                }
            }

            _processed = true;
        }
    }
}
