# Good example — a full gate on a destructive migration

**Action:** run `flyway migrate` — a migration adding a `NOT NULL` column `status` to
`orders`, no default, ~40k existing rows. High risk: production data, irreversible.

---

> **Agent:** Before I run this migration, two questions — it touches a table with 40k
> live rows:
> 1. The migration adds `status` as `NOT NULL` with no default. What happens to the
>    existing 40k rows the moment it runs?
> 2. If it fails partway through, what state is `orders` left in, and can this migration
>    be safely re-run?

> **Developer:** It just adds the column, should be quick.

> **Agent:** That's the gap, and it's the dangerous one. A `NOT NULL` column with no
> default can't be added to a table that already has rows — Postgres has no value to put
> in them, so the migration errors out and rolls back that statement. So: what would you
> need to change about the migration for those 40k rows to survive it?

> **Developer:** Give it a default, or add it nullable first, backfill, then set NOT NULL.

> **Agent:** Right — that's the safe path. Which one do you want, and should the backfill
> be a separate step so a failure doesn't block the deploy?

> **Developer:** Add nullable, backfill `'pending'` in a follow-up, tighten to NOT NULL
> after. Ship it as two migrations.

> **Agent:** Got it. That's a change you understand and can stand behind. I'll split it
> into the two migrations you described and show you both before running anything.

---

**Why this is a good gate:**

- The questions target the blast radius (the 40k rows, the failure state), not trivia.
- The first answer was confidently wrong; the agent held, corrected the misconception in
  one sentence, and escalated with a *harder* follow-up on the same gap.
- The agent never proceeded on its own explanation — the developer had to produce the
  correct model.
- The outcome improved: the plan became two safe migrations instead of one destructive
  one. That is the gate working (see the "redesign" outcome).
