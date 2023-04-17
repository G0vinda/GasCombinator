using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Vaccum : MonoBehaviour
    {
        public float baseSpeed;
        public AnimationCurve speedOverDistance;
        public Vector3 targetPosition;

        private MeshCollider m_collider;
        private List<Collider> m_capturedBeans;

        void Awake()
        {
            m_capturedBeans = new List<Collider>();
            m_collider = GetComponent<MeshCollider>();
            gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!m_capturedBeans.Contains(other))
            {
                m_capturedBeans.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            m_capturedBeans.Remove(other);
        }

        void Update()
        {
            foreach (var capturedBean in m_capturedBeans)
            {
                capturedBean.transform.position = Vector3.MoveTowards(
                    capturedBean.transform.position, targetPosition, baseSpeed * Time.deltaTime);
            }
        }
    }
}