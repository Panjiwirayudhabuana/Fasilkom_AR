using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 0.3f;

    [Header("Zoom / Scale")]
    public float zoomSpeed = 0.008f;
    public float minScale = 1f;
    public float maxScale = 7f;

    // ====== DATA UNTUK RESET ======
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private Vector2 prevTouchPos1;
    private Vector2 prevTouchPos2;

    void Start()
    {
        // Disimpan saat model BARU DI-SPAWN (posisi dari hit test)
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    // ====== DIPANGGIL OLEH BUTTON RESET ======
    public void ResetTransform()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
    }

    // ================= TOUCH (ANDROID) =================
    void HandleTouch()
    {
        // 1 jari → ROTATE
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                float rotX = t.deltaPosition.y * rotationSpeed;
                float rotY = -t.deltaPosition.x * rotationSpeed;

                transform.Rotate(Vector3.up, rotY, Space.World);
                transform.Rotate(Vector3.right, rotX, Space.World);
            }
        }

        // 2 jari → ZOOM
        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 prev1 = t1.position - t1.deltaPosition;
            Vector2 prev2 = t2.position - t2.deltaPosition;

            float prevDist = Vector2.Distance(prev1, prev2);
            float currDist = Vector2.Distance(t1.position, t2.position);
            float delta = currDist - prevDist;

            Vector3 newScale = transform.localScale + Vector3.one * (delta * zoomSpeed);

            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

            transform.localScale = newScale;
        }
    }

    // ================= MOUSE (EDITOR) =================
    void HandleMouse()
    {
        if (Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse Y") * rotationSpeed * 10f;
            float rotY = -Input.GetAxis("Mouse X") * rotationSpeed * 10f;

            transform.Rotate(Vector3.up, rotY, Space.World);
            transform.Rotate(Vector3.right, rotX, Space.World);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            Vector3 newScale = transform.localScale + Vector3.one * (scroll * 5f * zoomSpeed);
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
            transform.localScale = newScale;
        }
    }
}
