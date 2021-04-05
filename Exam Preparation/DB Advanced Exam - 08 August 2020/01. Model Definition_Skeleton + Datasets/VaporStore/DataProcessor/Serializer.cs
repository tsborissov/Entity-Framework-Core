namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var data = context.Genres
				.ToArray()
				.Where(x => genreNames.Contains(x.Name))
				.Select(x => new
				{
					Id = x.Id,
					Genre = x.Name,
					Games = x.Games
						.Where(x => x.Purchases.Count > 0)
						.Select(g => new
						{
							Id = g.Id,
							Title = g.Name,
							Developer = g.Developer.Name,
							Tags = string.Join(", ", g.GameTags.Select(gt => gt.Tag.Name)),
							Players = g.Purchases.Count
						})
						.OrderByDescending(x => x.Players)
						.ThenBy(x => x.Id),
					TotalPlayers = x.Games.Sum(g => g.Purchases.Count())
				})
				.OrderByDescending(x => x.TotalPlayers)
				.ThenBy(x => x.Id);

			
			return JsonConvert.SerializeObject(data, Formatting.Indented);
		}


		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			var data = context.Users
				.ToArray()
				.Where(x => x.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
				.Select(x => new UserXmlExportModel
				{
					Username = x.Username,
					Purchases = x.Cards
						.SelectMany(c => c.Purchases)
						.Where(p => p.Type.ToString() == storeType)
						.Select(p => new PurchaseXmlExportModel 
						{ 
							Card = p.Card.Number,
							Cvc = p.Card.Cvc,
							Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
							Game = new GameXmlExportModel 
							{
								GameName = p.Game.Name,
								Genre = p.Game.Genre.Name,
								Price = p.Game.Price
							}
						})
						.OrderBy(x => x.Date)
						.ToArray(),
					TotalSpent = x.Cards
						.Sum(c => c.Purchases
						.Where(p => p.Type.ToString() == storeType)
						.Sum(p => p.Game.Price))
				})
				.OrderByDescending(x => x.TotalSpent)
				.ThenBy(x => x.Username)
				.ToArray();


			var xmlRootAttribute = new XmlRootAttribute("Users");
			var serializer = new XmlSerializer(typeof(UserXmlExportModel[]), xmlRootAttribute);
			var textWriter = new StringWriter();
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			serializer.Serialize(textWriter, data, ns);

			return textWriter.ToString();
		}
	}
}