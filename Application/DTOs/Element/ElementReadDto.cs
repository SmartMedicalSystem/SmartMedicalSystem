namespace Application.DTOs.Element
{
    public class ElementReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Description { get; set; }
    }
}
