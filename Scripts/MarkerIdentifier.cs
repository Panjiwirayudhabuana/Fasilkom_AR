using UnityEngine;
using Vuforia;

public class MarkerIdentifier : MonoBehaviour
{
    public string markerName;
    private ObserverBehaviour observer;

    void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnMarkerStatusChanged;
    }

    private void OnMarkerStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            ModelManager.Instance.OnMarkerDetected(markerName);
        }
        else
        {
            ModelManager.Instance.OnMarkerLost(markerName);
        }
    }
}
