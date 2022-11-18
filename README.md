# PhotoAlbumConsoleApp Prompt

Create a console application that displays photo ids and titles in an album. The photos are available in this online web
service: https://jsonplaceholder.typicode.com/photos
Hint: Photos are filtered with a query string. This will return photos within albumId=3
https://jsonplaceholder.typicode.com/photos?albumId=3

Example:
```
>photo-album 2
[53] soluta et harum aliquid officiis ab omnis consequatur
[54] ut ex quibusdam dolore mollitia
...
>photo-album 3
[101] incidunt alias vel enim
[102] eaque iste corporis tempora vero distinctio consequuntur nisi nesciunt
```

## How to run - Powershell

1. Change to PhotoAlbumConsoleApp directory
> cd PhotoAlbumConsoleApp\PhotoAlbumConsoleApp
2. Build project
> dotnet build PhotoAlbumConsoleApp.csproj
3. Run Project
> dotnet run --project PhotoAlbumConsoleApp.csproj
