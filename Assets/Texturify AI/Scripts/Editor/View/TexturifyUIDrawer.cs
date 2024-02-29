using System;
using System.Collections.Generic;
using Texturify;
using Texturify_AI.Scripts.Editor.Controller;
using Texturify_AI.Scripts.Editor.Model;
using UnityEditor;
using UnityEngine;

namespace Texturify_AI.Scripts.Editor.View
{
    public class TexturifyUIDrawer
    {
        private readonly EditorDataModel _dataModel;
        private GUIStyle _applyButtonStyle;
        private GUIStyle _deleteButtonStyle;
        private TexturifyAI _window;
        public List<Texture2D> LoadedTextures = new List<Texture2D>();

        private bool _reloadTexturesFlag;
        private bool _stylesInitialized;

        public TexturifyUIDrawer(EditorDataModel model)
        {
            _dataModel = model;
        }

        public void DrawMainLayout(TexturifyAI window)
        {
            if (!_stylesInitialized)
            {
                InitializeGUIStyles();
                _stylesInitialized = true;
            }

            _window = window;
            GUILayout.Space(5);
            DrawHeader();
            DrawButtons();
            DrawApiKeySection();
            DrawOrganizationKeySection();
            DrawStyleInputSection();
            DrawTextureAndShaderOptions();
            DrawGenerateButton(window.isGeneratingTexture);
            DrawGeneratedTexturesSection(window);
        }

        private void InitializeGUIStyles()
        {
            _applyButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 10 };
            _deleteButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 10 };
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Texturify AI - Version {_dataModel.Version}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("How to?", EditorStyles.miniButton))
            {
                Application.OpenURL(_dataModel.GuideLink);
            }

            if (GUILayout.Button("Feedback", EditorStyles.miniButton))
            {
                string email = _dataModel.FeedbackAddress;
                string subject = Uri.EscapeDataString($"Feedback on Texturify AI {_dataModel.Version}");
                string body = Uri.EscapeDataString("Please enter your feedback here...");
                Application.OpenURL($"mailto:{email}?subject={subject}&body={body}");
            }

            GUILayout.EndHorizontal();
        }

        private void DrawApiKeySection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Your API Key:", EditorStyles.boldLabel);
            string newApiKey = EditorGUILayout.TextField(_dataModel.APIKey);
            if (newApiKey != _dataModel.APIKey)
            {
                _dataModel.APIKey = newApiKey;
                _dataModel.SavePreferences();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawOrganizationKeySection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Your Organization Key:", EditorStyles.boldLabel);
            string newOrganizationKey = EditorGUILayout.TextField(_dataModel.OrganizationKey);
            if (newOrganizationKey != _dataModel.OrganizationKey)
            {
                _dataModel.OrganizationKey = newOrganizationKey;
                _dataModel.SavePreferences();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawStyleInputSection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Type your style:", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _window.styleInput =
                EditorGUILayout.TextArea(_window.styleInput, GUILayout.Height(50), GUILayout.ExpandWidth(true));
        }

        private void DrawTextureAndShaderOptions()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Size:", GUILayout.Width(50));
            _dataModel.TextureSizeIndex = EditorGUILayout.Popup(_dataModel.TextureSizeIndex,
                _window.GetTextureSizeOptions(), GUILayout.Width(70));
            GUILayout.Label("Shader:", GUILayout.Width(50));
            _dataModel.SelectedShaderName =
                EditorGUILayout.TextField(_dataModel.SelectedShaderName, GUILayout.Width(150));
            GUILayout.EndHorizontal();
        }

        private void DrawGenerateButton(bool isGeneratingTexture)
        {
            if (GUILayout.Button(isGeneratingTexture ? "Generating..." : "Generate New Texture", GUILayout.Height(40)))
            {
                _window.StartTextureGeneration();
            }
        }

        private void DrawGeneratedTexturesSection(TexturifyAI window)
        {
            TextureUtility.LoadTexturesFromFolder(EditorDataModel.TexturesFolderPath, LoadedTextures);

            int texturesPerRow = 5;
            float textureButtonSize = 80;
            float buttonHeight = 20;
            float verticalSpacing = 5;

            for (int i = 0; i < LoadedTextures.Count; ++i)
            {
                if (i % texturesPerRow == 0)
                {
                    GUILayout.BeginHorizontal();
                }

                Texture2D texture = LoadedTextures[i];

                GUILayout.BeginVertical(GUILayout.Width(textureButtonSize),
                    GUILayout.Height(textureButtonSize + buttonHeight));

                Rect textureRect = GUILayoutUtility.GetRect(textureButtonSize, textureButtonSize);
                GUI.DrawTexture(textureRect, texture);

                Rect deleteButtonRect =
                    new Rect(textureRect.xMax - buttonHeight, textureRect.yMin, buttonHeight, buttonHeight);
                if (GUI.Button(deleteButtonRect, "X", _deleteButtonStyle))
                {
                    TextureUtility.DeleteTextureFile(texture);
                }

                if (GUILayout.Button("Test", _applyButtonStyle, GUILayout.Width(textureButtonSize),
                        GUILayout.Height(buttonHeight)))
                {
                    TextureUtility.ApplyTextureToSelectedObjects(texture);
                }

                if (GUILayout.Button("Create Mat.", _applyButtonStyle, GUILayout.Width(textureButtonSize),
                        GUILayout.Height(buttonHeight)))
                {
                    MaterialUtility.CreateMaterialWithTexture(texture, _dataModel.SelectedShaderName,
                        EditorDataModel.MaterialsFolderPath, window.styleInput);
                }

                GUILayout.EndVertical();

                if (i % texturesPerRow == texturesPerRow - 1 || i == LoadedTextures.Count - 1)
                {
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(verticalSpacing);
            }
        }
    }
}