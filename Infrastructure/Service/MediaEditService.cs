using Infrastructure.DTO;
using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.EF.Enum;
using Infrastructure.Interface;

namespace Infrastructure.Service
{
    public class MediaEditService
    {
        private readonly IService<MediaItem> _mediaItemService;
        private readonly IService<UserInteraction> _userInteractionService;
        private readonly IService<Relationship> _relationshipService;
        private readonly IService<MediaInstance> _mediaInstanceService;
        private readonly TagService _tagService;

        public MediaEditService(
            IService<MediaItem> mediaItemService,
            IService<UserInteraction> userInteractionService,
            IService<Relationship> relationshipService,
            IService<MediaInstance> mediaInstanceService,
            TagService tagService)
        {
            _mediaItemService = mediaItemService;
            _userInteractionService = userInteractionService;
            _relationshipService = relationshipService;
            _mediaInstanceService = mediaInstanceService;
            _tagService = tagService;
        }

        // Загрузка DTO для редактирования
        public async Task<MediaEditModel> GetEditModelAsync(Guid mediaItemId, Guid userId, CancellationToken ct = default)
        {
            var mediaItem = await _mediaItemService.GetItemAsync(mediaItemId);
            if (mediaItem == null) throw new KeyNotFoundException($"Entity with Id = {mediaItem.Id} not found");

            var userInteraction = (await _userInteractionService.GetItemsAsync())
                .FirstOrDefault(ui => ui.MediaInstance.MediaItemId == mediaItemId
                                    && ui.UserId == userId);

            var tags = userInteraction?.Tags.Select(t => t.Tag.Name).ToList() ?? new List<string>();

            return new MediaEditModel
            {
                Title = mediaItem.Title,
                OriginalTitle = mediaItem.OriginalTitle,
                Year = mediaItem.Year,
                Country = mediaItem.Country,
                Type = mediaItem.Type,
                DurationMinutes = mediaItem.DurationMinutes,
                SeasonsCount = mediaItem.SeasonsCount,
                EpisodesCount = mediaItem.EpisodesCount,
                OfficialRating = mediaItem.OfficialRating,
                Synopsis = mediaItem.Synopsis,

                Status = userInteraction?.Status ?? InteractionStatus.Planned,
                StartDate = userInteraction?.StartDate,
                CompletionDate = userInteraction?.CompletionDate,
                PersonalRating = userInteraction?.PersonalRating,
                UserNotes = userInteraction?.Notes,

                TagsInput = string.Join(", ", tags),

                Format = userInteraction?.MediaInstance?.Format,
                Source = userInteraction?.MediaInstance?.Source,
                AcquisitionDate = userInteraction?.MediaInstance?.AcquisitionDate,
                Location = userInteraction?.MediaInstance?.Location
            };
        }

        // Сохранение изменений из DTO
        public async Task UpdateAsync(Guid mediaItemId, Guid userId, MediaEditModel model, CancellationToken ct = default)
        {
            // 1. Обновляем MediaItem
            var mediaItem = await _mediaItemService.GetItemAsync(mediaItemId, ct);
            if (mediaItem == null) throw new KeyNotFoundException($"Entity with Id = {mediaItem.Id} not found");

            mediaItem.Title = model.Title;
            mediaItem.OriginalTitle = model.OriginalTitle;
            mediaItem.Year = model.Year;
            mediaItem.Country = model.Country;
            mediaItem.Type = model.Type;
            mediaItem.DurationMinutes = model.DurationMinutes;
            mediaItem.SeasonsCount = model.SeasonsCount;
            mediaItem.EpisodesCount = model.EpisodesCount;
            mediaItem.OfficialRating = model.OfficialRating;
            mediaItem.Synopsis = model.Synopsis;

            await _mediaItemService.UpdateItemAsync(mediaItem, ct);

            // 2. Обновляем UserInteraction
            var userInteraction = (await _userInteractionService.GetItemsAsync(ct))
                .FirstOrDefault(ui => ui.MediaInstance.MediaItemId == mediaItemId
                                && ui.UserId == userId);

            if (userInteraction == null)
            {
                userInteraction = new UserInteraction
                {
                    MediaInstance = new MediaInstance
                    {
                        MediaItemId = mediaItemId,
                        Format = model.Format ?? MediaFormat.Digital,
                        Source = model.Source ?? MediaSource.Purchase,
                        AcquisitionDate = model.AcquisitionDate,
                        Location = model.Location
                    },
                    UserId = userId,
                    Status = model.Status,
                    StartDate = model.StartDate,
                    CompletionDate = model.CompletionDate,
                    PersonalRating = model.PersonalRating,
                    Notes = model.UserNotes
                };
                await _userInteractionService.CreateItemAsync(userInteraction, ct);
            }
            else
            {
                userInteraction.Status = model.Status;
                userInteraction.StartDate = model.StartDate;
                userInteraction.CompletionDate = model.CompletionDate;
                userInteraction.PersonalRating = model.PersonalRating;
                userInteraction.Notes = model.UserNotes;

                if (userInteraction.MediaInstance != null)
                {
                    userInteraction.MediaInstance.Format = model.Format ?? userInteraction.MediaInstance.Format;
                    userInteraction.MediaInstance.Source = model.Source ?? userInteraction.MediaInstance.Source;
                    userInteraction.MediaInstance.AcquisitionDate = model.AcquisitionDate;
                    userInteraction.MediaInstance.Location = model.Location;
                }

                await _userInteractionService.UpdateItemAsync(userInteraction, ct);
            }

            // 3. Обновляем теги
            var tagNames = model.TagsInput
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            await UpdateTagsForInteractionAsync(userInteraction.Id, tagNames, ct);
        }

        private async Task UpdateTagsForInteractionAsync(Guid interactionId, List<string> tagNames, CancellationToken ct)
        {
            var interaction = await _userInteractionService.GetItemAsync(interactionId, ct);
            if (interaction == null) return;

            interaction.Tags.Clear();

            var tags = await _tagService.GetOrCreateTagsAsync(tagNames);
            foreach (var tag in tags)
            {
                interaction.Tags.Add(new UserInteractionTag
                {
                    TagId = tag.Id,
                    Tag = tag
                });
            }

            await _userInteractionService.UpdateItemAsync(interaction, ct);
        }

        public async Task DeleteAsync(Guid mediaItemId, Guid userId, bool deleteMediaItem = false, CancellationToken ct = default)
        {
            // 1. Удаляем UserInteraction
            var userInteraction = (await _userInteractionService.GetItemsAsync(ct))
                .FirstOrDefault(ui => ui.MediaInstance.MediaItemId == mediaItemId && ui.UserId == userId);

            if (userInteraction != null)
            {
                await _userInteractionService.DeleteItemAsync(userInteraction.Id, ct);
            }

            // 2. Если нужно удалить MediaItem — обрабатываем связи
            if (deleteMediaItem)
            {
                var sourceRelations = (await _relationshipService.GetItemsAsync(ct))
                    .Where(r => r.SourceMediaId == mediaItemId)
                    .ToList();
                foreach (var rel in sourceRelations)
                {
                    await _relationshipService.DeleteItemAsync(rel.Id, ct);
                }

                var targetRelations = (await _relationshipService.GetItemsAsync(ct))
                    .Where(r => r.TargetMediaId == mediaItemId)
                    .ToList();
                foreach (var rel in targetRelations)
                {
                    await _relationshipService.DeleteItemAsync(rel.Id, ct);
                }

                // Удаляем MediaItem (каскад сработает)
                await _mediaItemService.DeleteItemAsync(mediaItemId, ct);
            }
        }

        public async Task CreateAsync(Guid userId, MediaEditModel model, CancellationToken ct = default)
        {
            // 1. Создаём MediaItem
            var mediaItem = new MediaItem
            {
                Type = model.Type,
                Title = model.Title,
                OriginalTitle = model.OriginalTitle,
                Year = model.Year,
                Country = model.Country,
                DurationMinutes = model.DurationMinutes,
                SeasonsCount = model.SeasonsCount,
                EpisodesCount = model.EpisodesCount,
                OfficialRating = model.OfficialRating,
                Synopsis = model.Synopsis
            };

            await _mediaItemService.CreateItemAsync(mediaItem, ct);

            // После создания получаем реальный Id (если он генерируется БД)
            var createdMediaItem = await _mediaItemService.GetItemAsync(mediaItem.Id, ct);
            if (createdMediaItem == null)
                throw new Exception("Не удалось получить созданный MediaItem");

            // Используем реальный Id
            var mediaItemId = createdMediaItem.Id;


            // 2. Создаём MediaInstance
            var mediaInstance = new MediaInstance
            {
                MediaItemId = mediaItemId,
                Format = model.Format ?? MediaFormat.Digital,
                Source = model.Source ?? MediaSource.Purchase,
                AcquisitionDate = model.AcquisitionDate,
                Location = model.Location
            };

            await _mediaInstanceService.CreateItemAsync(mediaInstance, ct);

            // Получаем реальный Id экземпляра
            var createdInstance = await _mediaInstanceService.GetItemAsync(mediaInstance.Id, ct);
            if (createdInstance == null)
                throw new Exception("Не удалось получить созданный MediaInstance");
            var instanceId = createdInstance.Id;

            // 3. Создаём UserInteraction
            var userInteraction = new UserInteraction
            {
                MediaInstanceId = instanceId,
                UserId = userId,
                Status = model.Status,
                StartDate = model.StartDate,
                CompletionDate = model.CompletionDate,
                PersonalRating = model.PersonalRating,
                Notes = model.UserNotes
            };

            await _userInteractionService.CreateItemAsync(userInteraction, ct);


            // 4. Обрабатываем теги
            var tagNames = model.TagsInput
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            await UpdateTagsForInteractionAsync(userInteraction.Id, tagNames, ct);
        }
    }
}