using Texturify_AI.Scripts.Editor.Model;
using Texturify_AI.Scripts.Editor.View;
using Texturify_AI.Scripts.Services;
using UnityEditor;
using UnityEngine;

namespace Texturify_AI.Scripts.Editor.Controller
{
 public class TexturifyAI : EditorWindow
{
    private readonly EditorDataModel _dataModel = new();
    private TexturifyUIDrawer _uiDrawer;
    private TextureGenerator _textureGenerator;
    private ShaderOptions _shaderOptions;
    
    public string styleInput = "";
    public bool isGeneratingTexture;

    [MenuItem("Tools/Texturify AI")]
    public static void ShowWindow()
    {
        GetWindow<TexturifyAI>();
    }

    void OnEnable()
    {
        _dataModel.LoadPreferences();
        _uiDrawer = new TexturifyUIDrawer(_dataModel);
        _textureGenerator = new TextureGenerator(_dataModel);
        _shaderOptions = new ShaderOptions();
    }

    void OnGUI()
    {
        if (_uiDrawer != null)
        {
            _uiDrawer.DrawMainLayout(this);
        }
        Repaint();
    }
    

    public string[] GetTextureSizeOptions()
    {
        int[] textureSizeOptions = _dataModel.TextureSizeOptions;
        string[] options = new string[textureSizeOptions.Length];
        for (int i = 0; i < textureSizeOptions.Length; i++)
        {
            options[i] = textureSizeOptions[i].ToString();
        }

        return options;
    }

    public void StartTextureGeneration()
    {
        if (!isGeneratingTexture)
        {
            isGeneratingTexture = true;
            _textureGenerator.GenerateTexture(styleInput, OnTextureGenerated);
        }
    }
    

    private void OnTextureGenerated(Texture2D texture,string message)
    {
        isGeneratingTexture = false; 
    }
}   
}
