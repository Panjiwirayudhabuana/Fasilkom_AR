using UnityEngine;


public class InfoButton3D : MonoBehaviour
{
    [Header("Info Image For This Room")]
    public Sprite infoSprite;   // gambar info khusus prefab ini

    [Header("Raycast Settings")]
    public float maxRayDistance = 50f;
    public LayerMask hitLayers = ~0;  // default: semua layer

    void Update()
    {
#if UNITY_EDITOR
        // Klik kiri di editor
        if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
#else
        // Tap pertama di layar (HP)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                HandleTap(t.position);
            }
        }
#endif
    }

    void HandleTap(Vector2 screenPos)
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("[InfoButton3D] No MainCamera found.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, hitLayers))
        {
            // Pastikan ray benar-benar kena tombol ini
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                Debug.Log("[InfoButton3D] Hit info button: " + name);

                if (InfoUIManager.Instance == null)
                {
                    Debug.LogWarning("[InfoButton3D] InfoUIManager.Instance is null");
                    return;
                }

                if (infoSprite == null)
                {
                    Debug.LogWarning("[InfoButton3D] infoSprite is null on " + name);
                    return;
                }

                InfoUIManager.Instance.Show(infoSprite);
            }
        }
    }
}

