using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float defaultSpeed;
        [SerializeField] private float rotationSpeed;

        [HideInInspector]
        public float bonusSpeed;
        
        private Vector3 m_movement;
        private Quaternion m_moveRotation;
    
        // Gets called from Player Input Component
        public void OnMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            m_movement = new Vector3(move.x, 0f, move.y);

            if(move != Vector2.zero)
                m_moveRotation = Quaternion.LookRotation(m_movement);
        }

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_moveRotation, rotationSpeed);
            transform.Translate(m_movement * ((defaultSpeed + bonusSpeed) * Time.deltaTime), Space.World);
        }
    }
}
