using System.Collections.ObjectModel;
using Infrastructure.DTO;
using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.EF.Enum;
using Infrastructure.Interface;

namespace Infrastructure.Service
{
    public class PersonalListEditModelService
    {
        private readonly IService<UserLabel> _userLabelService;
        private readonly IService<UserLabelLink> _linkService;
        private readonly MediaEditService _mediaEditService; // Используем для получения DTO медиа

        public PersonalListEditModelService(
            IService<UserLabel> userLabelService,
            IService<UserLabelLink> linkService,
            MediaEditService mediaEditService)
        {
            _userLabelService = userLabelService;
            _linkService = linkService;
            _mediaEditService = mediaEditService;
        }

        public async Task<IEnumerable<PersonalListEditModel>> GetUserListsAsync(Guid userId, CancellationToken ct = default)
        {
            // Здесь UserLabelService уже подгрузит Links!
            var allLabels = await _userLabelService.GetItemsAsync(ct, l => l.UserId == userId);

            var result = new List<PersonalListEditModel>();

            foreach (var label in allLabels)
            {
                var items = new ObservableCollection<MediaEditModel>();

                // Используем уже загруженные Links, не делая лишних запросов к БД
                foreach (var link in label.Links)
                {
                    if (link.TargetType == LinkTargetType.MediaItem)
                    {
                        try
                        {
                            // Получаем детали медиа по ID
                            var mediaModel = await _mediaEditService.GetEditModelAsync(link.TargetId, userId, ct);
                            items.Add(mediaModel);
                        }
                        catch (KeyNotFoundException) { /* ignored */ }
                    }
                }

                result.Add(new PersonalListEditModel
                {
                    Id = label.Id,
                    Name = label.Name,
                    Description = label.Description,
                    Type = label.Type,
                    Items = items
                });
            }

            return result;
        }

        public async Task CreateListAsync(Guid userId, PersonalListEditModel model, CancellationToken ct = default)
        {
            var entity = new UserLabel
            {
                UserId = userId,
                Name = model.Name,
                Description = model.Description,
                Type = model.Type
            };

            await _userLabelService.CreateItemAsync(entity, ct);

            // Получаем созданный ID
            var created = (await _userLabelService.GetItemsAsync(ct))
                          .FirstOrDefault(l => l.Name == model.Name && l.UserId == userId); // Лучше, если Create возвращает ID

            if (created != null)
            {
                model.Id = created.Id; // Обновляем ID в DTO
                await UpdateLinksAsync(created.Id, model.Items.Select(i => i.Id).ToList(), ct);
            }
        }

        public async Task UpdateListAsync(PersonalListEditModel model, CancellationToken ct = default)
        {
            var entity = await _userLabelService.GetItemAsync(model.Id, ct);
            if (entity == null) throw new KeyNotFoundException("List not found");

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Type = model.Type;

            await _userLabelService.UpdateItemAsync(entity, ct);

            // Обновляем состав списка
            await UpdateLinksAsync(entity.Id, model.Items.Select(i => i.Id).ToList(), ct);
        }

        public async Task DeleteListAsync(Guid listId, CancellationToken ct = default)
        {
            // Удаляем связи
            var links = (await _linkService.GetItemsAsync(ct)).Where(l => l.UserLabelId == listId).ToList();
            foreach (var link in links)
            {
                await _linkService.DeleteItemAsync(link.Id, ct);
            }

            // Удаляем сам список
            await _userLabelService.DeleteItemAsync(listId, ct);
        }

        // Хелпер для синхронизации элементов списка
        private async Task UpdateLinksAsync(Guid listId, List<Guid> newItemIds, CancellationToken ct)
        {
            var existingLinks = (await _linkService.GetItemsAsync(ct))
                                .Where(l => l.UserLabelId == listId)
                                .ToList();

            // 1. Удаляем лишние
            foreach (var link in existingLinks)
            {
                if (!newItemIds.Contains(link.TargetId))
                {
                    await _linkService.DeleteItemAsync(link.Id, ct);
                }
            }

            // 2. Добавляем новые
            var existingTargetIds = existingLinks.Select(l => l.TargetId).ToList();
            foreach (var newId in newItemIds)
            {
                if (!existingTargetIds.Contains(newId))
                {
                    var newLink = new UserLabelLink
                    {
                        UserLabelId = listId,
                        TargetType = LinkTargetType.MediaItem, // Привязываемся к MediaItem
                        TargetId = newId
                    };
                    await _linkService.CreateItemAsync(newLink, ct);
                }
            }
        }
    }
}
