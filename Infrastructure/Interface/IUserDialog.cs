namespace Infrastructure.Interface
{
    public interface IUserDialog
    {
        bool ConfirmWarning(string Warning, string Caption);

        void ShowInfo(string Information, string Caption);

        void ShowError(string Error, string Caption);
    }
}