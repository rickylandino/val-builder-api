using val_builder_api.Dto;
using val_builder_api.Models;

namespace val_builder_api.Services;

public interface IValTemplateItemService
{
    Task<IEnumerable<ValtemplateItem>> GetValTemplateItemsByGroupIdAsync(int groupId);
    Task<ValtemplateItem?> GetValTemplateItemByIdAsync(int id);
    Task<ValtemplateItem> CreateValTemplateItemAsync(ValtemplateItem item);
    Task<ValtemplateItem?> UpdateValTemplateItemAsync(int id, ValtemplateItem item);
    Task UpdateDisplayOrderBulkAsync(int groupId, List<ValTemplateItemDisplayOrderUpdateDto.ItemOrder> items);
}
