using Newtonsoft.Json;

namespace TvMazeScraper.Unit.Tests.Helper
{
    public static class StringExtention
    {
        public static string ToJson(this object input)
        {
            return JsonConvert.SerializeObject(input);
        }
    }
}
