using UnityEngine;
namespace cowsins
{
    public class PlayerGraphics : MonoBehaviour
    {
        [SerializeField] private Transform player;

        [SerializeField] private Transform orientation; 
        private void Update()
        {
            transform.position = player.position;
            transform.rotation = orientation.rotation;
        }
    }
}