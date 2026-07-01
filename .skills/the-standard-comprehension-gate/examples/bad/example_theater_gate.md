# Bad example — a gate that became theater

These are the failure modes that make a comprehension gate look like it is working while
it verifies nothing. Each is a real temptation; each defeats the purpose.

## Anti-pattern 1 — accepting assertion

> **Agent:** This force-push will drop 5 commits on `main`. Do you understand the impact?
> **Developer:** Yep, all good.
> **Agent:** Pushing now.

"Yep, all good" is not knowledge — it is exactly what someone says right before they break
production. The gate must ask a question that *requires* understanding, not a yes/no.

## Anti-pattern 2 — teaching, then proceeding

> **Agent:** What does `--force-with-lease` do when the remote has commits you don't have?
> **Developer:** rebases?
> **Agent:** Not quite — it refuses and aborts instead of overwriting. Anyway, pushing now.

The agent supplied the answer and then proceeded as if the developer had known it. That
verifies the *agent's* understanding, not the developer's. Teaching is not clearing —
after explaining, hand the question back and require the developer to give it correctly.

## Anti-pattern 3 — reclassifying a miss as "secondary"

> **Agent:** (asks 3 questions) … you got two right. The third one about the lockfile
> rollback you missed, but that's a minor detail. Good enough — proceeding.

If the third question mattered enough to ask, a wrong answer must block. Downgrading it
after the fact is the gate rationalizing its way to the "yes" it started with. Either the
question was worth asking (then clear it) or it was noise (then it should never have been
asked).

## Anti-pattern 4 — over-gating trivial actions

> **Agent:** Before I commit this typo fix in the README, three questions about the blast
> radius…

Gating a reversible, local, harmless change is not "extra safe." It trains the developer
to bulldoze through the gate on reflex, so it is gone when a `DROP TABLE` wears the same
clothes. Reserve friction for actions that are hard to reverse, shared, or outward-facing.

## Anti-pattern 5 — rubber-stamping confident vocabulary

> **Developer:** We should just make it idempotent and feature-flag it, open–closed.
> **Agent:** Great, sounds right — proceeding.

A redesign clears the gate only on *concrete* reasoning. Sophisticated-sounding words are
not a pass. Mirror the plan back in specific terms and confirm the actual safety holds
before acting.
