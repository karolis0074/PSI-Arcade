const BASE = '/api';

export class ApiError extends Error {
  constructor(status, data) {
    super(describeError(status, data));
    this.status = status;
    this.data = data;
  }
}

function describeError(status, data) {
  if (typeof data === 'string' && data.trim()) return data;
  if (data && typeof data === 'object') {
    if (data.errors) {
      const first = Object.values(data.errors).flat()[0];
      if (first) return first;
    }
    if (data.title) return data.title;
  }
  switch (status) {
    case 0: return 'Įvyko klaida.';
    case 400: return 'Neteisingi duomenys.';
    case 401: return 'Neteisingas vartotojo vardas arba slaptažodis.';
    case 409: return 'Toks vartotojas jau egzistuoja.';
    default: return 'Įvyko klaida.';
  }
}

async function request(path, { method = 'GET', body, query } = {}) {
  let url = BASE + path;
  if (query) {
    const params = new URLSearchParams();
    for (const [k, v] of Object.entries(query)) {
      if (v !== undefined && v !== null && v !== '') params.set(k, v);
    }
    const qs = params.toString();
    if (qs) url += '?' + qs;
  }

  let res;
  try {
    res = await fetch(url, {
      method,
      headers: body ? { 'Content-Type': 'application/json' } : undefined,
      body: body ? JSON.stringify(body) : undefined,
    });
  } catch {
    throw new ApiError(0, null);
  }

  const text = await res.text();
  let data = null;
  if (text) {
    try { data = JSON.parse(text); } catch { data = text; }
  }
  if (!res.ok && (res.status === 502 || res.status === 504 || (res.status === 500 && !text))) {
    throw new ApiError(0, null);
  }
  if (!res.ok) throw new ApiError(res.status, data);
  return data;
}

export const accounts = {
  getName: (username) => request('/Accounts/getname', { query: { username } }),
  register: ({ displayName, username, password }) =>
    request('/Accounts/register', { method: 'POST', body: { displayName, username, password } }),
  login: ({ username, password }) =>
    request('/Accounts/login', { method: 'POST', body: { username, password } }),
  changeName: ({ username, password, newName }) =>
    request('/Accounts/changename', { method: 'POST', body: { username, password, newName } }),
  changePassword: ({ username, oldPassword, newPassword }) =>
    request('/Accounts/changepass', { method: 'POST', body: { username, oldPassword, newPassword } }),
  remove: ({ username, password }) =>
    request('/Accounts/delete', { method: 'DELETE', body: { username, password } }),
};

export const games = {
  getAll: () => request('/games/getall'),
  get: (id) => request('/games/get', { query: { id } }),
  create: ({ footballFieldId, createdByUsername, startTime, maxPlayers }) =>
    request('/games/create', {
      method: 'POST',
      body: { footballFieldId, createdByUsername, startTime, maxPlayers },
    }),
  remove: (id) => request('/games/delete', { method: 'DELETE', query: { id } }),
};

export const sportFields = {
  getAll: () => request('/sportfields/getall'),
  get: (id) => request('/sportfields/get', { query: { id } }),
  search: ({ city, gameType, fieldType }) =>
    request('/sportfields/search', { query: { city, gameType, fieldType } }),
};
