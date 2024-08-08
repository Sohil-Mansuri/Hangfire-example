
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ScheduleJob
{
    public class CurrencyRepo : ICurrencyRepo
    {
        private readonly DataContext _dbContext;

        public CurrencyRepo(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AutomaticRetry(Attempts = 1)]
        public async Task AddAsync(Currency currency)
        {
            await _dbContext.Currency.AddAsync(currency);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Currency?> DeleteAsync(int id)
        {
            var currency = await _dbContext.Currency.FindAsync(id);
            if (currency == null) return null;
            _dbContext.Currency.Remove(currency);
            await _dbContext.SaveChangesAsync();
            return currency;
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            return await _dbContext.Currency.ToListAsync();
        }

        public async Task<Currency?> GetByIDAsync(int id)
        {
            var currency = await _dbContext.Currency.FindAsync(id);
            if (currency is null) return null;
            return currency;
        }
    }
}
