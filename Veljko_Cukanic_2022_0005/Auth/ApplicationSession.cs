namespace AutoService.Web.Auth
{
    public class ApplicationSession
    {
        public string Id { get; } = Guid.NewGuid().ToString("N");
    }
}

