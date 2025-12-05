namespace SWP391_Project.Dtos;

public class CityDto
{
    public int ProvinceId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
}
