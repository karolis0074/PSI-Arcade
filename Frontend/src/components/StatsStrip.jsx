import { useCallback, useEffect, useState } from 'react';
import { games as gamesApi, metrics } from '../api.js';
import { useFields } from '../useFields.js';

const REFRESH_MS = 30000;

export default function StatsStrip({ upcoming }) {
  const { byId } = useFields();
  const [stats, setStats] = useState(null);
  const [activeUsers, setActiveUsers] = useState(null);

  const load = useCallback(async () => {
    const [s, l] = await Promise.allSettled([gamesApi.statistics(), metrics.activeUsers()]);
    setStats(s.status === 'fulfilled' ? s.value : null);
    setActiveUsers(l.status === 'fulfilled' ? l.value?.activeUsers ?? 0 : null);
  }, []);

  useEffect(() => {
    load();
    const t = setInterval(load, REFRESH_MS);
    return () => clearInterval(t);
  }, [load]);

  const average = stats ? Number(stats.averagePlayers).toLocaleString('lt-LT', { maximumFractionDigits: 1 }) : null;
  const popular = stats?.mostPopularFieldId != null ? byId.get(stats.mostPopularFieldId)?.name : null;

  return (
    <div className="stats stats-overview">
      <div className="stat">
        <span className="stat-value">
          <span className="live-dot" aria-hidden />
          {activeUsers ?? '–'}
        </span>
        <span className="stat-label">Prisijungę dabar</span>
      </div>
      <div className="stat">
        <span className="stat-value">{upcoming ?? '–'}</span>
        <span className="stat-label">Būsimų žaidimų</span>
      </div>
      <div className="stat">
        <span className="stat-value">{average ?? '–'}</span>
        <span className="stat-label">Vid. žaidėjų</span>
      </div>
      <div className="stat">
        <span className="stat-value stat-text" title={popular ?? undefined}>
          {popular ?? '–'}
        </span>
        <span className="stat-label">Populiariausia aikštelė</span>
      </div>
    </div>
  );
}
