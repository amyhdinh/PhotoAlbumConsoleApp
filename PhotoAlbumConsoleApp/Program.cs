namespace PhotoAlbumConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            var photoAlbumAccessor = new PhotoAlbumAccessor(new HttpClient());
            while (true)
            {
                Console.Write(">");
                var input = Console.ReadLine();
                var albumNumber = PhotoAlbumAccessor.ValidateInput(input);
                if (albumNumber != null)
                {
                    await photoAlbumAccessor.GetAlbumIdsAndTitlesAsync(albumNumber.Value);
                }
                Console.WriteLine();
            }
        }
    }
}