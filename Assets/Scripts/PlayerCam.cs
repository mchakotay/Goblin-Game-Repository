using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public TextMeshProUGUI interactPrompt;

    public Camera fpCamera;
    public float pickupDistance = 2f;
    InteractableObject obj;

    public Transform orientation;

    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        //lock the cursor in the middle of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        interactPrompt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        //lock the rotation so you cant look farther than 90 degrees straight up or down;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        //world position of where mouse cursor is pointing at (where we are lookign towards)
        Ray ray = fpCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            Debug.DrawRay(ray.origin, ray.direction * pickupDistance, Color.red);

            if (hit.collider.tag == "InteractableObject")
            {
                interactPrompt.enabled = true;

                if (Input.GetKeyDown("e"))
                {
                    //destroy is just an example, we could call a function, might remove later
                    Destroy(hit.transform.parent.gameObject);
                }
            }
            else
            {
                interactPrompt.enabled = false;
            }
        }
        else
        {
            interactPrompt.enabled = false;
        }
        if (Input.GetKeyDown("e") && obj)
        {
            Destroy(obj.transform.parent.gameObject);
            interactPrompt.enabled = false;
        }   
    }
}
