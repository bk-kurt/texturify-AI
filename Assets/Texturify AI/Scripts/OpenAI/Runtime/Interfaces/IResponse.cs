namespace Texturify
{
    public interface IResponse
    {
        ApiError Error { get; set; }
        public string Warning { get; set; }
    }
}
