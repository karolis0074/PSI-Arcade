import { useId } from 'react';
import { statusInfo } from '../format.js';
import { IconChevronRight, IconMinus, IconPlus } from './Icons.jsx';

export function Button({ variant = 'primary', size, block, loading, children, className = '', ...rest }) {
  const cls = ['btn', `btn-${variant}`, size && `btn-${size}`, block && 'btn-block', className]
    .filter(Boolean)
    .join(' ');
  return (
    <button className={cls} disabled={loading || rest.disabled} {...rest}>
      {loading ? <span className="spinner spinner-sm" aria-label="Kraunama" /> : children}
    </button>
  );
}

export function TextField({ label, hint, error, ...rest }) {
  const id = useId();
  return (
    <div className="field">
      {label && (
        <label className="field-label" htmlFor={id}>
          {label}
        </label>
      )}
      <input id={id} className={`input${error ? ' input-error' : ''}`} {...rest} />
      {error ? <div className="field-error">{error}</div> : hint && <div className="field-hint">{hint}</div>}
    </div>
  );
}

export function SelectField({ label, children, ...rest }) {
  const id = useId();
  return (
    <div className="field">
      {label && (
        <label className="field-label" htmlFor={id}>
          {label}
        </label>
      )}
      <select id={id} className="input select" {...rest}>
        {children}
      </select>
    </div>
  );
}

export function Segmented({ options, value, onChange, label, size }) {
  return (
    <div className={`segmented${size ? ` segmented-${size}` : ''}`} role="tablist" aria-label={label}>
      {options.map((o) => (
        <button
          key={o.value}
          role="tab"
          type="button"
          aria-selected={value === o.value}
          className={value === o.value ? 'active' : ''}
          onClick={() => onChange(o.value)}
        >
          {o.label}
        </button>
      ))}
    </div>
  );
}

export function Stepper({ value, onChange, min = 1, max = 99, label }) {
  const set = (v) => onChange(Math.max(min, Math.min(max, v)));
  return (
    <div className="stepper" aria-label={label}>
      <button type="button" onClick={() => set(value - 1)} disabled={value <= min} aria-label="Mažinti">
        <IconMinus />
      </button>
      <span className="stepper-value">{value}</span>
      <button type="button" onClick={() => set(value + 1)} disabled={value >= max} aria-label="Didinti">
        <IconPlus width={18} height={18} strokeWidth={2.2} />
      </button>
    </div>
  );
}

export function StatusPill({ status }) {
  const { label, tone } = statusInfo(status);
  return <span className={`pill pill-${tone}`}>{label}</span>;
}

export function Capacity({ joined, max, compact }) {
  const pct = max > 0 ? Math.min(100, (joined / max) * 100) : 0;
  const free = Math.max(0, max - joined);
  const tone = pct >= 100 ? 'red' : pct >= 80 ? 'orange' : 'green';
  return (
    <div className={`capacity${compact ? ' capacity-compact' : ''}`}>
      <div className="capacity-row">
        <span className="capacity-count">
          {joined}
          <span className="muted"> / {max}</span>
        </span>
        {!compact && <span className="muted">{free === 0 ? 'Vietų nebėra' : `Laisva vietų: ${free}`}</span>}
      </div>
      <div className="bar">
        <div className={`bar-fill bar-${tone}`} style={{ width: `${pct}%` }} />
      </div>
    </div>
  );
}

export function Group({ title, footer, children }) {
  return (
    <section className="group">
      {title && <h3 className="group-title">{title}</h3>}
      <div className="group-body">{children}</div>
      {footer && <p className="group-footer">{footer}</p>}
    </section>
  );
}

export function Row({ label, value, onClick, href, chevron, destructive, accent, children }) {
  const content = (
    <>
      <span className={`row-label${destructive ? ' destructive' : ''}${accent ? ' accent' : ''}`}>{label}</span>
      {value !== undefined && <span className="row-value">{value}</span>}
      {children}
      {(chevron || onClick) && !destructive && !accent && <IconChevronRight className="row-chevron" />}
    </>
  );
  if (href)
    return (
      <a className="row row-action" href={href} target="_blank" rel="noreferrer">
        {content}
      </a>
    );
  if (onClick)
    return (
      <button type="button" className="row row-action" onClick={onClick}>
        {content}
      </button>
    );
  return <div className="row">{content}</div>;
}

export function EmptyState({ icon, title, text, action }) {
  return (
    <div className="empty">
      {icon && <div className="empty-icon">{icon}</div>}
      <h3>{title}</h3>
      {text && <p>{text}</p>}
      {action}
    </div>
  );
}

export function Spinner() {
  return (
    <div className="center-pad">
      <span className="spinner" aria-label="Kraunama" />
    </div>
  );
}

export function ErrorNote({ error, onRetry }) {
  if (!error) return null;
  return (
    <div className="note note-error">
      <span>{error.message ?? String(error)}</span>
      {onRetry && (
        <button type="button" className="link" onClick={onRetry}>
          Bandyti dar kartą
        </button>
      )}
    </div>
  );
}
