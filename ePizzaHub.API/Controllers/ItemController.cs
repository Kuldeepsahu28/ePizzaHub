using AutoMapper;
using ePizzaHub.API.DTOs;
using ePizzaHub.API.Helpers;
using ePizzaHub.Core.Entities;
using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ePizzaHub.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class ItemController : ControllerBase
    {
       
        IItemService _itemService;
        IMapper _mapper;


        public ItemController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
    
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items= _itemService.GetItemsWithCategory();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item= _itemService.GetById(id);
            var mappedData=_mapper.Map<ItemDTO>(item);

            return Ok(mappedData);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public IActionResult Add(AddEditItemDTO item)
        {
            try
            {
               var mappedData = _mapper.Map<Item>(item);            
                _itemService.Add(mappedData);
                return StatusCode(StatusCodes.Status201Created);
             
            }
            catch (Exception ex)
            {
               return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("{id}")]
        public IActionResult Edit(int id,AddEditItemDTO item)
        {
            try
            {
                if (id!=item.Id)
                {
                    return BadRequest();
                }
                var mappedData = _mapper.Map<Item>(item);

                _itemService.Update(mappedData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var item = _itemService.GetById(id);
                if (item == null) { 
                    return BadRequest();
                }
                _itemService.Remove(item);
                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
         
            }
        }
    }
}
