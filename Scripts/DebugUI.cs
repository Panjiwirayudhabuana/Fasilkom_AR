using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public static DebugUI Instance;

    public TMP_Text debugOutput; // Drag TMP_Text ke sini

    void Awake()
    {
        Instance = this;
    }

    public void Log(string msg)
    {
        Debug.Log(msg); // tetap tampil di Console

        if (debugOutput != null)
            debugOutput.text = msg; // tampil di layar
    }
}
