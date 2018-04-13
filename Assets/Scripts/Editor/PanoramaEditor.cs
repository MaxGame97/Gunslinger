using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPanorama))]
public class PanoramaEditor : Editor {

    private int selected = 0;
    private int preview = 0;

    private Vector3 currentStartPosition = Vector3.zero;
    private Vector3 currentEndPosition = Vector3.zero;
    private Vector3 currentLookPosition = Vector3.zero;
    private float currentTime = 0f;

    void OnEnable()
    {
        EditorApplication.update += Update;
    }

    void Update()
    {
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        if (cameraPanorama.Panoramas.Count > 0 && preview < cameraPanorama.Panoramas.Count)
        {
            Vector3 position;
            Quaternion rotation;

            if (!cameraPanorama.Panoramas[preview].GetPanorama((float)EditorApplication.timeSinceStartup, out position, out rotation))
            {
                cameraPanorama.transform.position = position;
                cameraPanorama.transform.rotation = rotation;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        if (cameraPanorama.Panoramas.Count > 0)
        {
            GUILayout.Label("Currently editing Panorama " + selected + " out of " + (cameraPanorama.Panoramas.Count - 1));
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Panorama") && selected > 0)
            {
                selected--;
            }
            if (GUILayout.Button("Next Panorama") && selected < cameraPanorama.Panoramas.Count - 1)
            {
                selected++;
            }
            EditorGUILayout.EndHorizontal();

            LoadPanoramaValues(cameraPanorama);

            GUILayout.Space(15f);

            GUILayout.Label("Start Position:");
            EditorGUILayout.BeginHorizontal();
            currentStartPosition.x = EditorGUILayout.FloatField(currentStartPosition.x);
            currentStartPosition.y = EditorGUILayout.FloatField(currentStartPosition.y);
            currentStartPosition.z = EditorGUILayout.FloatField(currentStartPosition.z);
            EditorGUILayout.EndHorizontal();

            if (currentStartPosition == currentLookPosition)
            {
                GUILayout.Label("WARNING - The Start Position should not be the same as the Look Position");
                GUILayout.Space(5f);
            }

            GUILayout.Label("End Position:");
            EditorGUILayout.BeginHorizontal();
            currentEndPosition.x = EditorGUILayout.FloatField(currentEndPosition.x);
            currentEndPosition.y = EditorGUILayout.FloatField(currentEndPosition.y);
            currentEndPosition.z = EditorGUILayout.FloatField(currentEndPosition.z);
            EditorGUILayout.EndHorizontal();

            if(currentEndPosition == currentLookPosition)
            {
                GUILayout.Label("WARNING - The End Position should not be the same as the Look Position");
                GUILayout.Space(5f);
            }

            GUILayout.Label("Look Position:");
            EditorGUILayout.BeginHorizontal();
            currentLookPosition.x = EditorGUILayout.FloatField(currentLookPosition.x);
            currentLookPosition.y = EditorGUILayout.FloatField(currentLookPosition.y);
            currentLookPosition.z = EditorGUILayout.FloatField(currentLookPosition.z);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Time:");
            currentTime = EditorGUILayout.FloatField(currentTime);

            if (currentTime <= 0f)
            {
                GUILayout.Label("WARNING - The Time value should be larger than 0");
                GUILayout.Space(5f);
            }


            SavePanoramaValues(cameraPanorama);

            GUILayout.Space(15f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New Panorama"))
            {
                cameraPanorama.Panoramas.Add(new Panorama());

                selected = cameraPanorama.Panoramas.Count - 1;
            }
            if (GUILayout.Button("Delete Panorama"))
            {
                cameraPanorama.Panoramas.RemoveAt(selected);

                selected--;

                if (selected < 0)
                    selected = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Move Panorama to the left") && cameraPanorama.Panoramas.Count > 1 && selected > 0)
            {
                Panorama temp = cameraPanorama.Panoramas[selected];
                cameraPanorama.Panoramas[selected] = cameraPanorama.Panoramas[selected - 1];
                cameraPanorama.Panoramas[selected - 1] = temp;

                selected--;
            }
            if (GUILayout.Button("Move Panorama to the right") && cameraPanorama.Panoramas.Count > 1 && selected < cameraPanorama.Panoramas.Count - 1)
            {
                Panorama temp = cameraPanorama.Panoramas[selected];
                cameraPanorama.Panoramas[selected] = cameraPanorama.Panoramas[selected + 1];
                cameraPanorama.Panoramas[selected + 1] = temp;

                selected++;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Preview Panorama"))
            {
                preview = selected;
                cameraPanorama.Panoramas[preview].StartPanorama((float)EditorApplication.timeSinceStartup);
            }
        }
        else
        {
            GUILayout.Label("Please create a new Panorama before editing");

            if (GUILayout.Button("New Panorama"))
            {
                cameraPanorama.Panoramas.Add(new Panorama());

                selected = 0;
            }
        }
    }

    void OnSceneGUI()
    {
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        if (cameraPanorama.Panoramas.Count > 0)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 startPosition;
                Vector3 endPosition;
                Vector3 lookPosition;

                for (int i = 0; i < cameraPanorama.Panoramas.Count; i++)
                {
                    startPosition = cameraPanorama.Panoramas[i].startPosition;
                    endPosition = cameraPanorama.Panoramas[i].endPosition;
                    lookPosition = cameraPanorama.Panoramas[i].lookPosition;

                    Handles.DrawLine(startPosition, endPosition);

                    Handles.DrawLine(startPosition, startPosition + (lookPosition - startPosition).normalized * 5);
                    Handles.ConeHandleCap(0, startPosition + (lookPosition - startPosition).normalized * 5, Quaternion.LookRotation((lookPosition - startPosition).normalized), 1f, EventType.Repaint);

                    Handles.DrawLine(endPosition, endPosition + (lookPosition - endPosition).normalized * 5);
                    Handles.ConeHandleCap(0, endPosition + (lookPosition - endPosition).normalized * 5, Quaternion.LookRotation((lookPosition - endPosition).normalized), 1f, EventType.Repaint);
                }
            }

            LoadPanoramaValues(cameraPanorama);

            currentStartPosition = Handles.PositionHandle(currentStartPosition, Quaternion.identity);
            currentEndPosition = Handles.PositionHandle(currentEndPosition, Quaternion.identity);
            currentLookPosition = Handles.PositionHandle(currentLookPosition, Quaternion.identity);

            SavePanoramaValues(cameraPanorama);
        }
    }

    void LoadPanoramaValues(CameraPanorama cameraPanorama)
    {
        currentStartPosition = cameraPanorama.Panoramas[selected].startPosition;
        currentEndPosition = cameraPanorama.Panoramas[selected].endPosition;
        currentLookPosition = cameraPanorama.Panoramas[selected].lookPosition;
        currentTime = cameraPanorama.Panoramas[selected].panormaTime;
    }

    void SavePanoramaValues(CameraPanorama cameraPanorama)
    {
        cameraPanorama.Panoramas[selected].startPosition = currentStartPosition;
        cameraPanorama.Panoramas[selected].endPosition = currentEndPosition;
        cameraPanorama.Panoramas[selected].lookPosition = currentLookPosition;
        cameraPanorama.Panoramas[selected].panormaTime = currentTime;
    }
}
