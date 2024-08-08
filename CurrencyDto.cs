namespace ScheduleJob
{
    public class CurrencyDto
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string CurrencyCountry { get; set; } = string.Empty;

        public DateTime ScheduleTime { get; set; } = DateTime.UtcNow;
    }
}
