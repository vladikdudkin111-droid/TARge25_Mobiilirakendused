Interaktiivne CarouselView galerii
=================================

ProgrammingLanguagesPage.cs
- Loob CarouselView galerii.
- Seob CarouselView IndicatorView-ga.
- Vahetab kaarti automaatselt iga 10 sekundi järel.
- Kasutab telefonis kohanduvat paigutust: eraldi ülariba ja pealkirja rida.
- Kuvab ühe terve kaardi korraga (ei lõika naaberkaarte ekraani äärtes).
- Tagasinupp avab uuesti tööde peamenüü.
- Kaardi detailinupu tekst muutub vastavalt eesti/inglise keelele.
- Avab kaardile vajutades DisplayAlert detailse infoga.
- Võimaldab Pickeriga valida eesti või inglise keele.

ProgrammingLanguageItem.cs
- Andmemudel ühe galerii kaardi jaoks.

GalleryResources.cs
- Loeb tõlkeid .resx failidest.
- Haldab valitud keelt.

Resources/GalleryResources.resx
- Ingliskeelsed tekstid.

Resources/GalleryResources.et.resx
- Eestikeelsed tekstid.

Pildid asuvad projekti Resources/Images kaustas:
- csharp_gallery.svg
- python_gallery.svg
- javascript_gallery.svg
- java_gallery.svg
- cpp_gallery.svg

Uus taustapilt: Resources/Images/programming_background.jpg
- Programmeerimisteemaline taust; lehe koodis kasutatakse selle kohal gradienti.
