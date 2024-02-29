using System;
using Texturify;
using Texturify_AI.Scripts.Editor.Model;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace Texturify_AI.Scripts.Editor.Controller
{
    public class TextureGenerator
    {
        private EditorDataModel _dataModel;

        public TextureGenerator(EditorDataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public void GenerateTexture(string styleInput, Action<Texture2D, string> onTextureGenerated)
        {
            if (string.IsNullOrWhiteSpace(styleInput))
            {
                onTextureGenerated(null, "Style input is empty.");
                return;
            }

            EditorCoroutineUtility.StartCoroutineOwnerless(
                TextureUtility.GenerateTexture(styleInput, _dataModel.TextureSizeIndex, 
                    texture =>
                    {
                        if (texture != null)
                        {
                            _dataModel.GeneratedTextures.Add(texture);
                            TextureUtility.SaveTexture(texture, EditorDataModel.TexturesFolderPath, styleInput);
                            onTextureGenerated(texture, null);
                        }
                        else
                        {
                            onTextureGenerated(null, "Failed to generate texture.");
                        }
                    }, _dataModel.APIKey, _dataModel.OrganizationKey)
            );
        }
    }

}