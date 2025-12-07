using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance;

    [Header("UI Debug TMP")]
    public TMP_Text debugText;

    [Header("Models")]
    public List<GameObject> modelPrefabs;

    private Dictionary<string, GameObject> modelDict = new Dictionary<string, GameObject>();
    private string currentMarker = "";
    private GameObject currentModel;

    [Header("Freeze Interaction")]
    public bool isFrozen = false;
    public Image freezeIcon;
    public Sprite freezeOnSprite;
    public Sprite freezeOffSprite;

    // ====== FLOW STATE (PER SIKLUS) ======
    private bool markerLocked = false;   // marker hanya boleh “dipilih” sekali per siklus

    void Awake()
    {
        Instance = this;

        foreach (var prefab in modelPrefabs)
        {
            if (prefab != null)
                modelDict[prefab.name] = prefab;
        }

        // STATUS awal: Scanning...
        DebugUI("Scanning...");
        UpdateFreezeIcon();
    }

    // ================= DEBUG UI =================
    public void DebugUI(string msg)
    {
        Debug.Log("[DEBUG] " + msg);
        if (debugText != null) debugText.text = msg;
    }

    // ============== MARKER EVENTS ==============
    public void OnMarkerDetected(string markerName)
    {
        // Kalau marker sudah terkunci di siklus ini, abaikan marker lain
        if (markerLocked)
        {
            // Tidak mengubah teks lagi
            return;
        }

        currentMarker = markerName;
        markerLocked = true; // kunci marker untuk 1 siklus

        // STATUS 2: Room #marker – point to a flat surface
        DebugUI($"Room {currentMarker} – point to a flat surface");
    }

    public void OnMarkerLost(string markerName)
    {
        // Marker tetap terkunci dalam 1 siklus,
        // dan kita TIDAK mengubah teks di sini.
    }

    public bool CurrentMarkerEmpty()
    {
        return string.IsNullOrEmpty(currentMarker);
    }

    public GameObject GetCurrentModelPrefab()
    {
        if (string.IsNullOrEmpty(currentMarker)) return null;
        if (!modelDict.ContainsKey(currentMarker)) return null;
        return modelDict[currentMarker];
    }

    // ============== CURRENT MODEL HANDLER ==============
    public void SetCurrentModel(GameObject model)
    {
        if (currentModel != null)
            Destroy(currentModel);

        currentModel = model;

        if (currentModel != null)
        {
            // 🔹 SET SHADOW UNTUK SEMUA MESH DI DALAM MODEL
            var renderers = currentModel.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers)
            {
                // aktifkan cast & receive shadows
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                r.receiveShadows = true;
            }

            // Kalau sedang freeze, model baru juga langsung di-freeze
            if (isFrozen)
            {
                var interaction = currentModel.GetComponent<ObjectInteraction>();
                if (interaction != null)
                    interaction.enabled = false;
            }
        }
    }


    // Dipanggil oleh tombol RESCAN (via DropButtonHandler)
    public void ResetScan()
    {
        if (currentModel != null)
            Destroy(currentModel);

        currentModel = null;
        currentMarker = "";
        markerLocked = false;
        isFrozen = false;
        UpdateFreezeIcon();

        // STATUS 1: kembali ke Scanning...
        DebugUI("Scanning...");
    }

    // ============== RECENTER MODEL ==============
    public void ResetModelTransform()
    {
        if (currentModel == null) return;

        var interaction = currentModel.GetComponent<ObjectInteraction>();
        if (interaction != null)
            interaction.ResetTransform();
    }

    // ============== FREEZE TOGGLE ==============
    public void ToggleFreeze()
    {
        isFrozen = !isFrozen;

        if (currentModel != null)
        {
            var interaction = currentModel.GetComponent<ObjectInteraction>();
            if (interaction != null)
                interaction.enabled = !isFrozen;   // freeze = matikan interaksi
        }

        UpdateFreezeIcon();

        // STATUS 5 & 4: Room #marker Object Freeze / Room #marker
        if (string.IsNullOrEmpty(currentMarker))
        {
            // Tidak mengubah teks kalau belum ada marker
            return;
        }
        else
        {
            if (isFrozen)
                DebugUI($"Room {currentMarker} Object Freeze");   // Freeze ON
            else
                DebugUI($"Room {currentMarker}");                 // Freeze OFF → kembali ke Room #marker
        }
    }

    private void UpdateFreezeIcon()
    {
        if (freezeIcon == null) return;

        if (isFrozen)
        {
            if (freezeOnSprite != null)
                freezeIcon.sprite = freezeOnSprite;
        }
        else
        {
            if (freezeOffSprite != null)
                freezeIcon.sprite = freezeOffSprite;
        }
    }

    public string GetCurrentMarkerName()
    {
        return currentMarker;
    }
}
