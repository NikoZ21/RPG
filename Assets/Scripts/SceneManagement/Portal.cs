using System;
using System.Collections;
using System.Collections.Generic;
using Control;
using Saving;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int indexOfSceneToLoad = 1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float timeToFadeOut = 1.5f;
        [SerializeField] private float timeToFadeIn = 1.5f;
        private Fader _fader;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            _fader = FindObjectOfType<Fader>();
            DontDestroyOnLoad(gameObject);
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;


            _fader.FadeOut(timeToFadeOut);

            var savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            yield return new WaitForSeconds(2f);

            yield return SceneManager.LoadSceneAsync(indexOfSceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            savingWrapper.Load();


            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            savingWrapper.Save();

            _fader.FadeIn(timeToFadeIn);
            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.transform.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();

            foreach (Portal portal in portals)
            {
                if (portal == this) continue;

                return portal;
            }

            return null;
        }
    }
}