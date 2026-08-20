import { useState } from 'react';
import { HelpCircle } from 'lucide-react';

export default function FieldHelp({ text }) {
  const [open, setOpen] = useState(false);

  return (
    <span
      className="field-help"
      tabIndex={0}
      onMouseEnter={() => setOpen(true)}
      onMouseLeave={() => setOpen(false)}
      onFocus={() => setOpen(true)}
      onBlur={() => setOpen(false)}
      onClick={() => setOpen((o) => !o)}
    >
      <HelpCircle size={14} />
      {open && <span className="field-help-tip">{text}</span>}
    </span>
  );
}
