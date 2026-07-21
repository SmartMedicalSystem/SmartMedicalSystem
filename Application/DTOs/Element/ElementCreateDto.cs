namespace Application.DTOs.Element
{
    public class ElementCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Description { get; set; }
    }
}
