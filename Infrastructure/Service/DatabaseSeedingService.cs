using Infrastructure.EF.Context;
using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.EF.Enum;

namespace Infrastructure.Service
{
    public class DatabaseSeedingService
    {
        private readonly MediaNirvanaDbContext _db;

        public DatabaseSeedingService(MediaNirvanaDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            await _db.Database.EnsureCreatedAsync(ct);

            // 1. Проверяем, есть ли пользователи. Если да — считаем базу заполненной.
            if (_db.Users.Any()) return;

            // === ПОЛЬЗОВАТЕЛИ ===
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "User",
                Password = "123", // В реальности хешируем
                RegistrationDate = DateTimeOffset.Now
            };
            await _db.Users.AddAsync(user, ct);

            // === ЖАНРЫ ===
            var genreSciFi = new Genre { Id = Guid.NewGuid(), Name = "Sci-Fi" };
            var genreDrama = new Genre { Id = Guid.NewGuid(), Name = "Drama" };
            var genreFantasy = new Genre { Id = Guid.NewGuid(), Name = "Fantasy" };

            await _db.Genres.AddRangeAsync(new[] { genreSciFi, genreDrama, genreFantasy }, ct);

            // === ТЕГИ ===
            var tagFav = new Tag { Id = Guid.NewGuid(), Name = "Favorite" };
            var tagRewatch = new Tag { Id = Guid.NewGuid(), Name = "Rewatch" };
            var tagToBuy = new Tag { Id = Guid.NewGuid(), Name = "To Buy" };

            await _db.Tags.AddRangeAsync(new[] { tagFav, tagRewatch, tagToBuy }, ct);

            // === МЕДИА: ФИЛЬМ (Inception) ===
            var movieInception = new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Inception",
                OriginalTitle = "Inception",
                Type = MediaType.Film,
                Year = 2010,
                Country = "USA",
                DurationMinutes = 148,
                OfficialRating = "PG-13",
                Synopsis = "A thief who steals corporate secrets through the use of dream-sharing technology..."
            };

            // Связь с жанром
            movieInception.MediaItemGenres.Add(new MediaItemGenre { GenreId = genreSciFi.Id });

            await _db.MediaItems.AddAsync(movieInception, ct);

            // Экземпляр (Digital)
            var instanceInception = new MediaInstance
            {
                Id = Guid.NewGuid(),
                MediaItemId = movieInception.Id,
                Format = MediaFormat.Digital,
                Source = MediaSource.Rip,
                AcquisitionDate = DateTimeOffset.Now.AddYears(-2),
                Location = "HDD/Movies"
            };
            await _db.MediaInstances.AddAsync(instanceInception, ct);

            // Взаимодействие (Watched)
            var interactionInception = new UserInteraction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MediaInstanceId = instanceInception.Id,
                Status = InteractionStatus.Completed,
                PersonalRating = 9.5m,
                Notes = "Amazing visual effects!",
                StartDate = DateTimeOffset.Now.AddYears(-2),
                CompletionDate = DateTimeOffset.Now.AddYears(-2).AddHours(3)
            };
            // Тег
            interactionInception.Tags.Add(new UserInteractionTag { TagId = tagFav.Id });
            await _db.UserInteractions.AddAsync(interactionInception, ct);


            // === МЕДИА: КНИГА (The Hobbit) ===
            var bookHobbit = new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "The Hobbit",
                Type = MediaType.Book,
                Year = 1937,
                Country = "UK",
                OfficialRating = "G",
                Synopsis = "Bilbo Baggins lives a quiet life until Gandalf arrives..."
            };
            bookHobbit.MediaItemGenres.Add(new MediaItemGenre { GenreId = genreFantasy.Id });
            await _db.MediaItems.AddAsync(bookHobbit, ct);

            var instanceHobbit = new MediaInstance
            {
                Id = Guid.NewGuid(),
                MediaItemId = bookHobbit.Id,
                Format = MediaFormat.Physical,
                Source = MediaSource.Purchase,
                AcquisitionDate = DateTimeOffset.Now.AddYears(-5),
                Location = "Shelf 1"
            };
            await _db.MediaInstances.AddAsync(instanceHobbit, ct);

            var interactionHobbit = new UserInteraction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                MediaInstanceId = instanceHobbit.Id,
                Status = InteractionStatus.Reading,
                PersonalRating = 8.0m,
                StartDate = DateTimeOffset.Now.AddDays(-5)
            };
            await _db.UserInteractions.AddAsync(interactionHobbit, ct);


            // === СПИСКИ ПОЛЬЗОВАТЕЛЯ ===
            var userList = new UserLabel
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = LabelType.List,
                Name = "Must Read / Watch",
                Description = "Classics I haven't finished yet"
            };

            // Добавляем книгу в список
            userList.Links.Add(new UserLabelLink
            {
                TargetType = LinkTargetType.MediaItem,
                TargetId = bookHobbit.Id
            });

            await _db.UserLabels.AddAsync(userList, ct);

            // === СОХРАНЕНИЕ ===
            await _db.SaveChangesAsync(ct);
        }
    }
}