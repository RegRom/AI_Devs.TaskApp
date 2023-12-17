using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class PersonDto
{
    [JsonPropertyName("imie")]
    public string Name { get; set; }

    [JsonPropertyName("nazwisko")]
    public string Surname { get; set; }

    [JsonPropertyName("wiek")]
    public int Age { get; set; }

    [JsonPropertyName("o_mnie")]
    public string About { get; set; }

    [JsonPropertyName("ulubiona_postac_z_kapitana_bomby")]
    public string FavCharacter { get; set; }

    [JsonPropertyName("ulubiony_serial")]
    public string FavSeries { get; set; }

    [JsonPropertyName("ulubiony_film")]
    public string FavMovie { get; set; }

    [JsonPropertyName("ulubiony_kolor")]
    public string FavColor { get; set; }
}
