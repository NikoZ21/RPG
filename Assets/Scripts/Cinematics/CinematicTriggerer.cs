using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics
{
    public class CinematicTriggerer : MonoBehaviour
    {
        private bool isPlayed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isPlayed)
            {
                isPlayed = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}