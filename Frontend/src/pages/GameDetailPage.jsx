import { useCallback, useEffect, useState } from 'react';
import { games as gamesApi } from '../api.js';
import { useAuth } from '../auth.jsx';
import {
  fieldTypeLabel,
  fmtLong,
  fmtTime,
  gameTypeLabel,
  initials,
  isCoordinates,
  mapUrl,
  parseDate,
  sameUser,
  surfaceLabel,
} from '../format.js';
import { useFields } from '../useFields.js';
import { Button, Capacity, EmptyState, ErrorNote, Group, Row, Spinner, StatusPill } from '../components/ui.jsx';
import { IconChevronLeft, IconClock } from '../components/Icons.jsx';
import { useToast } from '../components/Toast.jsx';
import { navigate } from '../router.js';

export default function GameDetailPage({ id }) {
  const { user } = useAuth();
  const { byId } = useFields();
  const toast = useToast();
  const [game, setGame] = useState(null);
  const [error, setError] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    try {
      setGame(await gamesApi.get(id));
      setError(null);
    } catch (err) {
      setError(err);
    }
  }, [id]);

  useEffect(() => {
    load();
  }, [load]);

  async function handleDelete() {
    setDeleting(true);
    try {
      await gamesApi.remove(id);
      toast('Žaidimas ištrintas');
      navigate('/games');
    } catch (err) {
      toast(err.message, 'error');
      setDeleting(false);
    }
  }

  const back = (
    <a href="#/games" className="back-link">
      <IconChevronLeft /> Žaidimai
    </a>
  );

  if (error?.status === 404)
    return (
      <div className="page">
        {back}
        <EmptyState title="Žaidimas nerastas" text="Galbūt jis buvo ištrintas." />
      </div>
    );

  if (!game)
    return (
      <div className="page">
        {back}
        <ErrorNote error={error} onRetry={load} />
        {!error && <Spinner />}
      </div>
    );

  const field = byId.get(game.footballFieldId);
  const start = parseDate(game.startTime);
  const players = game.joinedUsernames ?? [];
  const isOwner = user && sameUser(user.username, game.createdByUsername);
  const map = mapUrl(field);

  return (
    <div className="page page-narrow">
      {back}

      <div className="detail-hero">
        <StatusPill status={game.status} />
        <h1 className="large-title">{field?.name ?? `Aikštelė #${game.footballFieldId}`}</h1>
        <p className="subtitle detail-when">
          <IconClock /> {fmtLong(start)}, {fmtTime(start)}
        </p>
      </div>

      <div className="panel">
        <Capacity joined={players.length} max={game.maxPlayers} />
      </div>

      <Group title="Informacija">
        <Row label="Organizatorius" value={game.createdByUsername || '—'} />
        <Row label="Data" value={fmtLong(start)} />
        <Row label="Pradžia" value={fmtTime(start)} />
        <Row label="Maks. žaidėjų" value={game.maxPlayers} />
      </Group>

      <Group title="Aikštelė">
        {field ? (
          <>
            <Row label="Sporto šaka" value={gameTypeLabel(field.gameType)} />
            <Row label="Dydis" value={fieldTypeLabel(field.fieldType)} />
            {field.surface && <Row label="Danga" value={surfaceLabel(field.surface)} />}
            <Row label="Miestas" value={field.city} />
            {!isCoordinates(field.address) && <Row label="Adresas" value={field.address} />}
            {map && <Row label="Rodyti žemėlapyje" href={map} accent />}
          </>
        ) : (
          <Row label="Aikštelės ID" value={game.footballFieldId} />
        )}
      </Group>

      <Group title={`Dalyviai · ${players.length}`}>
        {players.length === 0 ? (
          <div className="row">
            <span className="muted">Dar niekas neprisijungė.</span>
          </div>
        ) : (
          players.map((u) => (
            <div className="row" key={u}>
              <span className="avatar avatar-sm">{initials(u)}</span>
              <span className="row-label">{u}</span>
              {user && sameUser(u, user.username) && <span className="row-value">Jūs</span>}
            </div>
          ))
        )}
      </Group>

      {isOwner && (
        <div className="danger-zone">
          {confirmDelete ? (
            <div className="confirm">
              <p>Ištrinti šį žaidimą? Šio veiksmo atšaukti negalima.</p>
              <div className="confirm-actions">
                <Button variant="secondary" onClick={() => setConfirmDelete(false)}>
                  Atšaukti
                </Button>
                <Button variant="danger" loading={deleting} onClick={handleDelete}>
                  Ištrinti
                </Button>
              </div>
            </div>
          ) : (
            <Button variant="plain-danger" block onClick={() => setConfirmDelete(true)}>
              Ištrinti žaidimą
            </Button>
          )}
        </div>
      )}
    </div>
  );
}
