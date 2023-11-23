using System.IO;
using UnityEditor;
using UnityEngine;

namespace Texturify
{
    public static class MaterialUtility
    {
        public static void CreateMaterialWithTexture(Texture2D texture, string shaderName, string materialsFolderPath, string styleInput)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {shaderName}");
                return;
            }

            if (!Directory.Exists(materialsFolderPath))
            {
                Directory.CreateDirectory(materialsFolderPath);
            }

            Material material = new Material(shader)
            {
                mainTexture = texture
            };

            string uniqueIdentifier = System.Guid.NewGuid().ToString();
            styleInput = string.IsNullOrEmpty(styleInput) ? "random" : styleInput;
            string materialFileName = $"GeneratedMaterial_{styleInput.Replace(" ", "_")}_{uniqueIdentifier}.mat";
            string materialFullPath = Path.Combine(materialsFolderPath, materialFileName);

            AssetDatabase.CreateAsset(material, materialFullPath);
            AssetDatabase.Refresh();
            Debug.Log($"Material saved as asset: {materialFullPath}");
        }
    }
}
