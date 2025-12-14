using Infrastructure.DTO;
using Infrastructure.EF.Enum;

namespace Dialogs.ViewModels
{
    public class ListEditorDialogViewModel : BindableBase, IDialogAware
    {

        public PersonalListEditModel List { get; private set; }

        public List<LabelType> AvailableTypes { get; } = new List<LabelType>
        {
            LabelType.List, LabelType.Collection, LabelType.Priority
        };

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DialogCloseListener RequestClose { get; set; }

        public ListEditorDialogViewModel()
        {
            List = new PersonalListEditModel();

            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(List.Name);

        private void OnSave()
        {
            var parameters = new DialogParameters
            {
                { "result", List }
            };

            var result = new DialogResult(ButtonResult.OK)
            {
                Parameters = parameters
            };

            // В Prism 9: вызываем RequestClose как событие
            RequestClose.Invoke(result);
        }
        private void OnCancel()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            if (parameters.ContainsKey("existingList"))
            {
                List = parameters.GetValue<PersonalListEditModel>("existingList") ?? new PersonalListEditModel();
            }
            // Пересоздаём команды, чтобы обновить CanSave
            SaveCommand.RaiseCanExecuteChanged();
        }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}