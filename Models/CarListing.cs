using System.ComponentModel.DataAnnotations;

namespace Lotomoto.Models;

public class CarListing
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wprowadź tytuł ogłoszenia.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wprowadź cenę.")]
    [Range(1, 9999999, ErrorMessage = "Cena musi być większa niż 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Wprowadź przebieg.")]
    [Range(0, 2000000, ErrorMessage = "Przebieg musi być w poprawnym zakresie.")]
    public int Mileage { get; set; }

    [Required(ErrorMessage = "Wprowadź rok.")]
    [Range(1900, 2100, ErrorMessage = "Rok musi być poprawny.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Wprowadź rodzaj pojazdu.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wprowadź wersję.")]
    public string Version { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wprowadź opis.")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
