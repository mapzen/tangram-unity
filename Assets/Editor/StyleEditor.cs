using UnityEngine;
using UnityEditor;
using Mapzen;
using Mapzen.Unity;
using Mapzen.VectorData.Filters;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class StyleEditor : EditorBase
{
    [SerializeField]
    private string filterName = "";

    [SerializeField]
    private List<FilterStyleEditor> filterStyleEditors;

    [SerializeField]
    private FeatureStyle style;

    [SerializeField]
    private bool showLayerFoldout;

    private List<string> mapzenLayers = new List<string>(new string[] {
        "boundaries", "buildings", "earth", "landuse", "places", "pois", "roads", "transit", "water",
    });

    [SerializeField]
    private List<string> customLayers = new List<string>();

    public FeatureStyle Style
    {
        get { return style; }
    }

    public StyleEditor(FeatureStyle style)
        : base()
    {
        this.filterStyleEditors = new List<FilterStyleEditor>();
        this.style = style;

        foreach (var filterStyle in style.FilterStyles)
        {
            filterStyleEditors.Add(new FilterStyleEditor(filterStyle));
        }
    }

    public void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            filterName = EditorGUILayout.TextField("Filter name: ", filterName);

            EditorConfig.SetColor(EditorConfig.AddButtonColor);
            if (GUILayout.Button(EditorConfig.AddButtonContent, EditorConfig.SmallButtonWidth))
            {
                // Filters within a style are identified by their filter name
                var queryFilterStyleName = style.FilterStyles.Where(filterStyle => filterStyle.Name == filterName);

                if (filterName.Length == 0)
                {
                    Debug.LogError("The filter name can't be empty");
                }
                else if (queryFilterStyleName.Count() > 0)
                {
                    Debug.LogError("Filter with name " + filterName + " already exists");
                }
                else
                {
                    var filterStyle = new FeatureStyle.FilterStyle(filterName);
                    style.AddFilterStyle(filterStyle);
                    filterStyleEditors.Add(new FilterStyleEditor(filterStyle));
                }
            }
            EditorConfig.ResetColor();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;

        for (int i = filterStyleEditors.Count - 1; i >= 0; i--)
        {
            var editor = filterStyleEditors[i];
            var filterStyle = editor.FilterStyle;

            var state = FoldoutEditor.OnInspectorGUI(editor.GUID.ToString(), filterStyle.Name);

            if (state.show)
            {
                List<string> layers = new List<string>();
                // Merge custom and mapzen layers
                layers.AddRange(mapzenLayers);
                layers.AddRange(customLayers);

                editor.OnInspectorGUI(layers);
            }

            if (state.markedForDeletion)
            {
                style.RemoveFilterStyle(filterStyle);

                // Remove the editor for this filter
                filterStyleEditors.RemoveAt(i);
            }
        }

        EditorGUI.indentLevel--;
    }
}
