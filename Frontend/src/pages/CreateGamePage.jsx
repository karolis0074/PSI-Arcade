import { useEffect, useMemo, useState } from 'react';
import { games as gamesApi } from '../api.js';
import { useAuth } from '../auth.jsx';
import { useFields } from '../useFields.js';
import FieldPicker from '../components/FieldPicker.jsx';
import { Button, EmptyState, Spinner, Stepper, TextField } from '../components/ui.jsx';
import { IconPerson } from '../components/Icons.jsx';
import { useToast } from '../components/Toast.jsx';
import { navigate } from '../router.js';

function pad(n) {
  return String(n).padStart(2, '0');
}

function defaultDateTime() {
  const d = new Date(Date.now() + 24 * 3600 * 1000);
  return {
    date: `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`,
    time: '18:00',
  };
}

function todayIso() {
  const d = new Date();
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

function openingHours(field, date) {
  if (!field || !date) return null;
  const day = new Date(date + 'T12:00').getDay();
  const weekend = day === 0 || day === 6;
  const open = weekend ? field.weekendOpenTime : field.weekdayOpenTime;
  const close = weekend ? field.weekendCloseTime : field.weekdayCloseTime;
  if (!open || !close) return null;
  return { open, close, weekend };
}

export default function CreateGamePage({ fieldId }) {
  const { user } = useAuth();
  const toast = useToast();
  const fieldsState = useFields();
  const initial = useMemo(defaultDateTime, []);

  const [field, setField] = useState(fieldId ?? null);
  const [manualId, setManualId] = useState(fieldId ? String(fieldId) : '');
  const [date, setDate] = useState(initial.date);
  const [time, setTime] = useState(initial.time);
  const [maxPlayers, setMaxPlayers] = useState(10);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const selectedField = fieldsState.byId.get(field);

  useEffect(() => {
    if (selectedField?.capacity) setMaxPlayers(selectedField.capacity);
  }, [selectedField]);

  if (!user)
    return (
      <div className="page page-narrow">
        <EmptyState
          icon={<IconPerson width={40} height={40} strokeWidth={1.4} />}
          title="Prisijunkite, kad sukurtumėte žaidimą"
          text="Žaidimą gali sukurti tik prisijungę vartotojai."
          action={
            <a className="btn btn-primary" href="#/login?next=/create">
              Prisijungti
            </a>
          }
        />
      </div>
    );

  const hours = openingHours(selectedField, date);
  const outsideHours = hours && time && (time < hours.open || time > hours.close);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);

    const footballFieldId = fieldsState.available ? field : Number(manualId);
    if (!footballFieldId || footballFieldId <= 0) {
      setError('Pasirinkite aikštelę.');
      return;
    }
    const start = new Date(`${date}T${time}`);
    if (Number.isNaN(start.getTime())) {
      setError('Nurodykite datą ir laiką.');
      return;
    }
    if (start <= new Date()) {
      setError('Žaidimo pradžia turi būti ateityje.');
      return;
    }

    setSubmitting(true);
    try {
      const created = await gamesApi.create({
        footballFieldId,
        createdByUsername: user.username,
        startTime: start.toISOString(),
        maxPlayers,
      });
      toast('Žaidimas sukurtas', 'success');
      navigate(`/games/${created.id}`);
    } catch (err) {
      setError(err.message);
      setSubmitting(false);
    }
  }

  return (
    <div className="page page-narrow">
      <h1 className="large-title">Naujas žaidimas</h1>
      <p className="subtitle">Pasirinkite aikštelę, laiką ir žaidėjų skaičių.</p>

      <form className="form" onSubmit={handleSubmit}>
        <section className="form-section">
          <h3 className="group-title">Aikštelė</h3>
          {fieldsState.loading ? (
            <Spinner />
          ) : fieldsState.available ? (
            <FieldPicker fields={fieldsState.fields} value={field} onChange={setField} />
          ) : (
            <div className="panel">
              <TextField
                label="Aikštelės ID"
                type="number"
                min={1}
                inputMode="numeric"
                value={manualId}
                onChange={(e) => setManualId(e.target.value)}
                required
              />
            </div>
          )}
        </section>

        <section className="form-section">
          <h3 className="group-title">Laikas</h3>
          <div className="panel">
            <div className="grid-2">
              <TextField label="Data" type="date" min={todayIso()} value={date} onChange={(e) => setDate(e.target.value)} required />
              <TextField label="Pradžia" type="time" value={time} onChange={(e) => setTime(e.target.value)} required />
            </div>
            {hours && (
              <p className={`field-hint${outsideHours ? ' warn' : ''}`}>
                {outsideHours ? 'Pasirinktas laikas už darbo valandų ribų. ' : ''}
                Aikštelė {hours.weekend ? 'savaitgalį' : 'darbo dienomis'} dirba {hours.open}–{hours.close}.
              </p>
            )}
          </div>
        </section>

        <section className="form-section">
          <h3 className="group-title">Žaidėjai</h3>
          <div className="panel panel-row">
            <div>
              <div className="row-label">Maksimalus žaidėjų skaičius</div>
              {selectedField && <div className="field-hint">Aikštelės talpa – {selectedField.capacity}</div>}
            </div>
            <Stepper value={maxPlayers} onChange={setMaxPlayers} min={2} max={50} label="Žaidėjų skaičius" />
          </div>
        </section>

        {error && <div className="note note-error">{error}</div>}

        <Button type="submit" size="lg" block loading={submitting}>
          Sukurti žaidimą
        </Button>
      </form>
    </div>
  );
}
