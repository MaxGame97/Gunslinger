using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPanorama))]
public class PanoramaEditor : Editor {

    private int selected = 0;   // Defines which Panorama is selected
    private int preview = 0;    // Defines which Panorama is being previewed
    
    private Vector3 currentStartPosition = Vector3.zero;    // Defines the start position of the currently selected Panorama
    private Vector3 currentEndPosition = Vector3.zero;      // Defines the end position of the currently selected Panorama
    private Vector3 currentLookPosition = Vector3.zero;     // Defines the look position of the currently selected Panorama
    private float currentTime = 0f;                         // Defines the time of the currently selected Panorama

    // OnEnable is called when this script is enabled
    void OnEnable()
    {
        // Subscribe to the editor application update event
        EditorApplication.update += Update;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Panorama controller
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        // If the preview Panorama exists
        if (preview >= 0 && preview < cameraPanorama.Panoramas.Count)
        {
            Vector3 position;
            Quaternion rotation;

            // If the preview Panorama has ended
            if (cameraPanorama.Panoramas[preview].GetPanorama((float)EditorApplication.timeSinceStartup, out position, out rotation))
            {
                // Set the preview Panorama to as undefined
                preview = -1;
            }

            // Transform the Panorama controller accordingly
            cameraPanorama.transform.position = position;
            cameraPanorama.transform.rotation = rotation;
        }
    }

    // OnInspectorGUI Adds functionality to the inspector
    public override void OnInspectorGUI()
    {
        // Get the Panorama controller
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        // If there exists Panoramas
        if (cameraPanorama.Panoramas.Count > 0)
        {
            GUILayout.Label("Currently editing Panorama " + selected + " out of " + (cameraPanorama.Panoramas.Count - 1));
            
            EditorGUILayout.BeginHorizontal();
            // Selects the previous Panorama
            if (GUILayout.Button("Previous Panorama") && selected > 0)
            {
                selected--;
            }
            // Selects the next Panorama
            if (GUILayout.Button("Next Panorama") && selected < cameraPanorama.Panoramas.Count - 1)
            {
                selected++;
            }
            EditorGUILayout.EndHorizontal();

            // Loads the Panorama values
            LoadPanoramaValues(cameraPanorama);

            GUILayout.Space(15f);

            GUILayout.Label("Start Position:");
            EditorGUILayout.BeginHorizontal();
            // Edits the start position values
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
            // Edits the end position values
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
            // Edits the look position values
            currentLookPosition.x = EditorGUILayout.FloatField(currentLookPosition.x);
            currentLookPosition.y = EditorGUILayout.FloatField(currentLookPosition.y);
            currentLookPosition.z = EditorGUILayout.FloatField(currentLookPosition.z);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Time:");
            // Edits the time value
            currentTime = EditorGUILayout.FloatField(currentTime);

            if (currentTime <= 0f)
            {
                GUILayout.Label("WARNING - The Time value should be larger than 0");
                GUILayout.Space(5f);
            }

            // Saves the panorama values
            SavePanoramaValues(cameraPanorama);

            GUILayout.Space(15f);

            EditorGUILayout.BeginHorizontal();
            // Adds a new Panorama (and selects it)
            if (GUILayout.Button("New Panorama"))
            {
                cameraPanorama.Panoramas.Add(new Panorama());

                selected = cameraPanorama.Panoramas.Count - 1;
            }
            // Removes a Panorama
            if (GUILayout.Button("Delete Panorama"))
            {
                cameraPanorama.Panoramas.RemoveAt(selected);

                if(selected >= cameraPanorama.Panoramas.Count)
                    selected--;

                if (selected < 0)
                    selected = 0;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            // Moves the currently selected Panorama to the left (and selects it)
            if (GUILayout.Button("Move Panorama to the left") && cameraPanorama.Panoramas.Count > 1 && selected > 0)
            {
                Panorama temp = cameraPanorama.Panoramas[selected];
                cameraPanorama.Panoramas[selected] = cameraPanorama.Panoramas[selected - 1];
                cameraPanorama.Panoramas[selected - 1] = temp;

                selected--;
            }
            // Moves the currently selected Panorama to the right (and selects it)
            if (GUILayout.Button("Move Panorama to the right") && cameraPanorama.Panoramas.Count > 1 && selected < cameraPanorama.Panoramas.Count - 1)
            {
                Panorama temp = cameraPanorama.Panoramas[selected];
                cameraPanorama.Panoramas[selected] = cameraPanorama.Panoramas[selected + 1];
                cameraPanorama.Panoramas[selected + 1] = temp;

                selected++;
            }
            EditorGUILayout.EndHorizontal();

            // Previews the currently selected Panorama
            if (GUILayout.Button("Preview Panorama"))
            {
                preview = selected;
                cameraPanorama.Panoramas[preview].StartPanorama((float)EditorApplication.timeSinceStartup);
            }
        }
        else
        {
            GUILayout.Label("Please create a new Panorama before editing");

            // Adds a new Panorama (and selects it)
            if (GUILayout.Button("New Panorama"))
            {
                cameraPanorama.Panoramas.Add(new Panorama());

                selected = 0;
            }
        }
    }

    // OnSceneGUI draws to the scene window
    void OnSceneGUI()
    {
        // Get the Panorama controller
        CameraPanorama cameraPanorama = (CameraPanorama)target;

        // If there exists Panoramas
        if (cameraPanorama.Panoramas.Count > 0)
        {
            // If the scene should be redrawn
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 startPosition;
                Vector3 endPosition;
                Vector3 lookPosition;

                // Iterate through all Panoramas
                for (int i = 0; i < cameraPanorama.Panoramas.Count; i++)
                {
                    startPosition = cameraPanorama.Panoramas[i].startPosition;  // Get the start position
                    endPosition = cameraPanorama.Panoramas[i].endPosition;      // Get the end position
                    lookPosition = cameraPanorama.Panoramas[i].lookPosition;    // Get the look position

                    // Draw a line between the start and end positions
                    Handles.DrawLine(startPosition, endPosition);

                    // Draw a line and a cone pointing from the start position to the look position
                    Handles.DrawLine(startPosition, startPosition + (lookPosition - startPosition).normalized * 5);
                    Handles.ConeHandleCap(0, startPosition + (lookPosition - startPosition).normalized * 5, Quaternion.LookRotation((lookPosition - startPosition).normalized), 1f, EventType.Repaint);

                    // Draw a line and a cone pointing from the end position to the look position
                    Handles.DrawLine(endPosition, endPosition + (lookPosition - endPosition).normalized * 5);
                    Handles.ConeHandleCap(0, endPosition + (lookPosition - endPosition).normalized * 5, Quaternion.LookRotation((lookPosition - endPosition).normalized), 1f, EventType.Repaint);
                }
            }

            // Load the Panorama values
            LoadPanoramaValues(cameraPanorama);

            // Update the start, end and look positions from input (position handles)
            currentStartPosition = Handles.PositionHandle(currentStartPosition, Quaternion.identity);
            currentEndPosition = Handles.PositionHandle(currentEndPosition, Quaternion.identity);
            currentLookPosition = Handles.PositionHandle(currentLookPosition, Quaternion.identity);

            // Save the Panorama values
            SavePanoramaValues(cameraPanorama);
        }
    }

    // Loads the Panorama values from the currently selected Panorama
    void LoadPanoramaValues(CameraPanorama cameraPanorama)
    {
        currentStartPosition = cameraPanorama.Panoramas[selected].startPosition;
        currentEndPosition = cameraPanorama.Panoramas[selected].endPosition;
        currentLookPosition = cameraPanorama.Panoramas[selected].lookPosition;
        currentTime = cameraPanorama.Panoramas[selected].panormaTime;
    }

    // Saves the Panorama values to the currently selected Panorama
    void SavePanoramaValues(CameraPanorama cameraPanorama)
    {
        cameraPanorama.Panoramas[selected].startPosition = currentStartPosition;
        cameraPanorama.Panoramas[selected].endPosition = currentEndPosition;
        cameraPanorama.Panoramas[selected].lookPosition = currentLookPosition;
        cameraPanorama.Panoramas[selected].panormaTime = currentTime;
    }
}
