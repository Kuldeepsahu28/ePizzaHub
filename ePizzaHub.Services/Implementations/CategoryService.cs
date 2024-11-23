using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Implementations
{
    public class CategoryService:Service<Category>, ICategoryService
    {
        private readonly IMapper _mappper;

        public CategoryService(IRepository<Category> categoryRepository,IMapper mappper):base(categoryRepository) 
        {
            _mappper = mappper;
        }

        public IEnumerable<CategoryModel> GetCategories()
        {
            var category = _repository.GetAll();

            return _mappper.Map<IEnumerable<CategoryModel>>(category);
        }
    }
}
