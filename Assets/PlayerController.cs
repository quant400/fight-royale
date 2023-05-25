using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;  // The movement speed of the character
    public GameObject player;

    void Start()
    {
        //if (GetComponent<PhotonView>().IsMine)
        //{
        //    // Get a random color
        //    Color randomColor = new Color(Random.value, Random.value, Random.value);

        //    // Get the object's Renderer component
        //    Renderer renderer = player.GetComponent<Renderer>();

        //    // Set the material color to the random color
        //    renderer.material.color = randomColor;
        //}
    }

    void Update()
    {
        if(GetComponent<PhotonView>().IsMine)
        {
            // Get the horizontal and vertical axis inputs
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate the new position of the character based on the input and speed
            Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

            // Move the character to the new position
            transform.position = newPosition;
        }
    }
}
