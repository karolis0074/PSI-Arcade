const base = {
  width: 22,
  height: 22,
  viewBox: '0 0 24 24',
  fill: 'none',
  stroke: 'currentColor',
  strokeWidth: 1.8,
  strokeLinecap: 'round',
  strokeLinejoin: 'round',
  'aria-hidden': true,
};

export const IconCalendar = (p) => (
  <svg {...base} {...p}>
    <rect x="3.5" y="5" width="17" height="15.5" rx="3" />
    <path d="M3.5 10h17M8 3v4M16 3v4" />
  </svg>
);

export const IconPin = (p) => (
  <svg {...base} {...p}>
    <path d="M12 21s-6.5-5.6-6.5-11a6.5 6.5 0 0 1 13 0C18.5 15.4 12 21 12 21Z" />
    <circle cx="12" cy="10" r="2.3" />
  </svg>
);

export const IconPerson = (p) => (
  <svg {...base} {...p}>
    <circle cx="12" cy="8" r="3.8" />
    <path d="M4.5 20.5c1.2-3.8 4-5.6 7.5-5.6s6.3 1.8 7.5 5.6" />
  </svg>
);

export const IconPlus = (p) => (
  <svg {...base} {...p}>
    <path d="M12 5v14M5 12h14" />
  </svg>
);

export const IconChevronRight = (p) => (
  <svg {...base} width={14} height={14} strokeWidth={2.2} {...p}>
    <path d="m9 5 7 7-7 7" />
  </svg>
);

export const IconChevronLeft = (p) => (
  <svg {...base} width={18} height={18} strokeWidth={2.2} {...p}>
    <path d="m15 5-7 7 7 7" />
  </svg>
);

export const IconSearch = (p) => (
  <svg {...base} width={17} height={17} {...p}>
    <circle cx="11" cy="11" r="6.5" />
    <path d="m20 20-4.2-4.2" />
  </svg>
);

export const IconClock = (p) => (
  <svg {...base} width={16} height={16} {...p}>
    <circle cx="12" cy="12" r="8.5" />
    <path d="M12 7.5V12l3 2" />
  </svg>
);

export const IconMinus = (p) => (
  <svg {...base} width={18} height={18} strokeWidth={2.2} {...p}>
    <path d="M6 12h12" />
  </svg>
);

export const IconCheck = (p) => (
  <svg {...base} width={18} height={18} strokeWidth={2.4} {...p}>
    <path d="m5 12.5 4.5 4.5L19 7.5" />
  </svg>
);
