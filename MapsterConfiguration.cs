using Mapster;

namespace ScheduleJob
{
    public class MapsterConfiguration
    {
       
        public static void Configuration()
        {
            TypeAdapterConfig<Currency, CurrencyDto>.NewConfig()
                .Map(dest => dest.CurrencyCountry, source => source.Country);
        }     
    }
}
