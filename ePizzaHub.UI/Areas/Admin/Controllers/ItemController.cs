
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ePizzaHub.UI.Areas.Admin.Models;
using System.Text;
using ePizzaHub.Services.Interfaces;
using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net;

namespace ePizzaHub.UI.Areas.Admin.Controllers
{
    public class ItemController : BaseController
    {
        HttpClient _client;
        IConfiguration _configuration;
        IUtilityService _utilityService;
        private readonly IMapper _mapper;
        private string ContainerName = "images";


        public ItemController(HttpClient client, IConfiguration configuration, IUtilityService utilityService, IMapper mapper)
        {
            _client = client;
            _configuration = configuration;
            _client.BaseAddress = new Uri(_configuration["ApiAddress"]);
            _utilityService = utilityService;
            _mapper = mapper;
        }

        public IActionResult Index(int? categoryId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);
            var response = _client.GetAsync(_client.BaseAddress + "/Item/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var items = JsonSerializer.Deserialize<List<ItemVM>>(data);

                if (categoryId!=null)
                {
                    var itemsByCategory = items.Where(i => i.CategoryId == categoryId).ToList();
                    return View(itemsByCategory);
                }
                
             
                return View(items);
            }

        
            return View();
        }


        private IEnumerable<CategoryModel> GetCategories()
        {
            var response = _client.GetAsync(_client.BaseAddress + "/Category/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var categories = JsonSerializer.Deserialize<List<CategoryModel>>(data);
                return categories;
            }
            return null;
           
        }
        // Create and Edit
        public IActionResult Create(int? id)
        {
            ViewBag.categories = new SelectList(GetCategories(), "Id", "Name");
            AddOrEditItemVM itemVM = new AddOrEditItemVM();
            if (id == null || id == 0)
            {
                return View(itemVM);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);
                var response = _client.GetAsync(_client.BaseAddress + "/Item/GetById/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    var item = JsonSerializer.Deserialize<AddOrEditItemVM>(data);
                    return View(item);
                }
                if (response.StatusCode== HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account", new {area=""});
                }
                
                return View(itemVM);

            }
        }

        [HttpPost]
        public IActionResult Create(AddOrEditItemVM model)
        {
           
            if (model.Id != null)
            {
                ModelState.Remove("ImageFile");
            }
            else
            {
                ModelState.Remove("ImageUrl");
            }
            if (ModelState.IsValid)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);
                if (model.Id == 0 || model.Id == null)
                {
                    //Add
                    var mappedData = _mapper.Map<AddOrEditItemModel>(model);
                    if (model.ImageFile != null)
                    {
                        mappedData.ImageUrl = _utilityService.SaveImage(ContainerName, model.ImageFile);

                    }
                    var response = _client.PostAsJsonAsync(_client.BaseAddress + "/Item/Add", mappedData).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "New Item has been created Successfully.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    //Edit
                    var mappedData = _mapper.Map<AddOrEditItemModel>(model);
                    if (model.ImageFile != null)
                    {
                        mappedData.ImageUrl = _utilityService.EditImage(ContainerName, model.ImageFile, model.ImageUrl);
                    }
                    var response = _client.PutAsJsonAsync(_client.BaseAddress + "/Item/Edit/" + model.Id, mappedData).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Item has been updated Successfully.";
                        return RedirectToAction("Index");
                    }
                }

                TempData["error"] = "Operation failed.";
            }
            ViewBag.categories = new SelectList(GetCategories(), "Id", "Name");

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);

            var response = _client.DeleteAsync(_client.BaseAddress + "/Item/Delete/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Item has been deleted Successfully.";
                return RedirectToAction("Index");

            }
            TempData["error"] = "Item could not delete.";
            return RedirectToAction("Index");
        }

    }

}
