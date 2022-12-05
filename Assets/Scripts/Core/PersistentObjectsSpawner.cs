using UnityEngine;

namespace Core
{
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentGameobject;
        private static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned == true) return;

            SpawnPersistentObjectsSpawner();
            hasSpawned = true;
        }

        private void SpawnPersistentObjectsSpawner()
        {
            GameObject persistentObjectsSpawner = Instantiate(persistentGameobject);
            DontDestroyOnLoad(persistentObjectsSpawner);
        }
    }
}