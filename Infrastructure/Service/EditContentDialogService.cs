using Infrastructure.DTO;
using Infrastructure.Interface;

namespace Infrastructure.Service
{
    public class EditContentDialogService : IEditContentDialogService
    {
        private readonly IDialogService _dialogService;

        public EditContentDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<MediaEditModel> ShowEditDialogAsync(MediaEditModel item)
        {
            var parameters = new DialogParameters
            {
                { "item", item }
            };

            // Открываем диалог и ждём результата
            var result = await _dialogService.ShowDialogAsync("EditContentDialogView", parameters);

            // Проверяем результат
            if (result.Result == ButtonResult.OK && result.Parameters.ContainsKey("updatedItem"))
            {
                // Получаем обновлённый Item из параметров результата
                return result.Parameters.GetValue<MediaEditModel>("updatedItem");
            }

            return null;
        }


    }
}