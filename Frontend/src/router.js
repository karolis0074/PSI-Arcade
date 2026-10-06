import { useEffect, useState } from 'react';

function parse() {
  const raw = window.location.hash.replace(/^#/, '') || '/games';
  const [path, qs = ''] = raw.split('?');
  const segments = path.split('/').filter(Boolean);
  return { path, segments, query: Object.fromEntries(new URLSearchParams(qs)) };
}

export function useRoute() {
  const [route, setRoute] = useState(parse);
  useEffect(() => {
    const onChange = () => {
      setRoute(parse());
      window.scrollTo(0, 0);
    };
    window.addEventListener('hashchange', onChange);
    return () => window.removeEventListener('hashchange', onChange);
  }, []);
  return route;
}

export function navigate(to) {
  window.location.hash = to;
}
