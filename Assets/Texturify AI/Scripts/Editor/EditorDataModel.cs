using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Texturify
{
    public class EditorDataModel
    {
        public string GuideLink = "https://youtu.be/wpuMEnOSP80";
        public string FeedbackAddress = "burakkurt2010@gmail.com";
        public string Version = "v1.0";
    
        public string APIKey;
        public string OrganizationKey;
        public List<Texture2D> GeneratedTextures;
        public int TextureSizeIndex;
        public string SelectedShaderName;
        public readonly int[] TextureSizeOptions = { 32, 64, 128, 256, 512, 1024, 2048 };
        public string texturesFolderPath = "Assets/Texturify AI/Generated/Textures";
        public string materialsFolderPath = "Assets/Texturify AI/Generated/Materials";

        // Preference keys
        private const string ApiKeyPref = "YourEditorWindow_ApiKey";
        private const string OrganizationKeyPref = "YourEditorWindow_OrganizationKey";
        private const string TextureSizeIndexPref = "YourEditorWindow_TextureSizeIndex";
        private const string SelectedShaderNamePref = "YourEditorWindow_SelectedShaderName";

        public EditorDataModel()
        {
            GeneratedTextures = new List<Texture2D>();
        }

        public void LoadPreferences()
        {
            APIKey = EditorPrefs.GetString(ApiKeyPref, "");
            OrganizationKey = EditorPrefs.GetString(OrganizationKeyPref, "");
            TextureSizeIndex = EditorPrefs.GetInt(TextureSizeIndexPref, 2); // Default to 128
            SelectedShaderName = EditorPrefs.GetString(SelectedShaderNamePref, "Standard");
        }

        public void SavePreferences()
        {
            EditorPrefs.SetString(ApiKeyPref, APIKey);
            EditorPrefs.SetString(OrganizationKeyPref, OrganizationKey);
            EditorPrefs.SetInt(TextureSizeIndexPref, TextureSizeIndex);
            EditorPrefs.SetString(SelectedShaderNamePref, SelectedShaderName);
        }
    }
}
