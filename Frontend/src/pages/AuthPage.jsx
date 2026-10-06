import { useEffect, useState } from 'react';
import { useAuth } from '../auth.jsx';
import { Button, Segmented, TextField } from '../components/ui.jsx';
import { useToast } from '../components/Toast.jsx';
import { navigate } from '../router.js';

function validateRegister({ displayName, username, password, confirm }) {
  const e = {};
  if (displayName.trim().length < 3 || displayName.trim().length > 20) e.displayName = '3–20 simbolių.';
  if (username.trim().length < 3 || username.trim().length > 20) e.username = '3–20 simbolių.';
  if (password.length < 8 || password.length > 30) e.password = '8–30 simbolių.';
  if (confirm !== password) e.confirm = 'Slaptažodžiai nesutampa.';
  return e;
}

export default function AuthPage({ mode: initialMode = 'login', next }) {
  const { user, login, register } = useAuth();
  const toast = useToast();
  const [mode, setMode] = useState(initialMode === 'register' ? 'register' : 'login');
  const [form, setForm] = useState({ displayName: '', username: '', password: '', confirm: '' });
  const [errors, setErrors] = useState({});
  const [formError, setFormError] = useState(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (user) navigate(next || '/games');
  }, [user, next]);

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));

  async function handleSubmit(e) {
    e.preventDefault();
    setFormError(null);

    if (mode === 'register') {
      const errs = validateRegister(form);
      setErrors(errs);
      if (Object.keys(errs).length) return;
    } else {
      setErrors({});
    }

    setBusy(true);
    try {
      if (mode === 'register') {
        await register(form.displayName.trim(), form.username.trim(), form.password);
        toast('Paskyra sukurta', 'success');
      } else {
        await login(form.username.trim(), form.password);
      }
    } catch (err) {
      setFormError(err.message);
      setBusy(false);
    }
  }

  return (
    <div className="page auth-page">
      <div className="auth-card">
        <h1 className="title">{mode === 'login' ? 'Prisijungti' : 'Sukurti paskyrą'}</h1>
        <p className="subtitle">
          {mode === 'login' ? 'Sveiki sugrįžę į PAVADINIMAS.' : 'Organizuokite žaidimus ir prisijunkite prie kitų.'}
        </p>

        <Segmented
          options={[
            { value: 'login', label: 'Prisijungti' },
            { value: 'register', label: 'Registruotis' },
          ]}
          value={mode}
          onChange={(m) => {
            setMode(m);
            setErrors({});
            setFormError(null);
          }}
          label="Režimas"
        />

        <form className="form" onSubmit={handleSubmit} noValidate>
          {mode === 'register' && (
            <TextField
              label="Rodomas vardas"
              autoComplete="name"
              value={form.displayName}
              onChange={set('displayName')}
              error={errors.displayName}
              maxLength={20}
            />
          )}
          <TextField
            label="Vartotojo vardas"
            autoComplete="username"
            autoCapitalize="none"
            value={form.username}
            onChange={set('username')}
            error={errors.username}
            maxLength={20}
            required
          />
          <TextField
            label="Slaptažodis"
            type="password"
            autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
            value={form.password}
            onChange={set('password')}
            error={errors.password}
            hint={mode === 'register' ? 'Bent 8 simboliai.' : undefined}
            maxLength={30}
            required
          />
          {mode === 'register' && (
            <TextField
              label="Pakartokite slaptažodį"
              type="password"
              autoComplete="new-password"
              value={form.confirm}
              onChange={set('confirm')}
              error={errors.confirm}
              maxLength={30}
            />
          )}

          {formError && <div className="note note-error">{formError}</div>}

          <Button type="submit" size="lg" block loading={busy}>
            {mode === 'login' ? 'Prisijungti' : 'Registruotis'}
          </Button>
        </form>
      </div>
    </div>
  );
}
