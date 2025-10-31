
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Dtos.Inventory;

public class CategoryDetails
{
    public Ulid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public int? ItemCount{ get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ModifiedAt { get; set; } = string.Empty;
    public string ModifiedBy { get; set; }
}
public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    public string CategoryName { get; set; } = null!;
    public string Description { get; set; } = string.Empty;

}
public class UpdateCategoryDetails
{
    public Ulid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ModifiedAt { get; set; } = string.Empty;
    public string ModifiedBy { get; set; }
}