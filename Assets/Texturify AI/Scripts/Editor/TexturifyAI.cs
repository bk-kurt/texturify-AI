using System;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections.Generic;
using System.IO;

namespace Texturify
{
 public class TexturifyAI : EditorWindow
{
    private EditorDataModel dataModel = new EditorDataModel();
    private List<Texture2D> loadedTextures = new List<Texture2D>();

    private string _styleInput = "";
    private bool _isGeneratingTexture;
    private Vector2 _scrollPosition = Vector2.zero;

    private GUIStyle _applyButtonStyle;
    private GUIStyle _deleteButtonStyle;

    [MenuItem("Tools/Texturify AI")]
    public static void ShowWindow()
    {
        GetWindow<TexturifyAI>();
    }

    void OnEnable()
    {
        // Load the stored values when the window is enabled
        dataModel.LoadPreferences();
    }

    void OnGUI()
    {
        InitializeGUIStyles();
        DrawEditorLayout();
        Repaint();
    }

    private void DrawEditorLayout()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.normal.textColor = GUI.skin.label.normal.textColor;

        GUIStyle aiStyle = new GUIStyle(GUI.skin.label);
        aiStyle.normal.textColor = new Color(0.1f, 0.6f, 0);
        aiStyle.fontSize = 16;
        aiStyle.alignment = TextAnchor.UpperCenter;
        aiStyle.fontStyle = FontStyle.Bold;

        GUILayout.Space(5);

        float windowWidth = position.width;
        float labelWidth = windowWidth / 2;
        float labelHeight = 20;

        // Center the title and AI on the same line with different styles
        GUI.BeginGroup(new Rect(0, 5, windowWidth, 60));

        GUILayout.Label(dataModel.Version, EditorStyles.miniLabel);

        GUI.Label(new Rect((windowWidth - labelWidth) / 2 - 15, 0, labelWidth, labelHeight), "Texturify", titleStyle);


        GUI.Label(
            new Rect((windowWidth - labelWidth) / 2 - 25 + GUI.skin.label.CalcSize(new GUIContent("Texturify")).x, 0,
                labelWidth, labelHeight), "AI", aiStyle);

        GUI.EndGroup();


        GUIStyle howToStyle = new GUIStyle(EditorStyles.miniButton)
        {
            alignment = TextAnchor.MiddleCenter
        };


        float howToLabelWidth = 70;
        float howToLabelHeight = 20;
        Rect howToRect =
            new Rect(windowWidth - howToLabelWidth - 10, 5, howToLabelWidth, howToLabelHeight);

        if (GUI.Button(howToRect, "How to?", howToStyle))
        {
            Application.OpenURL(dataModel.GuideLink);
        }

        float feedbackLabelWidth = 70;
        float feedbackLabelHeight = howToLabelHeight + 5;
        Rect feedbackRect =
            new Rect(windowWidth - feedbackLabelWidth - 10, feedbackLabelHeight, feedbackLabelWidth,
                feedbackLabelHeight);

        if (GUI.Button(feedbackRect, "Feedback", howToStyle))
        {
            string email = dataModel.FeedbackAddress;
            string subject = Uri.EscapeDataString("Feedback on Texturify AI " + dataModel.Version);
            string body = Uri.EscapeDataString("Please enter your feedback here...");

            Application.OpenURL($"mailto:{email}?subject={subject}&body={body}");
        }

        GUILayout.Space(40);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Your API Key:", EditorStyles.boldLabel);

        string apiKey = dataModel.APIKey;
        string newApiKey = EditorGUILayout.TextField(apiKey);
        if (newApiKey != apiKey)
        {
            dataModel.APIKey = newApiKey;
            dataModel.SavePreferences();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Your Organization Key:", EditorStyles.boldLabel);

        string organizationKey = dataModel.OrganizationKey;
        string newOrganizationKey = EditorGUILayout.TextField(organizationKey);
        if (newOrganizationKey != organizationKey)
        {
            dataModel.OrganizationKey = newOrganizationKey;
            dataModel.SavePreferences();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(60);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Type your style: ", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        _styleInput = EditorGUILayout.TextArea(_styleInput, GUILayout.Height(50), GUILayout.ExpandWidth(true));

        GUILayout.Space(15);


        GUILayout.BeginHorizontal();

        GUILayout.Label("Size", GUILayout.Width(50));
        dataModel.TextureSizeIndex =
            EditorGUILayout.Popup(dataModel.TextureSizeIndex, GetTextureSizeOptions(),
                GUILayout.Width(70));

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Shader", GUILayout.Width(50));

        // Use a text field for shader name input
        dataModel.SelectedShaderName = EditorGUILayout.TextField(dataModel.SelectedShaderName, GUILayout.Width(150));

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        // make generation button status-sensitive
        string buttonLabel = _isGeneratingTexture ? "Generating..." : "Generate New Texture";
        if (GUILayout.Button(buttonLabel, GUILayout.Height(40)) && !_isGeneratingTexture)
        {
            StartTextureGeneration();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Display generated textures in a scrollable view
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("- Generated Textures -", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));

        DisplayGeneratedTextures();

        EditorGUILayout.EndScrollView();
    }


    private void InitializeGUIStyles()
    {
        _applyButtonStyle = new GUIStyle(GUI.skin.button);
        _deleteButtonStyle = new GUIStyle(GUI.skin.button);
    }

    string[] GetTextureSizeOptions()
    {
        int[] textureSizeOptions = dataModel.TextureSizeOptions;
        string[] options = new string[textureSizeOptions.Length];
        for (int i = 0; i < textureSizeOptions.Length; i++)
        {
            options[i] = textureSizeOptions[i].ToString();
        }

        return options;
    }

    private void StartTextureGeneration()
    {
        _isGeneratingTexture = true;
        EditorCoroutineUtility.StartCoroutineOwnerless(
            TextureUtility.GenerateTexture(_styleInput, GetSelectedTextureSize(), OnTextureGenerated, dataModel.APIKey,
                dataModel.OrganizationKey)
        );
    }

    private void OnTextureGenerated(Texture2D texture)
    {
        if (texture != null)
        {
            dataModel.GeneratedTextures.Add(texture);
            TextureUtility.SaveTexture(texture, dataModel.texturesFolderPath, _styleInput);
        }

        _isGeneratingTexture = false;
    }

    private int GetSelectedTextureSize()
    {
        return dataModel.TextureSizeIndex;
    }

    private void DisplayGeneratedTextures()
    {
        LoadTexturesFromFolder(dataModel.texturesFolderPath);

        int texturesPerRow = 5;
        float textureButtonSize = 80;
        float buttonHeight = 20;
        float verticalSpacing = 5;

        for (int i = 0; i < loadedTextures.Count; ++i)
        {
            if (i % texturesPerRow == 0)
            {
                GUILayout.BeginHorizontal();
            }

            Texture2D texture = loadedTextures[i];

            GUILayout.BeginVertical(GUILayout.Width(textureButtonSize),
                GUILayout.Height(textureButtonSize + buttonHeight));

            Rect textureRect = GUILayoutUtility.GetRect(textureButtonSize, textureButtonSize);
            GUI.DrawTexture(textureRect, texture);

            // Overlay delete button in the top right corner of the texture
            Rect deleteButtonRect =
                new Rect(textureRect.xMax - buttonHeight, textureRect.yMin, buttonHeight, buttonHeight);
            if (GUI.Button(deleteButtonRect, "X", _deleteButtonStyle))
            {
                DeleteTextureFile(texture);
            }

            if (GUILayout.Button("Test", _applyButtonStyle, GUILayout.Width(textureButtonSize),
                    GUILayout.Height(buttonHeight)))
            {
                TextureUtility.ApplyTextureToSelectedObjects(texture);
            }

            if (GUILayout.Button("Create Mat.", _applyButtonStyle, GUILayout.Width(textureButtonSize),
                    GUILayout.Height(buttonHeight)))
            {
                MaterialUtility.CreateMaterialWithTexture(texture, dataModel.SelectedShaderName,
                    dataModel.materialsFolderPath, _styleInput);
            }

            GUILayout.EndVertical();

            if (i % texturesPerRow == texturesPerRow - 1 || i == loadedTextures.Count - 1)
            {
                GUILayout.EndHorizontal();
            }

            // Add vertical spacing between loaded textures
            GUILayout.Space(verticalSpacing);
        }
    }


    private void DeleteTextureFile(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        AssetDatabase.DeleteAsset(path);
        Debug.Log("Deleted texture asset: " + path);
    }


    private void LoadTexturesFromFolder(string folderPath)
    {
        // Clear the existing loaded textures list
        loadedTextures.Clear();

        // Check if the specified folder exists, then add text. to list if yes
        if (Directory.Exists(folderPath))
        {
            string[] texturePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly);

            foreach (string texturePath in texturePaths)
            {
                Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                if (loadedTexture != null)
                {
                    loadedTextures.Add(loadedTexture);
                }
            }
        }
        else
        {
            Debug.LogWarning("Folder not found at path: " + folderPath);
        }
    }


    // deprecating this for v1.0 of texturify AI.  you may use string-o-matic for next versions.
    // use manual input as temp solution.

    /*
    private void InitializeShaderOptions()
    {
        List<string> shaderList = new List<string>();

        // Manually add common built-in shaders
        shaderList.Add("Standard");
        shaderList.Add("Mobile/Diffuse");
        shaderList.Add("Sprites/Default");
        // ... Add other common built-in shaders as needed

        // Find all custom shaders in the project
        string[] guids = AssetDatabase.FindAssets("t:Shader");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader != null)
            {
                shaderList.Add(shader.name);
            }
        }

        // Fallback if no shaders were found
        if (shaderList.Count == 0)
        {
            shaderList.Add("No Shaders Found");
        }
    }
    */
}   
}
