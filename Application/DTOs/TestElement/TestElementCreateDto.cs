namespace Application.DTOs.TestElement
{
    public class TestElementCreateDto
    {
        public string ElementName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public float NormalMin { get; set; }
        public float NormalMax { get; set; }
    }
}
