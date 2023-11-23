using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace Texturify
{ 
    public class CustomNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            var result = Regex.Replace(name, "([A-Z])", m => (m.Index > 0 ? "_" : "") + m.Value[0].ToString().ToLowerInvariant());
            return result;
        }
    }
}
