namespace ResRoomApi.Services
{
    public interface IReservationService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime start, DateTime end);
    }
}