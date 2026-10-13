import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { accounts } from './api.js';

const AuthContext = createContext(null);
const KEY = 'pavadinimas.user';

function load() {
  try {
    return JSON.parse(localStorage.getItem(KEY)) ?? null;
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

  const save = useCallback((u) => {
    setUser(u);
    persist(u);
  }, []);

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
      await accounts.login({ username, password });
      const displayName = await fetchDisplayName(username);
      save({ username, displayName });
    },
    [save]
  );

  const register = useCallback(
    async (displayName, username, password) => {
      await accounts.register({ displayName, username, password });
      save({ username, displayName });
    },
    [save]
  );

  const logout = useCallback(() => save(null), [save]);

  const setDisplayName = useCallback(
    (displayName) => save(user ? { ...user, displayName } : null),
    [save, user]
  );

  return (
    <AuthContext.Provider value={{ user, login, register, logout, setDisplayName }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
