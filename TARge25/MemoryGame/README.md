# OOP-põhine interaktiivne Memo mäng

Uus ülesanne on lisatud TARge25 peamenüüsse nupuga **OOP Memo mäng**.

## Failid

- `MemoryGamePage.xaml` — mängu kasutajaliides: 4×4 kaartide ruudustik, teema Picker, mängija nimi, nupud ja tulemused.
- `MemoryGamePage.xaml.cs` — ühendab XAML-i ja OOP klassid, töötleb puudutused, kuvab DisplayAlert'i ja animatsiooni ning haldab taimerit.
- `Game.cs` — mängureeglid: kaartide segamine, paari kontrollimine, käigud, punktid, paus ja võit.
- `Card.cs` — üks kaart: sümbol, paarinumber, avatud/peidetud olek ning visuaalsed omadused.
- `Player.cs` — mängija nimi ja statistika; salvestab võidud, parima skoori ning kiireima aja `Preferences` kaudu.
- `Theme.cs` — kolm teemat, värvid, font ja `Apply(ContentPage page)` meetod. `DynamicResource` uuendab XAML kujunduse kohe.

## Kasutamine

1. Ava peamenüüs **OOP Memo mäng**.
2. Vali teema: `Hele`, `Tume` või `Värviline`.
3. Puuduta järjest kahte kaarti. Kui need on sama sümboliga, jäävad avatuks.
4. Kui kõik 8 paari on leitud, kuvatakse võiduaken koos tulemusega.
5. `Uus mäng` segab kaardid uuesti. `Paus` peatab ajaloenduri ja blokeerib kaardid.
6. Statistika säilib rakenduse taaskäivitamisel.

Kõik olulised C# meetodid on kommenteeritud eesti keeles `//` kommentaaridega. XAML selgitused on XML-kommentaarid `<!-- ... -->`, sest XAML-is ei saa kasutada `//` kommentaare.
