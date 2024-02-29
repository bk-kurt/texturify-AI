using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using OpenAI;


namespace Texturify
{
    public static class TextureUtility
{
    public static IEnumerator GenerateTexture(string styleInput, int textureSize, 
        System.Action<Texture2D> onGenerated, string apiKey, string organizationKey)
    {
        if (string.IsNullOrEmpty(styleInput))
        {
            Debug.LogWarning("No style was entered, generating a texture with a random style...");
        }   

        DallE dalle = new DallE(apiKey, organizationKey);

        string capsulatedPrompt = "Generate a texture-like image with this style: " + styleInput;
        var textureTask = dalle.GenerateTextureFromPrompt(capsulatedPrompt, textureSize, textureSize);

        while (!textureTask.IsCompleted)
        {
            yield return null;
        }

        if (textureTask.Result != null)
        {
            onGenerated?.Invoke(textureTask.Result);
        }
        else
        {
            Debug.LogError("Texture generation failed.");
        }
    }

    public static void SaveTexture(Texture2D texture, string texturesFolderPath, string styleInput)
    {
        if (!Directory.Exists(texturesFolderPath))
        {
            Directory.CreateDirectory(texturesFolderPath);
        }

        string uniqueIdentifier = System.Guid.NewGuid().ToString();
        styleInput = string.IsNullOrEmpty(styleInput) ? "random" : styleInput;

        string textureFileName = $"GeneratedTexture_{styleInput.Replace(" ", "_")}_{uniqueIdentifier}.png";
        string textureFullPath = Path.Combine(texturesFolderPath, textureFileName);

        byte[] textureData = texture.EncodeToPNG();
        if (textureData != null)
        {
            File.WriteAllBytes(textureFullPath, textureData);
            AssetDatabase.ImportAsset(textureFullPath);
            AssetDatabase.Refresh();
            Debug.Log("Texture saved as asset: " + textureFullPath);
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }
    }
    
    
    public static void ApplyTextureToSelectedObjects(Texture2D texture)
    {
        // Get the currently selected objects in the Hierarchy window
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            foreach (GameObject selectedObject in selectedObjects)
            {
                Renderer renderer = selectedObject.GetComponent<Renderer>();

                if (renderer != null)
                {
                    // Create a new material for each selected object
                    Material material = new Material(Shader.Find("Standard"));
                    material.mainTexture = texture;

                    // Assign the material to the object's Renderer component
                    renderer.sharedMaterial = material;
                }
                else
                {
                    Debug.LogWarning(
                        "Selected object '" + selectedObject.name + "' does not have a Renderer component.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects selected in the Hierarchy window.");
        }
    }

    public static void LoadTexturesFromFolder(string folderPath,List<Texture2D> loadedTextures)
    {
        loadedTextures.Clear();
        
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
        
    public static void DeleteTextureFile(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        AssetDatabase.DeleteAsset(path);
        Debug.Log("Deleted texture asset: " + path);
    }
}
}

