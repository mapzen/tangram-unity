using UnityEngine;
using UnityEditor;
using Mapzen;
using System.Linq;
using Mapzen.Unity;
using System;

[Serializable]
public class PolygonBuilderEditor : EditorBase
{
    [SerializeField]
    private bool show;

    public PolygonBuilderEditor()
        : base()
    {
        this.show = false;
    }

    public static PolygonBuilder.Options DefaultOptions()
    {
        var defaultOptions = new PolygonBuilder.Options();

        defaultOptions.Extrusion = PolygonBuilder.ExtrusionType.TopAndSides;
        defaultOptions.UVMode = UVMode.Tile;
        defaultOptions.Enabled = true;
        defaultOptions.MaxHeight = 0.0f;

        return defaultOptions;
    }

    private void LoadPreferences()
    {
        show = EditorPrefs.GetBool(guid + ".show");
    }

    private void SavePreferences()
    {
        EditorPrefs.SetBool(guid + ".show", show);
    }

    public PolygonBuilder.Options OnInspectorGUI(PolygonBuilder.Options options)
    {
        LoadPreferences();

        show = EditorGUILayout.Foldout(show, "Polygon builder options");

        if (!show)
        {
            SavePreferences();
            return options;
        }

        options.MaxHeight = EditorGUILayout.FloatField("Max Height: ", options.MaxHeight);
        options.Extrusion = (PolygonBuilder.ExtrusionType)EditorGUILayout.EnumPopup("Extrusion type: ", options.Extrusion);
        options.UVMode = (UVMode)EditorGUILayout.EnumPopup("UV Mode:", options.UVMode);
        options.Enabled = EditorGUILayout.Toggle("Enabled: ", options.Enabled);

        SavePreferences();

        return options;
    }
}
