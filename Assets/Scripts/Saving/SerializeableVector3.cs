using System.Numerics;
using Unity.VisualScripting;
using Vector3 = UnityEngine.Vector3;

namespace Saving
{
    [System.Serializable]
    public class SerializeableVector3
    {
        private float x, y, z;

        public SerializeableVector3(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public void SerializeVector(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public Vector3 GetVector()
        {
            return new Vector3(x, y, z);
        }
    }
}