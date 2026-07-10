using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ScratchPaper{ }

namespace DebugTools
{//REPLACE INTERNALS WITH GAME-SPECIFIC LOGIC
    public interface IDebug_Name
    {
        public string HumanReadableName();
    }

    public class DebugToolsInspector : MonoBehaviour
    {//put this on a debugger object and don't-destroy-on-load, and make sure it spawns in somewhere, usually in core scenes
        public static DebugToolsInspector Instance;

//        [SerializeField] private bool ENABLE_DEBUG_MESSAGES = true;
        private void Start()
        {
            if (Instance == null) Destroy(gameObject);

            Instance = this;
        }
    }

    public static class DebugExtensions
    {//I'll get back to this eventually ig
        public static void LogError(this MonoBehaviour mono, string message)
        {
        }

        [Conditional("UNITY_EDITOR")]
        private static void LogError_Editor(MonoBehaviour mono, string message)
        {

        }
        private static void LogError_DevBuild(MonoBehaviour mono, string message)
        {

        }
    }
}