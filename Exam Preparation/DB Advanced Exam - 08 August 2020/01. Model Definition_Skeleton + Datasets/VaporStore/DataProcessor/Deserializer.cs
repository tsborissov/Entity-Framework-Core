namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var games = JsonConvert.DeserializeObject<IEnumerable<GameImportModel>>(jsonString);

			foreach (var jsonGame in games)
            {
				if (!IsValid(jsonGame) ||
					!jsonGame.Tags.Any())
				{
					sb.AppendLine("Invalid Data");
					continue;
				}

				var currentDeveloper = context.Developers.FirstOrDefault(x => x.Name == jsonGame.Developer)
					?? new Developer { Name = jsonGame.Developer };

				var currentGenre = context.Genres.FirstOrDefault(x => x.Name == jsonGame.Genre)
					?? new Genre { Name = jsonGame.Genre };

				var currentGame = new Game 
				{
					Name = jsonGame.Name,
					Price = jsonGame.Price,
					ReleaseDate = jsonGame.ReleaseDate.Value,
					Developer = currentDeveloper,
					Genre = currentGenre,
				};

                foreach (var jsonTag in jsonGame.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == jsonTag)
						?? new Tag { Name = jsonTag };

					currentGame.GameTags.Add(new GameTag { Tag = tag });
                }

				context.Games.Add(currentGame);
				context.SaveChanges();

				sb.AppendLine($"Added {currentGame.Name} ({currentGame.Genre.Name}) with {currentGame.GameTags.Count()} tags");
			}

			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var users = JsonConvert.DeserializeObject<IEnumerable<UserJsonImportModel>>(jsonString);

			var validUsers = new List<User>();

            foreach (var jsonUser in users)
            {

				if (!IsValid(jsonUser) ||
					!jsonUser.Cards.All(IsValid))
				{
					sb.AppendLine("Invalid Data");
					continue;
				}

				var user = new User
				{
					FullName = jsonUser.FullName,
					Username = jsonUser.Username,
					Email = jsonUser.Email,
					Age = jsonUser.Age,
					Cards = jsonUser.Cards.Select(x => new Card
					{
						Number = x.Number,
						Cvc = x.Cvc,
						Type = x.Type.Value
					})
					.ToArray()
				};

				validUsers.Add(user);

				sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
			}

			context.Users.AddRange(validUsers);
			context.SaveChanges();


			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var xmlRoot = new XmlRootAttribute("Purchases");
			var serializer = new XmlSerializer(typeof(PurchaseXmlImportModel[]), xmlRoot);
			var inputXmlReader = new StringReader(xmlString);
			var purchases = (PurchaseXmlImportModel[])serializer.Deserialize(inputXmlReader);

			var sb = new StringBuilder();

			var validPurchases = new List<Purchase>();

            foreach (var xmlPurchase in purchases)
            {
				if (!IsValid(xmlPurchase))
                {
					sb.AppendLine("Invalid Data");
					continue;
				}

				DateTime
					.TryParseExact(xmlPurchase.Date,
					"dd/MM/yyyy HH:mm",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out DateTime purchaseDate);

				var currentPurchase = new Purchase 
				{
					Type = xmlPurchase.Type.Value,
					ProductKey = xmlPurchase.Key,
					Date = purchaseDate,
					Card = context.Cards.FirstOrDefault(x => x.Number == xmlPurchase.Card),
					Game = context.Games.FirstOrDefault(x => x.Name == xmlPurchase.Title)
				};

				validPurchases.Add(currentPurchase);

				var userName = currentPurchase.Card.User.Username;

				sb.AppendLine($"Imported {currentPurchase.Game.Name} for {userName}");
			}

			context.Purchases.AddRange(validPurchases);
			context.SaveChanges();


			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}