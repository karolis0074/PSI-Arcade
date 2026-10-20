const STATUS_KEYS = ['Open', 'AlmostFull', 'Full', 'Confirmed', 'InProgress', 'Completed'];

const STATUS = {
  Open: { label: 'Atviras', tone: 'green' },
  AlmostFull: { label: 'Beveik pilnas', tone: 'orange' },
  Full: { label: 'Pilnas', tone: 'red' },
  Confirmed: { label: 'Patvirtintas', tone: 'blue' },
  InProgress: { label: 'Vyksta', tone: 'purple' },
  Completed: { label: 'Baigtas', tone: 'gray' },
};

export function statusKey(status) {
  if (typeof status === 'number') return STATUS_KEYS[status] ?? 'Open';
  return STATUS_KEYS.includes(status) ? status : 'Open';
}

export function statusInfo(status) {
  return STATUS[statusKey(status)];
}

const GAME_TYPES = {
  Football: 'Futbolas',
  Basketball: 'Krepšinis',
  Volleyball: 'Tinklinis',
  Tennis: 'Tenisas',
  Padel: 'Padelis',
  Badminton: 'Badmintonas',
  Kvadratas: 'Kvadratas',
};
export const gameTypeLabel = (t) => GAME_TYPES[t] ?? t;

const FIELD_TYPES = { S: 'Maža', M: 'Vidutinė', L: 'Didelė' };
export const fieldTypeLabel = (t) => FIELD_TYPES[t] ?? t;

const SURFACES = {
  Asphalt: 'Asfaltas',
  Artificial: 'Dirbtinė danga',
  Sand: 'Smėlis',
  Carpet: 'Kilimas',
  'Natural Grass': 'Natūrali žolė',
};
export const surfaceLabel = (s) => (s ? SURFACES[s] ?? s : null);

export function parseDate(value) {
  if (!value) return null;
  const s = String(value);
  const hasZone = /[zZ]|[+-]\d\d:?\d\d$/.test(s);
  return new Date(hasZone ? s : s + 'Z');
}

const weekdayFmt = new Intl.DateTimeFormat('lt-LT', { weekday: 'long' });
const longFmt = new Intl.DateTimeFormat('lt-LT', { weekday: 'long', month: 'long', day: 'numeric' });
const timeFmt = new Intl.DateTimeFormat('lt-LT', { hour: '2-digit', minute: '2-digit' });

export const fmtDay = (d) => String(d.getDate());
const MONTHS = ['Saus', 'Vas', 'Kov', 'Bal', 'Geg', 'Birž', 'Liep', 'Rugp', 'Rugs', 'Spal', 'Lapkr', 'Gruod'];
export const fmtMonth = (d) => MONTHS[d.getMonth()];
export const fmtWeekday = (d) => capitalize(weekdayFmt.format(d));
export const fmtLong = (d) => capitalize(longFmt.format(d));
export const fmtTime = (d) => timeFmt.format(d);

export function fmtRelative(d) {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const that = new Date(d);
  that.setHours(0, 0, 0, 0);
  const diff = Math.round((that - today) / 86400000);
  if (diff === 0) return 'Šiandien';
  if (diff === 1) return 'Rytoj';
  if (diff === -1) return 'Vakar';
  return fmtWeekday(d);
}

function capitalize(s) {
  return s.charAt(0).toUpperCase() + s.slice(1);
}

export function initials(name = '') {
  return name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase() ?? '')
    .join('') || '?';
}

export function mapUrl(field) {
  if (!field?.address) return null;
  const m = field.address.match(/^\s*(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)\s*$/);
  if (m) return `https://maps.apple.com/?ll=${m[1]},${m[2]}&q=${encodeURIComponent(field.name)}`;
  return `https://maps.apple.com/?q=${encodeURIComponent(field.address + ', ' + field.city)}`;
}

export function isCoordinates(address) {
  return /^\s*-?\d+(\.\d+)?\s*,\s*-?\d+(\.\d+)?\s*$/.test(address ?? '');
}

export function sameUser(a, b) {
  return (a ?? '').toLowerCase() === (b ?? '').toLowerCase();
}

export function isParticipant(game, username) {
  if (!username) return false;
  return (
    sameUser(game.createdByUsername, username) ||
    (game.joinedUsernames ?? []).some((u) => sameUser(u, username))
  );
}
