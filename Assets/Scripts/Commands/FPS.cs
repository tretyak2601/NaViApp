using UnityEngine;

public class FPS : MonoBehaviour {

    [SerializeField] int fps;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    void Update () {
        if (fps != Application.targetFrameRate)
            Application.targetFrameRate = fps;
	}
}
