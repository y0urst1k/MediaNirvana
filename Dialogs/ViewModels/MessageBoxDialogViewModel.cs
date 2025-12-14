namespace Dialogs.ViewModels
{
    public class MessageBoxDialogViewModel : BindableBase, IDialogAware
    {
        public string Message { get; set; }
        public string Title { get; set; }


        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DialogCloseListener RequestClose { get; private set; }

        public MessageBoxDialogViewModel(DialogCloseListener requestClose)
        {
            RequestClose = requestClose;

            OkCommand = new DelegateCommand(() =>
            {
                InvokeRequestClose(ButtonResult.OK);
            });

            CancelCommand = new DelegateCommand(() =>
            {
                InvokeRequestClose(ButtonResult.Cancel);
            });
        }

        private void InvokeRequestClose(ButtonResult result)
        {
            if (!Delegate.Equals(RequestClose, null))
            {
                RequestClose.Invoke(new DialogResult(result));
            }
        }

        public bool CanCloseDialog() => true;


        public void OnDialogClosed() { }


        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title") ?? "Dialog";
        }
    }
}
