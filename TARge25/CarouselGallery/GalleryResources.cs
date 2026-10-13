using System.Globalization;
using System.Resources;

namespace TARge25.CarouselGallery;

// See klass loeb tekste .resx failidest ja võimaldab keelt vahetada.
public static class GalleryResources
{
    // ResourceManager teab, kust leida GalleryResources.resx tõlkefailid.
    private static readonly ResourceManager ResourceManager = new(
        "TARge25.CarouselGallery.Resources.GalleryResources",
        typeof(GalleryResources).Assembly);

    // Vaikimisi kasutame eesti keelt.
    private static CultureInfo currentCulture = new("et");

    // Muudab galeriis kasutatavat keelt.
    public static void SetCulture(string cultureCode)
    {
        currentCulture = new CultureInfo(cultureCode);
    }

    // Tagastab võtmele vastava tõlgitud teksti aktiivses keeles.
    public static string Get(string key)
    {
        return ResourceManager.GetString(key, currentCulture) ?? key;
    }
}
