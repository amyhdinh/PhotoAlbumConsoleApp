using Newtonsoft.Json;

namespace PhotoAlbumConsoleApp
{
    public class PhotoAlbumAccessor
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://jsonplaceholder.typicode.com/photos";
        private const string QueryString = "?albumId=";

        public PhotoAlbumAccessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public static int? ValidateInput(string? input)
        {
            var inputParams = input?.Split(' ');
            if (inputParams?.Length == 2 && inputParams[0].Equals("photo-album")
                    && int.TryParse(inputParams[1], out int albumNumber))
            {
                return albumNumber;
            }
            Console.WriteLine("Input is invalid. Please format input as: photo-album {integer number}, e.g. photo-album 100");
            return null;
        }

        public async Task GetAlbumIdsAndTitlesAsync(int albumNumber)
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), BaseUrl + QueryString + albumNumber);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                try
                {
                    var photoAlbumList = JsonConvert.DeserializeObject<List<PhotoAlbum>>(jsonResult);
                    if (photoAlbumList != null && photoAlbumList.Count > 0)
                    {
                        PrintValues(photoAlbumList);
                    }
                    else
                    {
                        Console.WriteLine($"No information found with given album number: {albumNumber}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred when deserializing response: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"There is an issue getting a response. Code: {response.StatusCode}");
            }
        }

        private static void PrintValues(List<PhotoAlbum> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine($"[{item.Id}] {item.Title}");
            }
        }
    }
}
