using Dialogs.ViewModels;
using Dialogs.Views;
using MediaTracker.Views;

namespace Dialogs
{
    public class DialogsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<MessageBoxDialogView, MessageBoxDialogViewModel>();
            containerRegistry.RegisterDialog<AddContentDialogView, AddContentDialogViewModel>();
            containerRegistry.RegisterDialog<EditContentDialogView, EditContentDialogViewModel>();
            containerRegistry.RegisterDialog<ListEditorDialogView, ListEditorDialogViewModel>();
            containerRegistry.RegisterDialog<ListItemsDialogView, ListItemsDialogViewModel>();
        }
    }
}