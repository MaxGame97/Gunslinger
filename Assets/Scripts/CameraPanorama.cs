using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Panorama
{
    public Vector3 startPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero;
    public Vector3 lookPosition = Vector3.zero;

    public float panormaTime = 0f;
    private float currentTime = 0f;

    public Panorama()
    {
    }

    public Panorama(Vector3 startP, Vector3 endP, Vector3 lookP, float time)
    {
        startPosition = startP;
        endPosition = endP;
        lookPosition = lookP;
        panormaTime = time;
    }

    public void StartPanorama(float time)
    {
        currentTime = time + panormaTime;
    }

    public bool GetPanorama(float time, out Vector3 position, out Quaternion rotation)
    {
        float factor = Mathf.Clamp((currentTime - time) / panormaTime, 0f, 1f);

        position = startPosition * factor + endPosition * (1f - factor);
        rotation = Quaternion.LookRotation((lookPosition - position).normalized);

        if (factor == 0f)
            return true;
        else
            return false;
    }
}

public class CameraPanorama : MonoBehaviour {

    [SerializeField] private List<Panorama> panoramas = new List<Panorama>();

    private int currentPanorama = 0;

    public List<Panorama> Panoramas { get { return panoramas; } set { panoramas = value; } }

    // Use this for initialization
    void Start ()
    {
        if (panoramas.Count > 0)
            panoramas[currentPanorama].StartPanorama(Time.time);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position;
        Quaternion rotation;
        
        if(panoramas[currentPanorama].GetPanorama(Time.time, out position, out rotation))
        {
            currentPanorama = (currentPanorama + 1) % panoramas.Count;
            panoramas[currentPanorama].StartPanorama(Time.time);
        }

        transform.position = position;
        transform.rotation = rotation;
    }
}
