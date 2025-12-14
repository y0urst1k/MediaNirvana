using Dialogs.ViewModels;
using Dialogs.Views;

namespace Dialogs
{
    public class DialogsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<MessageBoxDialog, MessageBoxDialogViewModel>();
        }
    }
}