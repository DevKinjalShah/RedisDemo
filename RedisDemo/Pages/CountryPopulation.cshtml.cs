using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System;
using RedisDemo.Utility;
using static Pipelines.Sockets.Unofficial.SocketConnection;

namespace RedisDemo.Pages
{
    public interface ICountryService
    {
        public Task<(CountryPopulationArray[] countries, string loadLocation, string isCachedData)>
            GetRedisCountryPopulationAsync(DateTime startDate);
    }

    public class CountryService : ICountryService
    {
        private static readonly string[] Country = new[]
        {
            "India", "Pakistan", "USA", "UK", "Canada", "Sweden"
        };
        private readonly IDistributedCache _cache;
        public string loadLocation { get; set; } = "";
        public string isCachedData { get; set; } = "";
        public CountryPopulationArray[] countries { get; set; }
        public CountryService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public static CountryPopulationArray[] GetCountryPopulationAsync(DateTime startDate)
        {
           // Task.Delay(3000);
            CountryPopulationArray[]arrays = new CountryPopulationArray[Country.Length];
            
            Parallel.ForEach(arrays, (item, state, index) =>
            {
                // Calculate the value to be added

                // Add the value to the array
                arrays[index] = new CountryPopulationArray {
                    Date = DateTime.Now,
                    PopulationCount = Random.Shared.Next(1550555505, 1650555505),
                    Country = Country[index],
                };
            });
            return arrays;
        }

        public async Task<(CountryPopulationArray[] countries, string loadLocation, string isCachedData)> GetRedisCountryPopulationAsync(DateTime startDate)
        {
            string recordKey = "CountryPopulation_" + DateTime.Now.ToString("yyyMMdd_hhmm");

            var cachData = await _cache.GetRecordAsync<CountryPopulationArray[]>(recordKey);
            if (cachData is null)
            {
                countries = GetCountryPopulationAsync(DateTime.Now);
                _cache.setRecordAsync(recordKey, countries);

                loadLocation = $"Loaded from API at {DateTime.Now}";
                isCachedData = "";
            }
            else
            {
                countries = cachData;
                loadLocation = $"Loaded from cache at {DateTime.Now}";
                isCachedData = "text-danger";
            }

            return (countries, loadLocation, isCachedData);
        }

       
    }

    public class CountryPopulationArray
    {
        public DateTime Date { get; set; }
        public long PopulationCount { get; set; }
        public string Country { get; set; }
    }

    public class CountryPopulationModel : PageModel
    {
        [BindProperty] public CountryPopulationArray[] Countries { get; set; }
        [BindProperty] public string LoadLocation { get; set; } = "";
        [BindProperty] public string IsCachedData { get; set; } = "";

        private readonly IDistributedCache _cache;

        private  readonly  ICountryService _countryService;
        public CountryPopulationModel(ICountryService countryService)
        {
            _countryService = countryService;

        }

        public async Task<PageResult> OnGet()
        {
            (Countries, LoadLocation, IsCachedData) = await _countryService.GetRedisCountryPopulationAsync(DateTime.Now);
            return Page();
        }


      
    }
}
