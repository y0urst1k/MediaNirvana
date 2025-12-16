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
            containerRegistry.RegisterDialog<AddContentDialogView, AddContentDialogViewModel>("AddContentDialog");
            containerRegistry.RegisterDialog<EditContentDialogView, EditContentDialogViewModel>("EditContentDialogView");
            containerRegistry.RegisterDialog<ListEditorDialogView, ListEditorDialogViewModel>("ListEditorView");
            containerRegistry.RegisterDialog<ListItemsDialogView, ListItemsDialogViewModel>("ListItemsView");
        }
    }
}