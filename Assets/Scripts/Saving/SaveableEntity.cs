using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Core;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = "";
        private static Dictionary<string, SaveableEntity> globalLookUp = new Dictionary<string, SaveableEntity>();


#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookUp[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string canditate)
        {
            if (!globalLookUp.ContainsKey(canditate)) return true;

            if (globalLookUp[canditate] == this) return true;

            if (globalLookUp[canditate] == null)
            {
                globalLookUp.Remove(canditate);
                return true;
            }

            if (globalLookUp[canditate].GetUniqueIdentifier() != canditate)
            {
                globalLookUp.Remove(canditate);
                return true;
            }

            return false;
        }

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> restoredState = (Dictionary<string, object>)state;
            foreach (var saveable in GetComponents<ISaveable>())
            {
                string id = saveable.GetType().ToString();
                if (restoredState.ContainsKey(id))
                {
                    saveable.RestoreState(restoredState[id]);
                }
            }
        }
    }
}