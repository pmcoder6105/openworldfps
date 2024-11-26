using UnityEngine;
using UnityEngine.Events;

namespace cowsins
{
    public class Trigger : MonoBehaviour
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent onEnter, onStay, onExit;
        }

        [SerializeField] private Events events;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) events.onEnter?.Invoke();
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player")) events.onStay?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) events.onExit?.Invoke();
        }
    }

}