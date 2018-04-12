using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraPanorama : MonoBehaviour {

    [System.Serializable]
    private class Panorama
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3 startRotation;
        public Vector3 endRotation;

        public float panormaTime;
        private float currentTime;

        public Panorama(Vector3 startP, Vector3 endP, Vector3 startR, Vector3 endR, float time)
        {
            startPosition = startP;
            endPosition = endP;
            startRotation = startR;
            endRotation = endR;
            panormaTime = time;
        }

        public void StartPanorama()
        {
            currentTime = Time.time + panormaTime;
        }

        public bool GetPanorama(out Vector3 position, out Vector3 rotation)
        {
            float time = Mathf.Clamp((currentTime - Time.time) / panormaTime, 0f, 1f);

            position = startPosition * time + endPosition * (1f - time);
            rotation = startRotation * time + endRotation * (1f - time);
            
            if (time == 0f)
                return true;
            else
                return false;
        }
    }

    [SerializeField] private Panorama[] panoramas;

    private int currentPanorama = 0;

    // Use this for initialization
    void Start ()
    {
        if (panoramas.Length > 0)
            panoramas[currentPanorama].StartPanorama();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position;
        Vector3 rotation;
        
        if(panoramas[currentPanorama].GetPanorama(out position, out rotation))
        {
            currentPanorama = (currentPanorama + 1) % panoramas.Length;
            panoramas[currentPanorama].StartPanorama();
        }

        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
