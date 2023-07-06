using UnityEngine;
namespace HyperCasual.Runner
{
public class Car : Spawnable
{
    public float speed = 5f; // Speed of the object

    private void Start()
    {
        // Start the object's movement automatically
        MoveObject();
    }

    private void Update()
    {
        // Check for user input to control the movement
        float movement = Input.GetAxis("Vertical");

        // Calculate the new position of the object based on the speed and input
        Vector3 newPosition = transform.position + transform.forward * movement * speed * Time.deltaTime;

        // Update the position of the object
        transform.position = newPosition;
    }

    private void MoveObject()
    {
        // Automatically move the object forward
        float movement = 1f; // Set a constant forward movement
        Vector3 newPosition = transform.position + transform.forward * movement * speed * Time.deltaTime;
        transform.position = newPosition;
    }
}
}