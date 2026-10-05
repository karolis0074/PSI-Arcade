import { useEffect, useState } from 'react';
import { sportFields } from './api.js';

let cache = null;
let pending = null;

function loadFields() {
  if (cache) return Promise.resolve(cache);
  if (!pending) {
    pending = sportFields
      .getAll()
      .then((list) => {
        const fields = Array.isArray(list) ? list : [];
        cache = { available: true, fields, byId: new Map(fields.map((f) => [f.id, f])) };
        return cache;
      })
      .catch((err) => {
        pending = null;
        if (err.status === 404) {
          cache = { available: false, fields: [], byId: new Map() };
          return cache;
        }
        throw err;
      });
  }
  return pending;
}

export function useFields() {
  const [state, setState] = useState(
    cache ? { ...cache, loading: false } : { available: null, fields: [], byId: new Map(), loading: true }
  );

  useEffect(() => {
    let alive = true;
    loadFields().then(
      (c) => alive && setState({ ...c, loading: false }),
      (err) => alive && setState({ available: null, fields: [], byId: new Map(), loading: false, error: err })
    );
    return () => {
      alive = false;
    };
  }, []);

  return state;
}
