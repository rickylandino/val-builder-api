namespace val_builder_api.Dto;

public class ValTemplateItemDisplayOrderUpdateDto
{
    public int GroupId { get; set; }
    public List<ItemOrder> Items { get; set; } = new();

    public class ItemOrder
    {
        public int ItemId { get; set; }
        public int DisplayOrder { get; set; }
    }
}