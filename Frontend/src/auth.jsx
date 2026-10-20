import { createContext, useCallback, useContext, useEffect, useRef, useState } from 'react';
import { accounts, isTokenExpired, setSessionExpiredHandler, setToken } from './api.js';
import { useToast } from './components/Toast.jsx';
import { navigate } from './router.js';

const AuthContext = createContext(null);
const KEY = 'pavadinimas.user';

function load() {
  try {
    const user = JSON.parse(localStorage.getItem(KEY)) ?? null;
    if (!user?.token || isTokenExpired(user.token)) return null;
    setToken(user.token);
    return user;
  } catch {
    return null;
  }
}

function persist(user) {
  try {
    if (user) localStorage.setItem(KEY, JSON.stringify(user));
    else localStorage.removeItem(KEY);
  } catch {
  }
}

async function fetchDisplayName(username) {
  try {
    const res = await accounts.getName(username);
    return res?.displayName ?? res?.DisplayName ?? username;
  } catch {
    return username;
  }
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(load);
  const toast = useToast();
  const userRef = useRef(user);

  const save = useCallback((u) => {
    userRef.current = u;
    setToken(u?.token ?? null);
    setUser(u);
    persist(u);
  }, []);

  useEffect(() => {
    setSessionExpiredHandler(() => {
      if (!userRef.current) return;
      save(null);
      toast('Sesija baigėsi. Prisijunkite iš naujo.', 'error');
      const current = window.location.hash.replace(/^#/, '') || '/games';
      navigate(`/login?next=${encodeURIComponent(current)}`);
    });
  }, [save, toast]);

  useEffect(() => {
    if (!user) return;
    accounts.getName(user.username).then(
      (res) => {
        const displayName = res?.displayName ?? user.displayName;
        if (displayName !== user.displayName) save({ ...user, displayName });
      },
      (err) => {
        if (err.status === 404) save(null);
      }
    );
  }, []);

  const login = useCallback(
    async (username, password) => {
      const res = await accounts.login({ username, password });
      const token = res?.token ?? res?.Token;
      if (!token) throw new Error('Įvyko klaida.');
      setToken(token);
      const displayName = await fetchDisplayName(username);
      save({ username, displayName, token });
    },
    [save]
  );

  const register = useCallback(
    async (displayName, username, password) => {
      await accounts.register({ displayName, username, password });
      await login(username, password);
    },
    [login]
  );

  const logout = useCallback(() => {
    save(null);
    navigate('/games');
  }, [save]);

  const setDisplayName = useCallback(
    (displayName) => {
      const u = userRef.current;
      save(u ? { ...u, displayName } : null);
    },
    [save]
  );

  return (
    <AuthContext.Provider value={{ user, login, register, logout, setDisplayName }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
