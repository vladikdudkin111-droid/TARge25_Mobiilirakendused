namespace TARge25.CarouselGallery;

// Mudel kirjeldab ühte programmeerimiskeele kaarti CarouselView galeriis.
public class ProgrammingLanguageItem
{
    // Pildi faili nimi Resources/Images kaustast.
    public string ImageSource { get; set; } = string.Empty;

    // Programmeerimiskeele nimi, mida kuvatakse kaardi pealkirjana.
    public string Name { get; set; } = string.Empty;

    // Lühike kirjeldus, mida kuvatakse kaardil.
    public string Description { get; set; } = string.Empty;

    // Kaardi detailinupu tekst valitud keeles (loetakse .resx failist).
    public string ButtonText { get; set; } = string.Empty;

    // Pikem info, mida näidatakse kaardile vajutades DisplayAlert aknas.
    public string Detail { get; set; } = string.Empty;
}
