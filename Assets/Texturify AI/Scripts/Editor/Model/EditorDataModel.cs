using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Texturify_AI.Scripts.Editor.Model
{
    public class EditorDataModel
    {
        public readonly string GuideLink = "https://youtu.be/wpuMEnOSP80";
        public readonly string FeedbackAddress = "burakkurt2010@gmail.com";
        public readonly string Version = "v1.0";
    
        public string APIKey;
        public string OrganizationKey;
        public int TextureSizeIndex;
        public string SelectedShaderName;
        public readonly int[] TextureSizeOptions = { 32, 64, 128, 256, 512, 1024, 2048 };
        public const string TexturesFolderPath = "Assets/Texturify AI/Generated/Textures";
        public const string MaterialsFolderPath = "Assets/Texturify AI/Generated/Materials";

        private const string ApiKeyPref = "YourEditorWindow_ApiKey";
        private const string OrganizationKeyPref = "YourEditorWindow_OrganizationKey";
        private const string TextureSizeIndexPref = "YourEditorWindow_TextureSizeIndex";
        private const string SelectedShaderNamePref = "YourEditorWindow_SelectedShaderName";
        
        public List<Texture2D> GeneratedTextures = new();

        public void LoadPreferences()
        {
            APIKey = EditorPrefs.GetString(ApiKeyPref, "");
            OrganizationKey = EditorPrefs.GetString(OrganizationKeyPref, "");
            TextureSizeIndex = EditorPrefs.GetInt(TextureSizeIndexPref, 2);
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
