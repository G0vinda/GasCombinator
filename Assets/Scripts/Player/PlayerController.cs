using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float defaultSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Animator animator;

        private Rigidbody m_rigidbody;

        public float BonusSpeed
        {
            get => m_bonusSpeed;
            set
            {
                m_bonusSpeed = value;
                UpdateMovementSpeedValue(m_movement != Vector3.zero);
            }
        }

        private float m_bonusSpeed;
        private Vector3 m_movement;
        private Quaternion m_moveRotation;
        private int m_movementSpeedHash;

        private void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_movementSpeedHash = Animator.StringToHash("MovementSpeed");
        }

        // Gets called from Player Input Component
        public void OnMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            m_movement = new Vector3(move.x, 0f, move.y);

            if (move != Vector2.zero)
            {
                UpdateMovementSpeedValue(true);
                m_moveRotation = Quaternion.LookRotation(m_movement);
            }
            else
            {
                UpdateMovementSpeedValue(false);
            }
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void UpdateMovementSpeedValue(bool valueIsGreaterThanZero)
        {
            if (valueIsGreaterThanZero)
            {
                var newSpeedValue = (defaultSpeed + m_bonusSpeed) / defaultSpeed;
                animator.SetFloat(m_movementSpeedHash, newSpeedValue);
            }else{
                animator.SetFloat(m_movementSpeedHash, 0);
            }
        }

        private void MovePlayer()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_moveRotation, rotationSpeed);
            m_rigidbody.MovePosition(m_rigidbody.position + m_movement * ((defaultSpeed + m_bonusSpeed) * Time.deltaTime));
        }
    }
}
