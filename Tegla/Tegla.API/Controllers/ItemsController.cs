using Microsoft.AspNetCore.Mvc;
using Tegla.Application.Services.Items;
using Tegla.Application.Services.Items.Models;
using Tegla.Domain.Models.Items.Exceptions;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService) {
        _itemService = itemService;
    }

    [HttpPost]
    public async ValueTask<IActionResult> CreateItem(CreateItemRequest item) {
        try {
            var res = await _itemService.AddItem(item);
            return Ok(res);
        }
        catch (ItemValidationException itemValidationException) {
            return BadRequest(GetMessage(itemValidationException));
        }
        catch (ItemDependencyException itemDependencyException) {
            return BadRequest(GetMessage(itemDependencyException));
        }
        catch (ItemServiceException itemServiceException) {
            return BadRequest(GetMessage(itemServiceException));
        }
    }

    [HttpGet]
    public async ValueTask<IActionResult> GetItems() {
        try {
            var res = await _itemService.ListAllItems();
            return Ok(res);
        }
        catch (ItemDependencyException itemDependencyException) {
            return BadRequest(GetMessage(itemDependencyException));
        }
        catch (ItemServiceException itemServiceException) {
            return BadRequest(GetMessage(itemServiceException));
        }
    }

    [HttpPut]
    public async ValueTask<IActionResult> UpdateItem(UpdateItemRequest model) {
        try {
            var res = await _itemService.ModifyItem(model);
            return Ok(res);
        }
        catch (ItemValidationException itemValidationException) {
            return BadRequest(GetMessage(itemValidationException));
        }
        catch (ItemDependencyException itemDependencyException) {
            return BadRequest(GetMessage(itemDependencyException));
        }
        catch (ItemServiceException itemServiceException) {
            return BadRequest(GetMessage(itemServiceException));
        }
    }

    [HttpGet("{id}")]
    public async ValueTask<IActionResult> GetItem(Guid id) {
        try {
            var res = await _itemService.RetriveItemById(id);
            return Ok(res);
        }
        catch (ItemValidationException itemValidationException) {
            return BadRequest(GetMessage(itemValidationException));
        }
        catch (ItemDependencyException itemDependencyException) {
            return BadRequest(GetMessage(itemDependencyException));
        }
        catch (ItemServiceException itemServiceException) {
            return BadRequest(GetMessage(itemServiceException));
        }
    }

    [HttpDelete("{id}")]
    public async ValueTask<IActionResult> RemoveItem(Guid id) {
        try {
            var res = await _itemService.RemoveItemById(id);
            return Ok(res);
        }
        catch (ItemValidationException itemValidationException) {
            return BadRequest(GetMessage(itemValidationException));
        }
        catch (ItemDependencyException itemDependencyException) {
            return BadRequest(GetMessage(itemDependencyException));
        }
        catch (ItemServiceException itemServiceException) {
            return BadRequest(GetMessage(itemServiceException));
        }
    }

    private string GetMessage(Exception exception) =>
        exception.InnerException.Message;
}