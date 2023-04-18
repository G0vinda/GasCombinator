using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class Vaccum : MonoBehaviour
    {
        public float baseSpeed;
        public AnimationCurve speedOverDistance;
        [FormerlySerializedAs("targetPosition")] public Transform targetTransform;

        private MeshCollider m_collider;
        private List<Collider> m_capturedBeans;
        private float totalDistance = 6.5f;

        void Awake()
        {
            m_capturedBeans = new List<Collider>();
            m_collider = GetComponent<MeshCollider>();
            gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Bean.Bean>(out var bean) && !m_capturedBeans.Contains(other))
            {
                Debug.Log("Bean captured: " + other.gameObject.name);
                m_capturedBeans.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Bean dropped: " + other.gameObject.name);
            m_capturedBeans.Remove(other);
        }

        void Update()
        {
            m_capturedBeans.RemoveAll(bean => bean == null);
            foreach (var capturedBean in m_capturedBeans)
            {
                var a = Math.Abs(Vector3.Distance(capturedBean.transform.position, targetTransform.position)) / totalDistance;
                var speedMultiplier = speedOverDistance.Evaluate(1.0f -
                    a);
                Debug.Log(a);
                Debug.Log(speedMultiplier);
                Vector3 newPos = Vector3.MoveTowards(
                    capturedBean.transform.position, targetTransform.position, baseSpeed * 
                                                                               speedMultiplier * Time.deltaTime);
                capturedBean.transform.position = new Vector3(newPos.x, capturedBean.transform.position.y, newPos.z);

            }
        }
    }
}