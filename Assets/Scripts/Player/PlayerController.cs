using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;

        private Vector3 _movement;
        private Quaternion _moveRotation;
    
        // Gets called from Player Input Component
        public void OnMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            Debug.Log($"MoveValue is: {move}");
            _movement = new Vector3(move.x, 0f, move.y);

            if(move != Vector2.zero)
                _moveRotation = Quaternion.LookRotation(_movement);
        }

        void Update()
        {
            MovePlayer();
        }

        public void MovePlayer()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _moveRotation, rotationSpeed);
            transform.Translate(_movement * speed * Time.deltaTime, Space.World);
        }
    }
}
