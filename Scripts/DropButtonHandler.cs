using UnityEngine;
using Vuforia;

public class DropButtonHandler : MonoBehaviour
{
    public PlaneFinderBehaviour planeFinder; // drag PlaneFinder
    public ModelManager modelManager;        // drag ARCamera / ModelManager
    public float spawnScale = 1.5f;  // bisa diatur di Inspector


    private HitTestResult lastHit;
    private bool hasSpawned = false;         // 🔒 kontrol spawn per siklus

    private bool hasShownSpawnHint = false;


    void OnEnable()
    {
        if (planeFinder != null)
        {
            planeFinder.OnAutomaticHitTest.AddListener(OnHitTest);
            planeFinder.OnInteractiveHitTest.AddListener(OnHitTest);
        }
    }

    void OnDisable()
    {
        if (planeFinder != null)
        {
            planeFinder.OnAutomaticHitTest.RemoveListener(OnHitTest);
            planeFinder.OnInteractiveHitTest.RemoveListener(OnHitTest);
        }
    }

    // Dipanggil terus oleh PlaneFinder selama belum spawn
    public void OnHitTest(HitTestResult result)
    {
        if (hasSpawned) return;

        lastHit = result;

        // STATUS 3: Spawn Object (hanya sekali, tanpa marker name)
        if (!hasShownSpawnHint && !modelManager.CurrentMarkerEmpty())
        {
            modelManager.DebugUI("Spawn Object");
            hasShownSpawnHint = true;
        }
    }


    // Dipanggil oleh BUTTON DROP
    public void OnDropButtonPressed()
    {
        // 🔒 Kalau sudah spawn di siklus ini → jangan spawn lagi
        if (hasSpawned)
        {
            Debug.Log("The model has been spawned");
            return;
        }

        if (modelManager.CurrentMarkerEmpty())
        {
            Debug.Log("Scan marker");
            return;
        }

        if (lastHit == null)
        {
            Debug.Log("Plane not detected");
            return;
        }

        GameObject prefab = modelManager.GetCurrentModelPrefab();
        if (prefab == null)
        {
            Debug.Log("This room is not yet available");
            return;
        }

        // Hapus model lama kalau ada (harusnya tidak ada dalam siklus normal)
        modelManager.SetCurrentModel(null);

        // Spawn prefab di posisi hit test
        GameObject model = Object.Instantiate(prefab);
        model.transform.position = lastHit.Position;
        model.transform.rotation = lastHit.Rotation;

        // ambil scale asli prefab, lalu dikali faktor global
        model.transform.localScale = prefab.transform.localScale * spawnScale;

        modelManager.SetCurrentModel(model);

        hasSpawned = true; // 🔒 kunci spawn

        // Matikan PlaneFinder supaya tidak ganggu gesture interaksi
        if (planeFinder != null)
            planeFinder.gameObject.SetActive(false);

        string markerName = modelManager.GetCurrentMarkerName();
        if (!string.IsNullOrEmpty(markerName))
        {
            // STATUS 4: Room #nama marker
            modelManager.DebugUI($"Room {markerName}");
        }

        Debug.Log("Room" + prefab.name);
    }

    // Dipanggil oleh BUTTON RESCAN
    public void EnableScanAgain()
    {
        hasSpawned = false;
        hasShownSpawnHint = false;
        lastHit = null;

        if (planeFinder != null)
            planeFinder.gameObject.SetActive(true);

        modelManager.ResetScan();
    }
}
