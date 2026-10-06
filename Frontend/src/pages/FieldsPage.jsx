import { useEffect, useMemo, useState } from 'react';
import { sportFields } from '../api.js';
import { fieldTypeLabel, gameTypeLabel, mapUrl, surfaceLabel } from '../format.js';
import { useFields } from '../useFields.js';
import { EmptyState, ErrorNote, Segmented, SelectField, Spinner } from '../components/ui.jsx';
import { IconClock, IconSearch } from '../components/Icons.jsx';

const PAGE = 24;

export default function FieldsPage() {
  const all = useFields();
  const [city, setCity] = useState('');
  const [gameType, setGameType] = useState('');
  const [fieldType, setFieldType] = useState('');
  const [query, setQuery] = useState('');
  const [results, setResults] = useState(null);
  const [error, setError] = useState(null);
  const [limit, setLimit] = useState(PAGE);

  const cities = useMemo(() => [...new Set(all.fields.map((f) => f.city))].sort(), [all.fields]);
  const types = useMemo(() => {
    const counts = {};
    all.fields.forEach((f) => (counts[f.gameType] = (counts[f.gameType] ?? 0) + 1));
    return Object.keys(counts).sort((a, b) => counts[b] - counts[a]);
  }, [all.fields]);

  useEffect(() => {
    if (!all.available) return;
    let alive = true;
    setResults(null);
    setLimit(PAGE);
    sportFields.search({ city, gameType, fieldType }).then(
      (data) => alive && (setResults(data ?? []), setError(null)),
      (err) => alive && setError(err)
    );
    return () => {
      alive = false;
    };
  }, [all.available, city, gameType, fieldType]);

  const shown = useMemo(() => {
    const q = query.trim().toLowerCase();
    return (results ?? []).filter((f) => !q || f.name.toLowerCase().includes(q));
  }, [results, query]);

  if (all.loading)
    return (
      <div className="page">
        <h1 className="large-title">Aikštelės</h1>
        <Spinner />
      </div>
    );

  if (all.available === false)
    return (
      <div className="page">
        <h1 className="large-title">Aikštelės</h1>
        <div className="note note-error" style={{ marginTop: 24 }}>Įvyko klaida.</div>
      </div>
    );

  return (
    <div className="page">
      <h1 className="large-title">Aikštelės</h1>
      <p className="subtitle">{all.fields.length} aikštelės{cities.length === 1 ? ` · ${cities[0]}` : ''}</p>

      <ErrorNote error={all.error} />

      <div className="filters">
        <div className="search search-lg">
          <IconSearch />
          <input
            type="search"
            placeholder="Ieškoti pagal pavadinimą"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
        </div>
        <div className="filters-row">
          {cities.length > 1 && (
            <SelectField value={city} onChange={(e) => setCity(e.target.value)} aria-label="Miestas">
              <option value="">Visi miestai</option>
              {cities.map((c) => (
                <option key={c}>{c}</option>
              ))}
            </SelectField>
          )}
          <SelectField value={gameType} onChange={(e) => setGameType(e.target.value)} aria-label="Sporto šaka">
            <option value="">Visos sporto šakos</option>
            {types.map((t) => (
              <option key={t} value={t}>
                {gameTypeLabel(t)}
              </option>
            ))}
          </SelectField>
          <Segmented
            label="Dydis"
            value={fieldType}
            onChange={setFieldType}
            options={[
              { value: '', label: 'Visos' },
              { value: 'S', label: 'Maža' },
              { value: 'M', label: 'Vidutinė' },
              { value: 'L', label: 'Didelė' },
            ]}
          />
        </div>
      </div>

      <ErrorNote error={error} />
      {!results && !error && <Spinner />}

      {results && shown.length === 0 && <EmptyState title="Nieko nerasta" text="Pabandykite pakeisti filtrus." />}

      {shown.length > 0 && (
        <>
          <div className="field-grid">
            {shown.slice(0, limit).map((f) => (
              <FieldCard key={f.id} field={f} />
            ))}
          </div>
          {shown.length > limit && (
            <div className="center-pad">
              <button type="button" className="btn btn-secondary" onClick={() => setLimit((l) => l + PAGE)}>
                Rodyti daugiau ({shown.length - limit})
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}

function FieldCard({ field }) {
  const map = mapUrl(field);
  const sameHours =
    field.weekdayOpenTime === field.weekendOpenTime && field.weekdayCloseTime === field.weekendCloseTime;

  return (
    <article className="field-card">
      <span className="eyebrow">{gameTypeLabel(field.gameType)}</span>
      <h3 className="field-card-title">{field.name}</h3>
      <div className="tags">
        <span className="tag">{fieldTypeLabel(field.fieldType)}</span>
        {field.surface && <span className="tag">{surfaceLabel(field.surface)}</span>}
        <span className="tag">iki {field.capacity} žaid.</span>
      </div>
      <p className="field-card-hours">
        <IconClock />
        {sameHours ? (
          <>
            Kasdien {field.weekdayOpenTime}–{field.weekdayCloseTime}
          </>
        ) : (
          <>
            I–V {field.weekdayOpenTime}–{field.weekdayCloseTime} · VI–VII {field.weekendOpenTime}–
            {field.weekendCloseTime}
          </>
        )}
      </p>
      <div className="field-card-actions">
        <a className="btn btn-primary btn-sm" href={`#/create?field=${field.id}`}>
          Kurti žaidimą
        </a>
        {map && (
          <a className="btn btn-secondary btn-sm" href={map} target="_blank" rel="noreferrer">
            Žemėlapis
          </a>
        )}
      </div>
    </article>
  );
}
