using UnityEngine;
using Vuforia;

// Alias biar tidak bentrok dengan Vuforia.Image
using UIImage = UnityEngine.UI.Image;
using UIButton = UnityEngine.UI.Button;

public class FlashButton : MonoBehaviour
{
    [Header("Icon Settings")]
    public UIImage flashIcon;     // drag Image dari button legacy
    public Sprite iconOn;         // sprite ketika flash ON
    public Sprite iconOff;        // sprite ketika flash OFF

    private UIButton button;      // komponen Button (legacy)
    private bool isOn = false;
    private bool isSupported = false;

    void Awake()
    {
        button = GetComponent<UIButton>();
    }

    void Start()
    {
        // Cek apakah Vuforia & CameraDevice sudah ada
        if (VuforiaBehaviour.Instance != null && VuforiaBehaviour.Instance.CameraDevice != null)
        {
            isSupported = VuforiaBehaviour.Instance.CameraDevice.IsFlashSupported();
            Debug.Log("[FlashButton] IsFlashSupported: " + isSupported);
        }
        else
        {
            Debug.LogWarning("[FlashButton] VuforiaBehaviour or CameraDevice is null.");
            isSupported = false;
        }

        // Kalau tidak support, boleh disable tombol
        if (!isSupported && button != null)
        {
            button.interactable = false;  // tombol jadi abu-abu
        }

        UpdateIcon();
    }

    // Dipanggil dari OnClick() di Button
    public void OnFlashButtonClicked()
    {
        Debug.Log("[FlashButton] Button clicked");

        if (!isSupported)
        {
            Debug.Log("[FlashButton] Flash NOT supported on this device/provider.");
            return;
        }

        isOn = !isOn;

        bool ok = VuforiaBehaviour.Instance.CameraDevice.SetFlash(isOn);
        Debug.Log("[FlashButton] SetFlash(" + isOn + ") result = " + ok);

        if (!ok)
        {
            // Kalau gagal nyalain flash, kembalikan state ke OFF
            isOn = false;
        }

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (flashIcon == null) return;

        if (isOn)
        {
            if (iconOn != null)
                flashIcon.sprite = iconOn;
        }
        else
        {
            if (iconOff != null)
                flashIcon.sprite = iconOff;
        }
    }

    // OPTIONAL: kalau kamu mau toggle interaktif tombol dari script lain
    public void SetButtonEnabled(bool enabled)
    {
        if (button != null)
            button.interactable = enabled;
    }

    public bool IsFlashOn()
    {
        return isOn;
    }
}
