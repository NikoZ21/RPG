using UnityEngine;

namespace Core
{
    public class CameraFacing : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}