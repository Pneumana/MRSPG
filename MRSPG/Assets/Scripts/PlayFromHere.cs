#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PlayFromHere
{
    //very lightly modified script from https://www.reddit.com/r/Unity3D/comments/a82vcm/stop_wasting_time_while_testing_and_play_from/
    //thank you u/yowambo <3



    private static double s_startClickTime;
    private static GameObject s_goHackObject;
    private static Vector3 s_v3Position;

    //private static PlayFromHereInitData s_initDataObject;

    private const string c_strHackName = "Play From Here Hack Object";

    static PlayFromHere()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;

        s_goHackObject = GameObject.Find(c_strHackName);
        if (s_goHackObject != null)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange _eStateChange)
    {
        switch (_eStateChange)
        {
            case PlayModeStateChange.EnteredEditMode:
                //If we are coming back from PlayMode which was started via PlayFromHere, we delete the helper game object
                if (s_goHackObject != null)
                {
                    GameObject.DestroyImmediate(s_goHackObject);
                }
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                //If we entered PlayMode and a Helper Object exists, we should use its position to initialize the PlayFromHere-Objects
                if (s_goHackObject != null)
                {
                    RefreshPlayFromHereDataObject();
                    var check = GameObject.Find("PlayerObj");
                    if (check != null)
                    {
                        check.transform.position = s_goHackObject.transform.position + Vector3.up * 1.5f;
                    }
                }
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    private static void RefreshPlayFromHereDataObject()
    {
        /*if (s_initDataObject == null)
        {
            s_initDataObject = AssetDatabase.LoadAssetAtPath<PlayFromHereInitData>(c_strPlayFromHereObjectPath);
        }*/
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        //Check for right mouse button
        if (Event.current.isMouse && Event.current.button == 1 && !EditorApplication.isPlaying)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                //Save time of mouse down Event to later check for click duration
                s_startClickTime = EditorApplication.timeSinceStartup;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                //Check time since mouse down and open menu for short click
                if (EditorApplication.timeSinceStartup - s_startClickTime < .25f)
                {
                    Camera camera = sceneView.camera;
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit rayhit;

                    //Default layermask is "Everything"
                    LayerMask layerMask = ~0;

                    //Try to get layermask from DataObject
                    RefreshPlayFromHereDataObject();

                    //Get world position for click
                    if (Physics.Raycast(ray, out rayhit, float.MaxValue, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
                    {
                        s_v3Position = rayhit.point + Vector3.up * .1f;

                        //Show confirmation window
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Play From Here"), false, OnConfirmPlayFromHere);

                        menu.ShowAsContext();

                        Event.current.Use();
                    }
                }
            }
        }
    }

    private static void OnConfirmPlayFromHere()
    {
        //Start PlayMode
        EditorApplication.isPlaying = true;

        //Unity reinitializes all scripts when starting PlayMode so we can't save any variables regularly
        //Create a GameObject with "DontSave"-Flag to "save" Position for start of PlayMode
        GameObject goPlayFromHere = new GameObject(c_strHackName);
        goPlayFromHere.transform.position = s_v3Position;
        goPlayFromHere.hideFlags = HideFlags.DontSave;
    }
}
#endif
