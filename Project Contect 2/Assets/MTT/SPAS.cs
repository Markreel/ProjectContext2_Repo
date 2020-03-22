
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;

//Substance Painter Automisation System is a tool made by Mark Meerssman
//This tool is used to automatically assign exported textures from Substance Painter to a new material

//Right-Click in the folder that contains your exported textures (in the Project Window)
//and select: Create>SPAS_Material to create the material
//To change the settings, open the settings window by going to: Window/SPAS


public class SPAS : EditorWindow
{
    public static SPASSettings SPASSettings = new SPASSettings();

    private string materialName = "New Material";
    private string directoryPath = "";
    private string fixedPath = "";

    private static float windowWidth = 400;
    private static float windowHeight = 475;

    [MenuItem("Assets/Create/SPAS_Material", false, 0)]
    private static void CreateMaterialWithSelectedAssetPath()
    {
        CreateMaterial(AssetDatabase.GetAssetPath(Selection.activeObject) + "/");
    }

    [MenuItem("Window/SPAS")]
    private static void Init()
    {
        LoadSettingsFromFile();

        SPAS _window = (SPAS)GetWindow(typeof(SPAS));
        _window.minSize = new Vector2(windowWidth, windowHeight);
    }

    private void DrawHorizontalLine()
    {
        GUIStyle _horizontalLine;
        _horizontalLine = new GUIStyle();
        _horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        _horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        _horizontalLine.fixedHeight = 1;

        Color _oldColor = GUI.color;
        GUI.color = Color.grey;
        GUILayout.Box(GUIContent.none, _horizontalLine);
        GUI.color = _oldColor;
    }

    private void OnGUI()
    {
        GUIStyle _headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold };
        GUIStyle _subHeaderStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };

        #region EditorWindow
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        #region Settings

        //Header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Settings:", _headerStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(30));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //Actual Settings
        SPASSettings.AlbedoColor = EditorGUILayout.ColorField("Albedo Color", SPASSettings.AlbedoColor);
        SPASSettings.Smoothness = EditorGUILayout.Slider("Smoothness", SPASSettings.Smoothness, 0, 1);
        SPASSettings.AlphaSource = (SPASSettings.Source)EditorGUILayout.EnumPopup("Alpha Source", SPASSettings.AlphaSource);
        SPASSettings.Occlusion = EditorGUILayout.Slider("Occlusion", SPASSettings.Occlusion, 0, 1);

        #endregion

        EditorGUILayout.Space();
        DrawHorizontalLine();
        EditorGUILayout.Space();

        #region Prefixes

        EditorGUILayout.BeginHorizontal();

        //Header
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Prefixes: ", _subHeaderStyle);
        EditorGUILayout.EndVertical();

        //Edit button
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Edit Prefixes", GUILayout.Width(80), GUILayout.Height(15)))
        {
            SPASPrefixWindow.Init();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //Prefix Preview
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Albedo: " + '"' + SPASSettings.AlbedoPrefix + '"');
        EditorGUILayout.LabelField("Normal: " + '"' + SPASSettings.NormalPrefix + '"');
        EditorGUILayout.LabelField("Metallic: " + '"' + SPASSettings.MetallicPrefix + '"');
        EditorGUILayout.LabelField("Ambient Occlusion: " + '"' + SPASSettings.AmbientOcclusionPrefix + '"');
        for (int i = 0; i < SPASSettings.NamingFilterCount; i++)
        {
            EditorGUILayout.LabelField("Filter " + i + ": " + '"' + SPASSettings.NamingFilterPrefixes[i] + '"');
        }
        EditorGUILayout.EndVertical();

        #endregion

        #region Reset button

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowWidth / 2 - 100);
        if (GUILayout.Button("Reset to default settings", GUILayout.Width(200), GUILayout.Height(15)))
        {
            if (EditorUtility.DisplayDialog("Reset to default settings", "Are you sure you want to reset to the default settings? " +
                 "Your current settings will be lost forever.", "I'm sure", "Cancel"))
            {
                ResetSettings();
            }
        }
        EditorGUILayout.EndHorizontal();

        #endregion

        DrawHorizontalLine();
        EditorGUILayout.Space();

        #region Create SPAS Item

        //Header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Create SPAS Item", _headerStyle, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginVertical(EditorStyles.helpBox);

        //Name input field
        EditorGUILayout.Space();
        materialName = EditorGUILayout.TextField("Material name: ", materialName);
        EditorGUILayout.Space();

        //Folder select button
        if (GUILayout.Button("Select Textures Folder"))
        {
            directoryPath = EditorUtility.OpenFolderPanel("Select Texture Folder", "", "");
        }

        //Selected folder preview text (shows the selected folder starting from "Assets")
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (directoryPath.Contains("Assets"))
        {
            int _index = directoryPath.IndexOf("Assets");
            fixedPath = directoryPath.Remove(0, _index);
            EditorGUILayout.LabelField(fixedPath);
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //Create asset button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("Create Asset", GUILayout.Width(100), GUILayout.Height(30)))
        {
            CreateMaterial(fixedPath + "/");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndVertical();
        #endregion

        SaveSettingsToFile();
    }

    private void ResetSettings()
    {
        SPASSettings _newSettings = new SPASSettings();
        _newSettings.NamingFilterPrefixes[0] = "_initialShadingGroup_";
        _newSettings.NamingFilterPrefixes[1] = "_lambert1_";

        SPASSettings = _newSettings;
        SaveSettingsToFile();
    }

    private void SaveSettingsToFile()
    {
        BinaryFormatter _bf = new BinaryFormatter();
        FileStream _file = File.Create(Application.persistentDataPath + "/SPASDATA.dat");

        SPASSettingsData _SPASSettingsData = new SPASSettingsData();

        //Color
        _SPASSettingsData.R = SPASSettings.AlbedoColor.r;
        _SPASSettingsData.G = SPASSettings.AlbedoColor.g;
        _SPASSettingsData.B = SPASSettings.AlbedoColor.b;
        _SPASSettingsData.A = SPASSettings.AlbedoColor.a;

        //Settings
        _SPASSettingsData.Smoothness = SPASSettings.Smoothness;
        _SPASSettingsData.AlphaSource = SPASSettings.AlphaSource;
        _SPASSettingsData.Occlusion = SPASSettings.Occlusion;

        //Pprefixes
        _SPASSettingsData.NormalPrefix = SPASSettings.NormalPrefix;
        _SPASSettingsData.AlbedoPrefix = SPASSettings.AlbedoPrefix;
        _SPASSettingsData.MetallicPrefix = SPASSettings.MetallicPrefix;
        _SPASSettingsData.AmbientOcclusionPrefix = SPASSettings.AmbientOcclusionPrefix;

        //NamingFilters
        _SPASSettingsData.NamingFilterCount = SPASSettings.NamingFilterCount;
        _SPASSettingsData.NamingFilters = SPASSettings.NamingFilterPrefixes;

        _bf.Serialize(_file, _SPASSettingsData);
        _file.Close();
    }

    [InitializeOnLoadMethod]
    private static void LoadSettingsFromFile()
    {
        if (File.Exists(Application.persistentDataPath + "/SPASDATA.dat"))
        {
            BinaryFormatter _bf = new BinaryFormatter();
            FileStream _file = File.Open(Application.persistentDataPath + "/SPASDATA.dat", FileMode.Open);
            SPASSettingsData _SPASSettingsData = (SPASSettingsData)_bf.Deserialize(_file);
            _file.Close();

            //Color
            Color _col = new Color(_SPASSettingsData.R, _SPASSettingsData.G, _SPASSettingsData.B, _SPASSettingsData.A);
            SPASSettings.AlbedoColor = _col;

            //Settings
            SPASSettings.Smoothness = _SPASSettingsData.Smoothness;
            SPASSettings.AlphaSource = _SPASSettingsData.AlphaSource;
            SPASSettings.Occlusion = _SPASSettingsData.Occlusion;

            //Prefixes
            SPASSettings.NormalPrefix = _SPASSettingsData.NormalPrefix;
            SPASSettings.AlbedoPrefix = _SPASSettingsData.AlbedoPrefix;
            SPASSettings.MetallicPrefix = _SPASSettingsData.MetallicPrefix;
            SPASSettings.AmbientOcclusionPrefix = _SPASSettingsData.AmbientOcclusionPrefix;

            //Naming Filters
            SPASSettings.NamingFilterCount = _SPASSettingsData.NamingFilterCount;
            SPASSettings.NamingFilterPrefixes = _SPASSettingsData.NamingFilters;
        }
    }

    private static void CreateMaterial(string _directoryPath)
    {
        string[] _files = Directory.GetFiles(_directoryPath, "*.png");
        List<Texture2D> _textures = new List<Texture2D>();

        //Check if any files are found at all
        if (_files.Length == 0)
        {
            Debug.LogError("No .png files found, make sure you've selected the right folder.");
            return;
        }

        foreach (var _file in _files)
        {
            Texture2D _texture = (Texture2D)AssetDatabase.LoadAssetAtPath(_file, typeof(Texture2D));

            _textures.Add(_texture);
        }

        //Check if any of the files are .png
        if (_textures.Count == 0)
        {
            Debug.LogError("No textures found, make sure you've selected the right folder.");
            return;
        }

        //Create and setup new material
        Material _outputMaterial = new Material(Shader.Find("Standard"));
        _outputMaterial.EnableKeyword("_NORMALMAP");
        _outputMaterial.EnableKeyword("_METALLICGLOSSMAP");

        //Set material values according to the settings of the tool
        _outputMaterial.SetInt("_SmoothnessTextureChannel", (int)SPASSettings.AlphaSource);
        _outputMaterial.SetFloat("_GlossMapScale", SPASSettings.Smoothness);
        _outputMaterial.SetColor("_Color", SPASSettings.AlbedoColor);
        _outputMaterial.SetFloat("_OcclusionStrength", SPASSettings.Occlusion);

        string _materialName = "New Material";

        //Go through each texture and check for prefixes in their name. If found, add textures to the new material accordingly.
        foreach (var _texture in _textures)
        {
            if (_texture.name.Contains(SPASSettings.AlbedoPrefix))
            {
                _outputMaterial.SetTexture("_MainTex", _texture);
                _materialName = _texture.name.Remove(_texture.name.IndexOf("AlbedoTransparency"), _texture.name.Length - _texture.name.IndexOf("AlbedoTransparency"));
            }

            if (_texture.name.Contains(SPASSettings.NormalPrefix))
                _outputMaterial.SetTexture("_BumpMap", _texture);

            if (_texture.name.Contains(SPASSettings.MetallicPrefix))
                _outputMaterial.SetTexture("_MetallicGlossMap", _texture);

            if (_texture.name.Contains(SPASSettings.AmbientOcclusionPrefix))
                _outputMaterial.SetTexture("_OcclusionMap", _texture);


        }

        //Filter out unnecessary parts of the material's name based on the naming filter prefixes
        for (int i = 0; i < SPASSettings.NamingFilterCount; i++)
        {
            if (_materialName.Contains(SPASSettings.NamingFilterPrefixes[i]))
            {
                _materialName = _materialName.Remove(_materialName.IndexOf(SPASSettings.NamingFilterPrefixes[i]),
                    _materialName.Length - _materialName.IndexOf(SPASSettings.NamingFilterPrefixes[i]));

            }
        }

        Selection.activeObject = null;
        ProjectWindowUtil.CreateAsset(_outputMaterial, _directoryPath + _materialName + "_mat.mat");
    }
}

public class SPASSettings
{
    public Color AlbedoColor = Color.white;
    public float Smoothness;
    public enum Source { MetallicAlpha, AlbedoAlpha };
    public Source AlphaSource = Source.MetallicAlpha;
    public float Occlusion;

    public string NormalPrefix = "Normal";
    public string AlbedoPrefix = "AlbedoTransparency";
    public string MetallicPrefix = "MetallicSmoothness";
    public string AmbientOcclusionPrefix = "AO";

    public int NamingFilterCount = 2;
    public string[] NamingFilterPrefixes = new string[2];
}

[System.Serializable]
public class SPASSettingsData
{
    public float R = 1;
    public float G = 1;
    public float B = 1;
    public float A = 1;

    public float Smoothness;
    public SPASSettings.Source AlphaSource = SPASSettings.Source.MetallicAlpha;
    public float Occlusion;

    public string NormalPrefix = "Normal";
    public string AlbedoPrefix = "AlbedoTransparency";
    public string MetallicPrefix = "MetallicSmoothness";
    public string AmbientOcclusionPrefix = "AO";

    public int NamingFilterCount;
    public string[] NamingFilters;
}

public class SPASPrefixWindow : EditorWindow
{
    private static float windowWidth = 300;
    private static float windowHeight = 225;

    public static void Init()
    {
        SPASPrefixWindow _window = (SPASPrefixWindow)GetWindow(typeof(SPASPrefixWindow));
        _window.minSize = new Vector2(windowWidth, windowHeight);
    }

    private void OnGUI()
    {
        GUIStyle _headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold };

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        //Header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefixes:", _headerStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(30));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //Textures Prefixes
        SPAS.SPASSettings.AlbedoPrefix = EditorGUILayout.TextField(SPAS.SPASSettings.AlbedoPrefix);
        SPAS.SPASSettings.NormalPrefix = EditorGUILayout.TextField(SPAS.SPASSettings.NormalPrefix);
        SPAS.SPASSettings.MetallicPrefix = EditorGUILayout.TextField(SPAS.SPASSettings.MetallicPrefix);
        SPAS.SPASSettings.AmbientOcclusionPrefix = EditorGUILayout.TextField(SPAS.SPASSettings.AmbientOcclusionPrefix);

        //Filters Header
        EditorGUILayout.LabelField("Filters");
        SPAS.SPASSettings.NamingFilterCount = EditorGUILayout.IntField(SPAS.SPASSettings.NamingFilterCount);

        //Update the length of the "NamingFilterPrefixes" array according to the "NamingFilterCount"
        if (SPAS.SPASSettings.NamingFilterPrefixes.Length != SPAS.SPASSettings.NamingFilterCount)
        {
            SPAS.SPASSettings.NamingFilterPrefixes = new string[SPAS.SPASSettings.NamingFilterCount];
        }

        //Filters Prefixes
        for (int i = 0; i < SPAS.SPASSettings.NamingFilterCount; i++)
        {
            SPAS.SPASSettings.NamingFilterPrefixes[i] = EditorGUILayout.TextField(SPAS.SPASSettings.NamingFilterPrefixes[i]);
        }

        EditorGUILayout.Space();

        //Done button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowWidth / 2 - 50);
        if (GUILayout.Button("Done", GUILayout.Width(100), GUILayout.Height(30)))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }
}