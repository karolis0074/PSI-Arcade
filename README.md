# Football Game Organizer

## Projekto aprašymas

„Football Game Organizer“ – tai internetinė platforma, skirta mėgėjų futbolo žaidimų organizavimui.

Sistema leidžia vartotojams rasti tinkamus futbolo žaidimus, pasirinkti aikštelę, sukurti naują žaidimą ir prisijungti prie kitų vartotojų organizuojamų žaidimų.

Pagrindinis sistemos tikslas – automatizuoti žaidimo organizavimo procesą: nuo aikštelės ir žaidimo pasirinkimo iki žaidėjų registracijos, vietų tikrinimo, žaidimo patvirtinimo bei statistikos atnaujinimo.

## Pagrindinės funkcijos

### 1. Vartotojo paskyra

* Registracija ir prisijungimas.
* Profilio informacijos valdymas.
* Dalyvautų žaidimų istorija.
* Asmeninės statistikos peržiūra.

### 2. Futbolo aikštelių katalogas

* Aikštelių informacija.
* Adresas ir vietovė.
* Aikštelės tipas ir talpa.
* Aikštelės naudojimo statistika.
* Vartotojų įvertinimai.

### 3. Aikštelių paieška

Vartotojas gali ieškoti aikštelių pagal:

* Miestą.
* Rajoną.
* Aikštelės tipą.
* Žaidėjų skaičių.

Sistema gali pateikti tinkamiausias aikšteles pagal vartotojo pasirinktus kriterijus.

### 4. Žaidimo sukūrimas

Vartotojas gali sukurti naują futbolo žaidimą nurodydamas:

* Aikštelę.
* Datą.
* Pradžios laiką.
* Maksimalų žaidėjų skaičių.

Sistema patikrina, ar pasirinkta aikštelė ir laikas gali būti panaudoti naujam žaidimui.

### 5. Prisijungimas prie žaidimo

* Vartotojas gali prisijungti prie atviro žaidimo.
* Vartotojas gali iš jo pasitraukti.
* Sistema neleidžia viršyti maksimalaus žaidėjų skaičiaus.
* Dalyvių sąrašas atnaujinamas automatiškai.

### 6. Žaidėjų skaičiaus ir vietų valdymas

Sistema nuolat tikrina žaidimo užpildymą.

Pavyzdžiui:

```text
5 / 10 žaidėjų → Open
8 / 10 žaidėjų → Almost Full
10 / 10 žaidėjų → Full
```

Kai žaidimas užsipildo, nauji žaidėjai nebegali prisijungti.

Jeigu žaidėjas pasitraukia, laisva vieta vėl tampa prieinama.

### 7. Žaidimo būsenų valdymas

Žaidimas turi gyvavimo ciklą:

```text
Open
  ↓
Almost Full
  ↓
Full
  ↓
Confirmed
  ↓
In Progress
  ↓
Completed
```

Sistema automatiškai keičia būseną pagal:

* Žaidėjų skaičių.
* Žaidimo pradžios laiką.
* Žaidimo užbaigimą.

Neleistini būsenų pakeitimai turi būti blokuojami backend dalyje.

### 8. Žaidimų rekomendacijos

Sistema gali atrinkti vartotojui tinkamiausius žaidimus pagal:

* Vietovę.
* Datą ir laiką.
* Laisvų vietų skaičių.
* Žaidimo populiarumą.
* Ankstesnius vartotojo pasirinkimus.

Žaidimams gali būti apskaičiuojamas rekomendacijos balas, pagal kurį jie pateikiami vartotojui.

### 9. Žaidimų statistika

Sistema kaupia ir apskaičiuoja:

* Žaidimų skaičių.
* Vidutinį žaidėjų skaičių.
* Populiariausias aikšteles.
* Populiariausius žaidimo laikus.
* Vartotojo dalyvavimo statistiką.

Statistika gali būti naudojama ir rekomendacijų sistemoje.

### 10. Žaidimų istorija ir įvertinimai

Po žaidimo vartotojai gali:

* Peržiūrėti savo žaidimų istoriją.
* Įvertinti futbolo aikštelę.
* Peržiūrėti kitų vartotojų įvertinimus.
* Matyti bendrą aikštelės įvertinimą.

Užbaigto žaidimo duomenys naudojami statistikai atnaujinti.

## Pagrindinis sistemos procesas

```text
Aikštelės pasirinkimas
        ↓
Žaidimo sukūrimas
        ↓
Žaidėjų registracija
        ↓
Vietų tikrinimas
        ↓
Žaidimo statuso atnaujinimas
        ↓
Žaidimo patvirtinimas
        ↓
Žaidimo pradžia
        ↓
Žaidimo užbaigimas
        ↓
Statistikos atnaujinimas
        ↓
Aikštelės įvertinimas
```

## Automatiniai procesai

Kad sistema nebūtų paremta vien tik paprastomis CRUD operacijomis, dalis procesų bus atliekama automatiškai.

`BackgroundService` periodiškai gali:

* Tikrinti artėjančių žaidimų laiką.
* Keisti žaidimų būsenas.
* Uždaryti registraciją prasidėjus žaidimui.
* Pažymėti pasibaigusius žaidimus.
* Paleisti užbaigto žaidimo statistikos atnaujinimą.

Backend bus atsakingas už:

* Žaidimų kūrimo validaciją.
* Aikštelių ir laikų tikrinimą.
* Žaidėjų registracijos kontrolę.
* Maksimalaus žaidėjų skaičiaus užtikrinimą.
* Žaidimų būsenų valdymą.
* Rekomendacijų balo apskaičiavimą.
* Statistikos skaičiavimą.
* Įvertinimų valdymą.
* Automatinius foninius procesus.

React frontend dalis daugiausia bus naudojama duomenų atvaizdavimui ir vartotojo veiksmams, o pagrindinė logika bus atliekama backend'e.

## Duomenų bazė

Pagrindinės duomenų bazės lentelės:

```text
Users
  ↓
Games ← FootballFields
  ↓
GamePlayers
  ↓
GameResults
  ↓
Statistics

FootballFields
  ↓
Ratings
```

### Pagrindiniai objektai

* `Users` – vartotojai.
* `FootballFields` – futbolo aikštelės.
* `Games` – organizuojami žaidimai.
* `GamePlayers` – vartotojų dalyvavimas žaidimuose.
* `GameResults` – užbaigtų žaidimų rezultatai.
* `Ratings` – aikštelių įvertinimai.
* `Statistics` – apskaičiuota žaidimų ir vartotojų statistika.

## Projekto rezultatas

Galutinis produktas leis vartotojui:

1. Susikurti paskyrą.
2. Rasti tinkamą futbolo aikštelę.
3. Sukurti futbolo žaidimą.
4. Pakviesti arba priimti kitus žaidėjus.
5. Matyti laisvų vietų skaičių.
6. Automatiškai valdyti žaidimo būseną.
7. Rasti rekomenduojamus žaidimus.
8. Užbaigti žaidimą ir išsaugoti jo rezultatą.
9. Kaupti žaidimų statistiką.
10. Įvertinti aikštelę ir peržiūrėti savo žaidimų istoriją.

 ## Projekto komanda 
 * Lukas Kriaučiūnas
 * Karolis Čiburas
 * Armandas Kiaunė
 * Džiugas Arcimavičius

