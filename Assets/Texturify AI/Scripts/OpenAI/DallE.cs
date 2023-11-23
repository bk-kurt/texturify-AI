using System.Threading.Tasks;
using Texturify;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenAI
{
    public class DallE
    {
        private OpenAIApi openai;

        public DallE(string apiKey, string organizationKey)
        {
            // initialize the OpenAIApi instance with the provided API key and organization key
            openai = new OpenAIApi(apiKey, organizationKey);
        }

        public async Task<Texture2D> GenerateTextureFromPrompt(string prompt, int width, int height)
        {
            var response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = prompt,
                Size = ImageSize.Size256
            });

            if (response.Data != null && response.Data.Count > 0)
            {
                using (var request = new UnityWebRequest(response.Data[0].Url))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = new Texture2D(width, height);
                        texture.LoadImage(request.downloadHandler.data);
                        return texture;
                    }
                }
            }

            Debug.LogWarning("No image was created from this prompt.");
            return null;
        }
    }
}