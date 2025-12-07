using UnityEngine;
using UnityEngine.UI;

public class InfoUIManager : MonoBehaviour
{
    public static InfoUIManager Instance;

    [Header("UI Panel")]
    public GameObject panelRoot;   // panel yang berisi Image
    public Image infoImage;        // Image untuk menampilkan gambar info

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panelRoot != null)
            panelRoot.SetActive(false);   // awalnya disembunyikan
    }

    public void Show(Sprite sprite)
    {
        if (panelRoot == null || infoImage == null || sprite == null)
            return;

        infoImage.sprite = sprite;
        infoImage.preserveAspect = true;
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot == null) return;
        panelRoot.SetActive(false);
    }

    void Update()
    {
        if (!IsOpen) return;

#if UNITY_EDITOR
        // Klik kiri di editor → tutup
        if (Input.GetMouseButtonDown(0))
        {
            Hide();
        }
#else
        // Tap pertama di layar → tutup
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                Hide();
        }
#endif
    }
}
