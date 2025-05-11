using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] float lookSpeed = 2.0f;
    [SerializeField] float sprintMultiplier = 2.0f;
    [SerializeField] GameObject gates;
    [SerializeField] ParticleSystem[] particleSystems;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    Vector3 originalRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rotation = transform.localRotation.eulerAngles;
        rotationY = rotation.y;
        rotationX = rotation.x;
        originalRotation = gates.transform.localEulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        HandleMouseLook();
        HandleMovement();
        HandleInteraction();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Quaternion localRotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        transform.rotation = localRotation;
    }

    void HandleMovement()
    {
        float speed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;

        float moveForward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        Vector3 forward = transform.forward * moveForward;
        Vector3 right = transform.right * moveRight;

        transform.position += forward + right;
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenGates());
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(CloseGates());
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
        }
    }

    IEnumerator OpenGates()
    {
        Quaternion startRot = gates.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, -90, 125);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            gates.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }

    IEnumerator CloseGates()
    {
        Quaternion startRot = gates.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(originalRotation);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            gates.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }
}