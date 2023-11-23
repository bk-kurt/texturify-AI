using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Texturify
{
    public static class UnityWebRequestExtensions
    {
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest.Result>();
            asyncOp.completed += _ => tcs.TrySetResult(asyncOp.webRequest.result);
            return tcs.Task.GetAwaiter();
        }
    } 
}