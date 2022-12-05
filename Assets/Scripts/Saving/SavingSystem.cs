using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Cinemachine.Utility;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Saving
{
    public class SavingSystem : MonoBehaviour
    {

        private void Start()
        {
            print(Application.persistentDataPath);
        }


        public IEnumerator LoadLastScene(string saveFile)
        {
            var state = LoadFile(saveFile);

            var sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                sceneIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            var state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Delete(string saveFile)
        {
            var path = GetPathFromSaveFile(saveFile);
            File.Delete(path);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);

            print("Saving to " + path);

            using (var stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state); 
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            print("Loading from " + path);

            using (var stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                var id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }


        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}