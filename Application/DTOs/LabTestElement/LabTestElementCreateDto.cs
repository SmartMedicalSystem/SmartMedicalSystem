namespace Application.DTOs.LabTestElement
{
    public class LabTestElementCreateDto
    {
        public int LabTestId { get; set; }
        public int ElementId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}
