using Control;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using UnityEngine;

namespace Combat
{
    public class WeaponPickUp : MonoBehaviour, IRayCastable
    {
        [SerializeField] private float heal = 0;
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float respawnTime = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }

            if (heal > 0)
            {
                subject.GetComponent<Health>().Heal(heal);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float respawnTime)
        {
            ShowPickUp(false);
            yield return new WaitForSeconds(respawnTime);
            ShowPickUp(true);
        }

        private void ShowPickUp(bool shouldShow)
        {
            GetComponent<SphereCollider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(1))
            {
                PickUp(callingController.gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }
    }
}