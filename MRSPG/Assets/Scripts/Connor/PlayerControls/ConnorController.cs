using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnorController : MonoBehaviour
{
    Rigidbody rb;

    public float speed;

    GameObject holder;
    public float jumpStrength;

    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        holder = GameObject.Find("CameraHolder");
    }

    // Update is called once per frame
    void Update()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");
        rb.rotation = Quaternion.Euler(new Vector3(0, holder.GetComponent<ConnorCamHolder>().lookX, 0));
        if(rb.velocity.magnitude < speed)
        {
            rb.AddForce((transform.forward * speed * inputY) + (transform.right * speed * inputX));
        }
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red, Time.deltaTime);

        RaycastHit hit;
        var pos = transform.transform.position + Vector3.down * 0.9f;
        Physics.Raycast(pos, Vector3.down, out hit, 0.1f);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject != this)
                grounded = true;
        }
        else
        {

        }

        InputEventJump();

        
    }
    void InputEventJump()
    {
        if (Gamepad.current == null)
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                transform.position += Vector3.up * Time.deltaTime;
                grounded = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded || Gamepad.current.aButton.wasPressedThisFrame && grounded)
            {
                rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                transform.position += Vector3.up * Time.deltaTime;
                grounded = false;
            }
        }
        
    }
}
