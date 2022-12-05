using SceneManagement;
using System;
using System.Collections;
using UnityEngine;

namespace Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        private SavingSystem _savingSystem;
        private const string defaultSaveFile = "save";


        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            _savingSystem = GetComponent<SavingSystem>();
            yield return _savingSystem.LoadLastScene(defaultSaveFile);
            yield return FindObjectOfType<Fader>().FadeOut(0);
            yield return FindObjectOfType<Fader>().FadeIn(2);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            _savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(defaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(defaultSaveFile);
        }
    }
}