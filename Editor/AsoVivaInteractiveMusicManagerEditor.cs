#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AsoVivaInteractiveMusicManager))]
public class AsoVivaInteractiveMusicManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AsoVivaInteractiveMusicManager music =
            (AsoVivaInteractiveMusicManager)target;

        GUILayout.Space(12);
        GUILayout.Label("AsoViva Debug Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Play All Synced"))
        {
            music.PlayAllSynced();
        }

        if (GUILayout.Button("Change to Walking"))
        {
            music.ChangeState("Walking");
        }

        if (GUILayout.Button("Change to Talking"))
        {
            music.ChangeState("Talking");
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Unlock Guitar"))
        {
            music.UnlockLayer("Guitar");
        }

        if (GUILayout.Button("Unlock Cello"))
        {
            music.UnlockLayer("Cello");
        }

        if (GUILayout.Button("Unlock ViolinViola"))
        {
            music.UnlockLayer("ViolinViola");
        }

        if (GUILayout.Button("Unlock Marimba"))
        {
            music.UnlockLayer("Marimba");
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Reset Progress"))
        {
            music.ResetProgress();
        }

        if (GUILayout.Button("Stop All"))
        {
            music.StopAll();
        }
    }
}
#endif