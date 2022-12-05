using System;
using Control;
using Core;
using SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControls;
            GetComponent<PlayableDirector>().stopped += EnableControls;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControls;
            GetComponent<PlayableDirector>().stopped -= EnableControls;
        }


        void DisableControls(PlayableDirector pd)
        {
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControls(PlayableDirector pd)
        {
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}