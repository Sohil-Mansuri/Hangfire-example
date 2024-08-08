namespace ScheduleJob
{
    public interface ICurrencyRepo
    {
        Task<List<Currency>> GetAllAsync();
        Task<Currency?> GetByIDAsync(int id);
        Task AddAsync(Currency currency);
        Task<Currency?> DeleteAsync(int id);
    }
}
