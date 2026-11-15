# PAVADINIMAS – Frontend

React (Vite) frontend'as „Football Game Organizer“ projektui. Minimalus, Apple dizaino principais paremtas stilius, be skaidrių fonų, su šviesiu ir tamsiu režimu.

## Paleidimas

1. Paleiskite backend'ą (http profilis, `http://localhost:5108`):

   ```bash
   cd Backend
   dotnet run --launch-profile http
   ```

2. Paleiskite frontend'ą:

   ```bash
   cd Frontend
   npm install
   npm run dev
   ```

3. Atidarykite http://localhost:5173

Visos `/api/*` užklausos per Vite proxy nukreipiamos į backend'ą, todėl CORS konfigūruoti nereikia.
Kitą backend'o adresą galima nurodyti `Frontend/.env.local` faile:

```
VITE_API_TARGET=https://localhost:7084
```

## Puslapiai

| Kelias | Ką daro | Backend endpointai |
| --- | --- | --- |
| `#/games` | Žaidimų sąrašas (Būsimi / Mano / Praėję), atnaujinamas kas 15 s | `GET /games` |
| `#/games/:id` | Žaidimo informacija, užpildymas, dalyviai, ištrynimas (organizatoriui) | `GET /games/{id}`, `POST /games/{id}/players`, `DELETE /games/{id}/players/me`, `DELETE /games/{id}` |
| `#/create` | Naujo žaidimo kūrimas | `POST /games` |
| `#/fields` | Aikštelių katalogas su filtrais | `GET /sportfields`, `GET /sportfields?city=&gameType=&fieldType=` |
| `#/login` | Prisijungimas / registracija | `POST /accounts/login`, `POST /accounts`, `GET /Accounts/getname` |
| `#/profile` | Profilis, vardo ir slaptažodžio keitimas, paskyros trynimas | `POST /Accounts/changename`, `PUT /accounts/me/password`, `DELETE /accounts/me` |

## Struktūra

```
src/
  api.js            – visi backend'o endpointai
  auth.jsx          – prisijungusio vartotojo kontekstas
  format.js         – datos, būsenos, vertimai į lietuvių k.
  router.js         – minimalus hash router'is
  useFields.js      – aikštelių sąrašo cache
  components/       – UI primityvai, Layout, GameCard, FieldPicker, Toast
  pages/            – puslapiai
  styles.css        – dizaino sistema (CSS kintamieji, šviesus/tamsus režimas)
```
