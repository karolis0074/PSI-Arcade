const BASE = '/api';

let token = null;
let onSessionExpired = () => {};

// issaugo jwt tokena, kuri pridesim prie uzklausu
export function setToken(value) {
  token = value || null;
}

// nustato funkcija, kuri kvieciama kai sesija baigiasi
export function setSessionExpiredHandler(fn) {
  onSessionExpired = fn;
}

// istraukia jwt tokeno duomenis (payload)
function decodeToken(value) {
  return JSON.parse(atob(value.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
}

// grazina vartotojo varda is jwt tokeno (taip kaip jis issaugotas backend)
export function usernameFromToken(value) {
  try {
    const payload = decodeToken(value);
    return (
      payload.unique_name ??
      payload.name ??
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      null
    );
  } catch {
    return null;
  }
}

// patikrina ar jwt tokeno galiojimo laikas (exp) jau praejo
export function isTokenExpired(value) {
  if (!value) return true;
  try {
    const payload = decodeToken(value);
    return typeof payload.exp === 'number' && payload.exp * 1000 <= Date.now();
  } catch {
    return true;
  }
}

const SESSION_EXPIRED = 'Sesija baigėsi. Prisijunkite iš naujo.';

const BACKEND_MESSAGES = {
  'StartTime must be in the future': 'Žaidimo pradžia turi būti ateityje.',
  'MaxPlayers must be greater than 0': 'Žaidėjų skaičius turi būti didesnis už 0.',
  'Football field not found': 'Aikštelė nerasta.',
  'Sport field not found': 'Aikštelė nerasta.',
  'Game not found': 'Žaidimas nerastas.',
  'Game has already started': 'Žaidimas jau prasidėjo.',
  'User already joined this game': 'Jūs jau dalyvaujate šiame žaidime.',
  'Game is full': 'Žaidimas pilnas.',
  'User is not in this game': 'Jūs nedalyvaujate šiame žaidime.',
  'Host cannot leave the game': 'Šeimininkas negali išeiti iš savo žaidimo.',
};

// isvercia backend pranesimus su kintama dalimi (pvz. aikstes talpa)
function translateDynamic(text) {
  const capacity = text.match(/^MaxPlayers cannot exceed the field capacity \((\d+)\)$/);
  if (capacity) return `Žaidėjų skaičius negali viršyti aikštelės talpos (${capacity[1]}).`;
  return null;
}

// klaida su http statusu ir pranesimu vartotojui
export class ApiError extends Error {
  constructor(status, data, message, sessionExpired = false) {
    super(message ?? describeError(status, data));
    this.status = status;
    this.data = data;
    this.sessionExpired = sessionExpired;
  }
}

// pavercia backend atsakyma i suprantama klaidos teksta
function describeError(status, data) {
  if (typeof data === 'string' && data.trim()) {
    const text = data.trim();
    return BACKEND_MESSAGES[text] ?? translateDynamic(text) ?? 'Įvyko klaida.';
  }
  if (data && typeof data === 'object' && data.errors) return 'Neteisingi duomenys.';
  switch (status) {
    case 400: return 'Neteisingi duomenys.';
    case 401: return 'Neteisingas vartotojo vardas arba slaptažodis.';
    case 403: return 'Neturite teisės atlikti šio veiksmo.';
    case 404: return 'Nerasta.';
    case 409: return 'Toks vartotojas jau egzistuoja.';
    default: return 'Įvyko klaida.';
  }
}

// isvalo tokena, pranesa apie pasibaigusia sesija ir grazina klaida
function sessionExpired() {
  setToken(null);
  onSessionExpired();
  return new ApiError(401, null, SESSION_EXPIRED, true);
}

// pagrindine funkcija: siuncia uzklausa i backend ir grazina atsakymo duomenis
async function request(path, { method = 'GET', body, query, auth = false, messages = {} } = {}) {
  if (auth && isTokenExpired(token)) throw sessionExpired();

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
      headers: {
        ...(body ? { 'Content-Type': 'application/json' } : {}),
        ...(auth ? { Authorization: `Bearer ${token}` } : {}),
      },
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
  if (res.status === 401 && auth && (res.headers.get('WWW-Authenticate') ?? '').includes('invalid_token')) {
    throw sessionExpired();
  }
  if (!res.ok) throw new ApiError(res.status, data, messages[res.status]);
  return data;
}

// paskyros endpointai (/accounts)
export const accounts = {
  // grazina vartotojo rodoma varda
  getName: (username) => request('/accounts/getname', { query: { username } }),
  // sukuria nauja paskyra
  register: ({ displayName, username, password }) =>
    request('/accounts/register', { method: 'POST', body: { displayName, username, password } }),
  // prisijungia ir grazina jwt tokena
  login: ({ username, password }) =>
    request('/accounts/login', { method: 'POST', body: { username, password } }),
  // pakeicia prisijungusio vartotojo rodoma varda
  changeName: ({ newName }) =>
    request('/accounts/changename', { method: 'POST', body: { newName }, auth: true }),
  // pakeicia slaptazodi, reikia senojo slaptazodzio
  changePassword: ({ oldPassword, newPassword }) =>
    request('/accounts/changepass', {
      method: 'POST',
      body: { oldPassword, newPassword },
      auth: true,
      messages: { 401: 'Neteisingas dabartinis slaptažodis.' },
    }),
  // istrina prisijungusio vartotojo paskyra
  remove: () => request('/accounts/delete', { method: 'DELETE', auth: true }),
};

// zaidimu endpointai (/games)
export const games = {
  // grazina visus zaidimus
  getAll: () => request('/games/getall'),
  // grazina viena zaidima pagal id
  get: (id) => request('/games/get', { query: { id } }),
  // sukuria nauja zaidima, kurejas tampa seimininku ir automatiskai prisijungia
  create: ({ footballFieldId, startTime, maxPlayers }) =>
    request('/games/create', {
      method: 'POST',
      body: { footballFieldId, startTime, maxPlayers },
      auth: true,
    }),
  // istrina zaidima, gali tik seimininkas
  remove: (id) =>
    request('/games/delete', {
      method: 'DELETE',
      query: { id },
      auth: true,
      messages: { 403: 'Žaidimą gali ištrinti tik jo šeimininkas.' },
    }),
  // prisijungia prie zaidimo
  join: (id, username) => request('/games/join', { method: 'POST', query: { id, username } }),
  // iseina is zaidimo, seimininkas iseiti negali
  leave: (id, username) => request('/games/leave', { method: 'POST', query: { id, username } }),
};

// aikstelu endpointai (/sportfields)
export const sportFields = {
  // grazina visas aiksteles
  getAll: () => request('/sportfields/getall'),
  // grazina viena aikstele pagal id
  get: (id) => request('/sportfields/get', { query: { id } }),
  // iesko aikstelu pagal miesta, sporto saka ir dydi
  search: ({ city, gameType, fieldType }) =>
    request('/sportfields/search', { query: { city, gameType, fieldType } }),
};
