import { useCallback, useEffect, useMemo, useState } from 'react';
import { games as gamesApi } from '../api.js';
import { useAuth } from '../auth.jsx';
import { isParticipant, parseDate, statusKey } from '../format.js';
import { useFields } from '../useFields.js';
import GameCard from '../components/GameCard.jsx';
import { Button, EmptyState, ErrorNote, Segmented, Spinner } from '../components/ui.jsx';
import { IconCalendar, IconPlus } from '../components/Icons.jsx';
import { navigate } from '../router.js';

const REFRESH_MS = 15000;

export default function GamesPage() {
  const { user } = useAuth();
  const { byId } = useFields();
  const [list, setList] = useState(null);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState('upcoming');

  const load = useCallback(async () => {
    try {
      const data = await gamesApi.getAll();
      setList(Array.isArray(data) ? data : []);
      setError(null);
    } catch (err) {
      setError(err);
    }
  }, []);

  useEffect(() => {
    load();
    const t = setInterval(load, REFRESH_MS);
    return () => clearInterval(t);
  }, [load]);

  const shown = useMemo(() => {
    if (!list) return [];
    const now = Date.now();
    const sorted = [...list].sort((a, b) => parseDate(a.startTime) - parseDate(b.startTime));
    switch (filter) {
      case 'upcoming':
        return sorted.filter((g) => parseDate(g.startTime) > now && statusKey(g.status) !== 'Completed');
      case 'mine':
        return sorted.filter((g) => isParticipant(g, user?.username));
      case 'past':
        return sorted
          .filter((g) => parseDate(g.startTime) <= now || statusKey(g.status) === 'Completed')
          .reverse();
      default:
        return sorted;
    }
  }, [list, filter, user]);

  const options = [
    { value: 'upcoming', label: 'Būsimi' },
    ...(user ? [{ value: 'mine', label: 'Mano' }] : []),
    { value: 'past', label: 'Praėję' },
  ];

  const createHref = user ? '#/create' : '#/login?next=/create';

  return (
    <div className="page">
      <div className="page-head">
        <div>
          <h1 className="large-title">Žaidimai</h1>
          <p className="subtitle">Raskite žaidimą netoliese arba sukurkite savo.</p>
        </div>
        <Button onClick={() => navigate(createHref.slice(1))} className="hide-mobile">
          <IconPlus width={18} height={18} strokeWidth={2.2} /> Naujas žaidimas
        </Button>
      </div>

      <div className="toolbar">
        <Segmented options={options} value={filter} onChange={setFilter} label="Filtras" />
        {list && <span className="muted small">{shown.length} žaid.</span>}
      </div>

      <ErrorNote error={error} onRetry={load} />

      {!list && !error && <Spinner />}

      {list && shown.length === 0 && (
        <EmptyState
          icon={<IconCalendar width={40} height={40} strokeWidth={1.4} />}
          title={filter === 'mine' ? 'Jūs dar nedalyvaujate žaidimuose' : 'Žaidimų nėra'}
          text={
            filter === 'past'
              ? 'Čia matysite įvykusius žaidimus.'
              : 'Būkite pirmas – sukurkite žaidimą ir pakvieskite kitus.'
          }
          action={
            filter !== 'past' && (
              <a href={createHref} className="btn btn-primary">
                Sukurti žaidimą
              </a>
            )
          }
        />
      )}

      {shown.length > 0 && (
        <div className="card-list">
          {shown.map((g) => (
            <GameCard key={g.id} game={g} field={byId.get(g.footballFieldId)} />
          ))}
        </div>
      )}
    </div>
  );
}
