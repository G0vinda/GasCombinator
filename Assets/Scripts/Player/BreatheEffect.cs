using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class BreatheEffect : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        void Update()
        {
            transform.RotateAround(transform.position, transform.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
