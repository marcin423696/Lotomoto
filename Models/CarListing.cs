using System.ComponentModel.DataAnnotations;

namespace Lotomoto.Models;

public class CarListing
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Wprowadź tytuł ogłoszenia.")]
    [Display(Name = "Tytuł")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wprowadź cenę.")]
    [Range(1, 9999999, ErrorMessage = "Cena musi być większa niż 0.")]
    [Display(Name = "Cena zł")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Wprowadź przebieg.")]
    [Range(0, 2000000, ErrorMessage = "Przebieg musi być w poprawnym zakresie.")]
    [Display(Name = "Przebieg km")]
    public int Mileage { get; set; }

    [Required(ErrorMessage = "Wprowadź rok produkcji.")]
    [Range(1900, 2100, ErrorMessage = "Rok musi być poprawny.")]
    [Display(Name = "Rok produkcji")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Wybierz rodzaj pojazdu.")]
    [Display(Name = "Rodzaj pojazdu")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wprowadź moc silnika.")]
    [Range(1, 2000, ErrorMessage = "Moc musi być w poprawnym zakresie.")]
    [Display(Name = "Moc KM")]
    public int Power { get; set; }

    [Required(ErrorMessage = "Wprowadź opis.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Opis")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Zdjęcie")]
    public string ImageUrl { get; set; } = string.Empty;
}