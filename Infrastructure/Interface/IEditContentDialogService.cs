using Infrastructure.DTO;

namespace Infrastructure.Interface
{
    public interface IEditContentDialogService
    {
        Task<MediaEditModel> ShowEditDialogAsync(MediaEditModel item);


    }
}