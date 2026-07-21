namespace Application.DTOs.LabTestElement
{
    public class LabTestElementReadDto
    {
        public int LabTestId { get; set; }
        public int ElementId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
    }
}
