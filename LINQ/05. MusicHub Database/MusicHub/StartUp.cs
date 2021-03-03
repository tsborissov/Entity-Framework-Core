namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here

            var result = ExportAlbumsInfo(context, 9);

            Console.WriteLine(result);
            
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumInfo = context.Producers
                .Include(x => x.Albums)
                .ThenInclude(x => x.Songs)
                .FirstOrDefault(x => x.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new 
                    {
                        SongName = s.Name,
                        SongPrice = s.Price,
                        WriterName = s.Writer.Name
                    })
                    .ToList(),
                    AlbumPrice = a.Price
                })
                .ToList();



            ;

            
            
            
            
            //var albumInfo = context.Producers
            //    .FirstOrDefault(x => x.Id == producerId)
            //    .Albums
            //    .Select(a => new
            //    {
            //        AlbumName = a.Name,
            //        ReleaseDate = a.ReleaseDate,
            //        ProducerName = a.Producer.Name,
            //        Songs = a.Songs.Select(s => new
            //        {
            //            SongName = s.Name,
            //            SongPrice = s.Price,
            //            WriterName = s.Writer.Name
            //        })
            //     .OrderByDescending(x => x.SongName)
            //     .ThenBy(x => x.WriterName)
            //     .ToList(),
            //    AlbumPrice = a.Price
            //    })
            //    .OrderByDescending(x => x.AlbumPrice)
            //    .ToList();


            var sb = new StringBuilder();

            //foreach (var album in albums)
            //{
            //    sb.AppendLine($"-AlbumName: {album.AlbumName}");
            //    sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo)}");
            //    sb.AppendLine($"-ProducerName: {album.ProducerName}");
            //    sb.AppendLine($"-Songs:");

            //    var rowCounter = 1;

            //    foreach (var song in album.Songs)
            //    {
            //        sb.AppendLine($"---#{rowCounter++}");
            //        sb.AppendLine($"---SongName: {song.SongName}");
            //        sb.AppendLine($"---Price: {song.SongPrice:F2}");
            //        sb.AppendLine($"---Writer: {song.WriterName}");
            //    }

            //    sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:F2}");
            //}

            return sb.ToString().TrimEnd();
        }



        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
