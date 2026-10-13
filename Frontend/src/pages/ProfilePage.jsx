import { useEffect, useMemo, useState } from 'react';
import { accounts, games as gamesApi } from '../api.js';
import { useAuth } from '../auth.jsx';
import { initials, isParticipant, parseDate, sameUser } from '../format.js';
import { Button, Group, Row, TextField } from '../components/ui.jsx';
import { useToast } from '../components/Toast.jsx';
import { navigate } from '../router.js';

export default function ProfilePage() {
  const { user, logout, setDisplayName } = useAuth();
  const toast = useToast();
  const [open, setOpen] = useState(null);
  const [games, setGames] = useState(null);

  useEffect(() => {
    if (!user) navigate('/login?next=/profile');
  }, [user]);

  useEffect(() => {
    gamesApi.getAll().then(setGames, () => setGames([]));
  }, []);

  const stats = useMemo(() => {
    if (!games || !user) return null;
    const mine = games.filter((g) => isParticipant(g, user.username));
    const now = Date.now();
    return {
      organized: games.filter((g) => sameUser(g.createdByUsername, user.username)).length,
      joined: games.filter((g) => (g.joinedUsernames ?? []).some((u) => sameUser(u, user.username))).length,
      upcoming: mine.filter((g) => parseDate(g.startTime) > now).length,
    };
  }, [games, user]);

  if (!user) return null;

  const toggle = (k) => setOpen((o) => (o === k ? null : k));

  return (
    <div className="page page-narrow">
      <div className="profile-head">
        <span className="avatar avatar-lg">{initials(user.displayName)}</span>
        <h1 className="title">{user.displayName}</h1>
        <p className="muted">@{user.username}</p>
      </div>

      <div className="stats">
        <Stat value={stats?.upcoming} label="Būsimi" />
        <Stat value={stats?.organized} label="Organizuoti" />
        <Stat value={stats?.joined} label="Dalyvauta" />
      </div>

      <Group title="Paskyra">
        <Row label="Rodomas vardas" value={user.displayName} onClick={() => toggle('name')} />
        {open === 'name' && (
          <ChangeName
            user={user}
            onDone={(name) => {
              setDisplayName(name);
              setOpen(null);
              toast('Vardas pakeistas', 'success');
            }}
          />
        )}
        <Row label="Slaptažodis" value="••••••••" onClick={() => toggle('password')} />
        {open === 'password' && (
          <ChangePassword
            user={user}
            onDone={() => {
              setOpen(null);
              toast('Slaptažodis pakeistas', 'success');
            }}
          />
        )}
      </Group>

      <Group>
        <Row label="Atsijungti" accent onClick={logout} />
      </Group>

      <Group footer="Paskyra ir visi jos duomenys bus ištrinti visam laikui.">
        <Row label="Ištrinti paskyrą" destructive onClick={() => toggle('delete')} />
        {open === 'delete' && (
          <DeleteAccount
            user={user}
            onDone={() => {
              logout();
              toast('Paskyra ištrinta');
            }}
          />
        )}
      </Group>
    </div>
  );
}

function Stat({ value, label }) {
  return (
    <div className="stat">
      <span className="stat-value">{value ?? '–'}</span>
      <span className="stat-label">{label}</span>
    </div>
  );
}

function useSubmit(fn) {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState(null);
  async function run(e) {
    e.preventDefault();
    setError(null);
    setBusy(true);
    try {
      await fn();
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  }
  return { busy, error, setError, run };
}

function ChangeName({ user, onDone }) {
  const [name, setName] = useState(user.displayName);
  const [password, setPassword] = useState('');
  const { busy, error, setError, run } = useSubmit(async () => {
    const newName = name.trim();
    if (newName.length < 3 || newName.length > 20) return setError('Vardas turi būti 3–20 simbolių.');
    await accounts.changeName({ username: user.username, password, newName });
    onDone(newName);
  });
  return (
    <form className="inline-form" onSubmit={run}>
      <TextField label="Naujas vardas" value={name} onChange={(e) => setName(e.target.value)} maxLength={20} />
      <TextField
        label="Slaptažodis"
        type="password"
        autoComplete="current-password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        hint="Patvirtinkite pakeitimą slaptažodžiu."
        required
      />
      {error && <div className="note note-error">{error}</div>}
      <Button type="submit" loading={busy}>
        Išsaugoti
      </Button>
    </form>
  );
}

function ChangePassword({ user, onDone }) {
  const [oldPassword, setOld] = useState('');
  const [newPassword, setNew] = useState('');
  const [confirm, setConfirm] = useState('');
  const { busy, error, setError, run } = useSubmit(async () => {
    if (newPassword.length < 8 || newPassword.length > 30) return setError('Naujas slaptažodis turi būti 8–30 simbolių.');
    if (newPassword !== confirm) return setError('Slaptažodžiai nesutampa.');
    await accounts.changePassword({ username: user.username, oldPassword, newPassword });
    onDone();
  });
  return (
    <form className="inline-form" onSubmit={run}>
      <TextField label="Dabartinis slaptažodis" type="password" autoComplete="current-password" value={oldPassword} onChange={(e) => setOld(e.target.value)} required />
      <TextField label="Naujas slaptažodis" type="password" autoComplete="new-password" value={newPassword} onChange={(e) => setNew(e.target.value)} maxLength={30} required />
      <TextField label="Pakartokite naują" type="password" autoComplete="new-password" value={confirm} onChange={(e) => setConfirm(e.target.value)} maxLength={30} required />
      {error && <div className="note note-error">{error}</div>}
      <Button type="submit" loading={busy}>
        Pakeisti slaptažodį
      </Button>
    </form>
  );
}

function DeleteAccount({ user, onDone }) {
  const [password, setPassword] = useState('');
  const { busy, error, run } = useSubmit(async () => {
    await accounts.remove({ username: user.username, password });
    onDone();
  });
  return (
    <form className="inline-form" onSubmit={run}>
      <TextField
        label="Slaptažodis"
        type="password"
        autoComplete="current-password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      {error && <div className="note note-error">{error}</div>}
      <Button type="submit" variant="danger" loading={busy}>
        Ištrinti visam laikui
      </Button>
    </form>
  );
}
