using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using RedisDemo.Repository;
using RedisDemo.Repository.Interface;

namespace RedisDemo.Pages
{
    

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
