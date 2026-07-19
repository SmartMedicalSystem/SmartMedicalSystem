namespace Application.DTOs.TestElement
{
    public class TestElementReadDto
    {
        public int Id { get; set; }
        public string ElementName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public float NormalMin { get; set; }
        public float NormalMax { get; set; }
    }
}
