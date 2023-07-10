using UnityEngine;

namespace Enemy
{
    public class PortalRotate : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        
        void Update() 
        {
            transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
        }
    }
}
