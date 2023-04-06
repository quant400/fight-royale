using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class GrabRotation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private float rotationSpeed = 1f;

        private RectTransform rectTransform;
        private StarterAssetsInputs _input;


        private Vector2 startPoint;
        private Vector2 endPoint;
        private bool drag;

        // Start is called before the first frame update
        void Start()
        {
            _input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();

            rectTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseDrag()
        {

        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            startPoint = eventData.pressPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            drag = true;

            float xAxisRotation = _input.look.x * rotationSpeed;

            rectTransform.Rotate(Vector3.down, xAxisRotation, Space.World);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            drag = false;
        }

    }
}
