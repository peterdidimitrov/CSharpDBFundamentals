namespace MusicHub;

using System;
using System.Text;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;

public class StartUp
{
    public static void Main()
    {
        MusicHubDbContext context =
            new MusicHubDbContext();

        //DbInitializer.ResetDatabase(context);

        Console.WriteLine(ExportSongsAboveDuration(context, 4));
    }
    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        StringBuilder sb = new StringBuilder();

        var albumsInfo = context.Producers
            .Include(p => p.Albums)
            .ThenInclude(a => a.Songs)
            .ThenInclude(s => s.Writer)
            .First(a => a.Id == producerId)
            .Albums.Select(a => new
            {
                a.Name,
                ReleaseDate = a.ReleaseDate.ToString(@"MM\/dd\/yyyy"),
                Producer = a.Producer.Name,
                Songs = a.Songs.Select(s => new
                {
                    s.Name,
                    s.Price,
                    WriterName = s.Writer.Name
                })
                .OrderByDescending(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToList(),
                TotalPrice = a.Price
            })
            .OrderByDescending(a => a.TotalPrice)
            .ToArray();

        foreach (var album in albumsInfo)
        {
            sb.AppendLine($"-AlbumName: {album.Name}");
            sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
            sb.AppendLine($"-ProducerName: {album.Producer}");
            sb.AppendLine($"-Songs:");
            int count = 1;
            foreach (var song in album.Songs)
            {
                sb.AppendLine($"---#{count}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Price: {song.Price:f2}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                count++;
            }
            sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        StringBuilder sb = new StringBuilder();

        var songs = context.Songs
            .Include(s => s.SongPerformers)
            .ThenInclude(sp => sp.Performer)
            .Include(s => s.Writer)
            .Include(s => s.Album)
            .ThenInclude(a => a.Producer)
            .ToList()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                s.Name,
                SongWriter = s.Writer.Name,
                Performers = s.SongPerformers
                .Select(p => new
                {
                    PerformerName = p.Performer.FirstName + " " + p.Performer.LastName
                })
                .OrderBy(p => p.PerformerName)
                .ToList(),
                AlbumProduserName = s.Album.Producer.Name,
                Duration = s.Duration
            })
            .OrderBy(s => s.Name)
            .ThenBy(s => s.SongWriter)
            .ToList();

        int count = 1;

        foreach (var song in songs)
        {
            sb.AppendLine($"-Song #{count++}");
            sb.AppendLine($"---SongName: {song.Name}");
            sb.AppendLine($"---Writer: {song.SongWriter}");


            foreach (var performer in song.Performers)
            {
                sb.AppendLine($"---Performer: {performer.PerformerName}");
            }

            sb.AppendLine($"---AlbumProducer: {song.AlbumProduserName}");
            sb.AppendLine($"---Duration: {song.Duration}");
        }

        return sb.ToString().TrimEnd();
    }
}
