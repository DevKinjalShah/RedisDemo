using RedisDemo.Pages;

namespace RedisDemo.Repository.Interface
{
    public interface ICountryService
    {
        public Task<(CountryPopulationArray[] countries, string loadLocation, string isCachedData)>
            GetRedisCountryPopulationAsync(DateTime startDate);
    }
}
