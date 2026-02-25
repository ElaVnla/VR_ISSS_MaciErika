using UnityEngine;

public class DebrisRespawner : MonoBehaviour
{
    [System.Serializable]
    public class DebrisInfo
    {
        public GameObject obj;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }

    public DebrisInfo[] debris;

    private void Awake()
    {
        // Auto-fill from children if you want:
        // (Put this script on DebrisParent and assign in inspector OR use the auto method below)
    }

    public void Capture()
    {
        // Call once at start (optional if you manually assign)
        for (int i = 0; i < debris.Length; i++)
        {
            if (debris[i].obj == null) continue;
            Transform t = debris[i].obj.transform;
            debris[i].pos = t.position;
            debris[i].rot = t.rotation;
            debris[i].scale = t.localScale;
        }
    }

    public void RespawnAll()
    {
        for (int i = 0; i < debris.Length; i++)
        {
            if (debris[i].obj == null) continue;

            var go = debris[i].obj;
            go.SetActive(true);

            Transform t = go.transform;
            t.position = debris[i].pos;
            t.rotation = debris[i].rot;
            t.localScale = debris[i].scale;
        }
    }
}