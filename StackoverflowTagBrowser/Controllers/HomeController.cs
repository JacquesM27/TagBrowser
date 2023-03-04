using Microsoft.AspNetCore.Mvc;
using StackoverflowTagBrowser.Data.Enums;
using StackoverflowTagBrowser.Data.Services.StackExchangeService;
using StackoverflowTagBrowser.Exceptions;
using StackoverflowTagBrowser.Models;
using StackoverflowTagBrowser.ViewModels;
using System.Diagnostics;

namespace StackoverflowTagBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStackExchangeService _stackExchangeService;

        public HomeController(IStackExchangeService stackExchangeService)
        {
            _stackExchangeService = stackExchangeService;
        }

        public async Task<IActionResult> Index()
        {
            TagViewModel tagVM;
            try
            {
                tagVM = await _stackExchangeService.GetTagsAsync("stackoverflow", 1000, SortType.popular, SortOrder.desc);
            }
            catch (RateLimitException ex)
            {
                return View("RateLimit", ex.Message);
            }
            catch (NotOkResponseException ex)
            {
                return View("NotOk", new NotOkViewModel() { StatusCode = (int)ex.StatusCode, Description = ex.Message});
            }
            return View(tagVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}