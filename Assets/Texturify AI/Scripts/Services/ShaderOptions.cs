
// deprecating this for v1.0 of texturify AI.  you may use string-o-matic for next versions.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Texturify_AI.Scripts.Services
{
    public class ShaderOptions
    {
        private List<string> _shaderList;

        public ShaderOptions()
        {
            InitializeShaderOptions();
        }

        private void InitializeShaderOptions()
        {
            _shaderList = new List<string>
            {
                "Standard", "Mobile/Diffuse", "Sprites/Default"
                
                // use manual inputting via inspector as for this version.
            };

            string[] guids = AssetDatabase.FindAssets("t:Shader");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader != null)
                {
                    _shaderList.Add(shader.name);
                }
            }

            if (_shaderList.Count == 0)
            {
                _shaderList.Add("No Shaders Found");
            }
        }

        public IEnumerable<string> GetShaderOptions()
        {
            return _shaderList;
        }
    }
}