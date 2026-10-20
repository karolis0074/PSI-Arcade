import { useMemo, useState } from 'react';
import { fieldTypeLabel, gameTypeLabel } from '../format.js';
import { IconCheck, IconSearch } from './Icons.jsx';

export default function FieldPicker({ fields, value, onChange }) {
  const [query, setQuery] = useState('');
  const [type, setType] = useState('');

  const types = useMemo(() => [...new Set(fields.map((f) => f.gameType))].sort(), [fields]);

  const results = useMemo(() => {
    const q = query.trim().toLowerCase();
    return fields
      .filter((f) => (!type || f.gameType === type) && (!q || f.name.toLowerCase().includes(q)))
      .slice(0, 50);
  }, [fields, query, type]);

  const selected = fields.find((f) => f.id === value);

  return (
    <div className="picker">
      <div className="picker-controls">
        <div className="search">
          <IconSearch />
          <input
            type="search"
            placeholder="Ieškoti aikštelės"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
        </div>
        <select className="input select picker-type" value={type} onChange={(e) => setType(e.target.value)}>
          <option value="">Visos sporto šakos</option>
          {types.map((t) => (
            <option key={t} value={t}>
              {gameTypeLabel(t)}
            </option>
          ))}
        </select>
      </div>

      <div className="picker-list" role="listbox" aria-label="Aikštelės">
        {selected && !results.includes(selected) && (
          <PickerRow field={selected} selected onSelect={onChange} />
        )}
        {results.map((f) => (
          <PickerRow key={f.id} field={f} selected={f.id === value} onSelect={onChange} />
        ))}
        {results.length === 0 && <div className="picker-empty">Nieko nerasta</div>}
      </div>
    </div>
  );
}

function PickerRow({ field, selected, onSelect }) {
  return (
    <button
      type="button"
      role="option"
      aria-selected={selected}
      className={`picker-row${selected ? ' selected' : ''}`}
      onClick={() => onSelect(field.id)}
    >
      <span className="picker-row-text">
        <span className="picker-row-name">{field.name}</span>
        <span className="picker-row-meta">
          {gameTypeLabel(field.gameType)} · {fieldTypeLabel(field.fieldType)} · iki {field.capacity} žaid.
        </span>
      </span>
      {selected && <IconCheck className="accent" />}
    </button>
  );
}
