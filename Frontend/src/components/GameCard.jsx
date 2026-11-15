import { fmtDay, fmtMonth, fmtRelative, fmtTime, gameTypeLabel, parseDate } from '../format.js';
import { Capacity, StatusPill } from './ui.jsx';
import { IconChevronRight } from './Icons.jsx';

export default function GameCard({ game, field }) {
  const start = parseDate(game.startTime);
  const joined = game.joinedUsernames?.length ?? 0;
  const title = field?.name ?? `Aikštelė #${game.footballFieldId}`;
  const meta = [field && gameTypeLabel(field.gameType), `org. ${game.createdByUsername || '—'}`]
    .filter(Boolean)
    .join(' · ');

  return (
    <a href={`#/games/${game.id}`} className="game-card">
      <div className="date-tile">
        <span className="date-month">{fmtMonth(start)}</span>
        <span className="date-day">{fmtDay(start)}</span>
      </div>
      <div className="game-card-body">
        <div className="game-card-top">
          <span className="eyebrow">
            {fmtRelative(start)}, {fmtTime(start)}
          </span>
          <StatusPill status={game.status} />
        </div>
        <h3 className="game-card-title">{title}</h3>
        <p className="game-card-meta">{meta}</p>
        <Capacity joined={joined} max={game.maxPlayers} compact />
      </div>
      <IconChevronRight className="game-card-chevron" />
    </a>
  );
}
