namespace MusicHub
{
    using System;
    using System.Text;
    using System.Xml.Linq;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();
           var albums= context.Albums
                .Where(x => x.ProducerId == producerId)
                .Select(x => new
                {
                    AlbmName = x.Name,
                    ReleaseDate=x.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = x.Producer.Name,
                    Songs = x.Songs
                        .Select(y => new 
                        { SongName = y.Name,
                            y.Price,
                            WriterName = y.Writer.Name 
                        })
                        .OrderByDescending(y => y.SongName)
                        .ThenBy(y => y.WriterName)
                        .ToList(),
                    x.Price
                })
                .ToList()
                .OrderByDescending(x => x.Price);
            
            foreach (var album in albums) 
            {
                sb.AppendLine($"-AlbumName: {album.AlbmName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                foreach (var song in album.Songs.Select((value, index) => (value, index)))
                {
                    sb.AppendLine($"---#{song.index + 1}");
                    sb.AppendLine($"---SongName: {song.value.SongName}");
                    sb.AppendLine($"---Price: {song.value.Price:f2}");
                    sb.AppendLine($"---Writer: {song.value.WriterName}" );
                }
                sb.AppendLine($"-AlbumPrice: {album.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(duration);
            var songs=context.Songs
                .Where(y => y.Duration > timeSpan)
                .Select(x=>new
                {
                    SongName=x.Name,
                    Performers=x.SongPerformers.Select(y=>$"{y.Performer.FirstName} {y.Performer.LastName}"),
                    WriterName=x.Writer.Name,
                    AlbumProducerName=x.Album.Producer.Name,
                    x.Duration
                }).OrderBy(x=>x.SongName)
                .ThenBy(x=>x.WriterName)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var song in songs.Select((value, index) => (value, index)))
            {
                sb.AppendLine($"-Song #{song.index+1}");
                sb.AppendLine($"---SongName: {song.value.SongName}");
                sb.AppendLine($"---Writer: {song.value.WriterName}");
                if (song.value.Performers.Any())
                {
                    foreach (var item in song.value.Performers.OrderBy(x=>x))
                    {
                        sb.AppendLine($"---Performer: {item}");
                    }
                    
                }
                
                sb.AppendLine($"---AlbumProducer: {song.value.AlbumProducerName}");
                sb.AppendLine($"---Duration: {song.value.Duration.ToString("c")}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
