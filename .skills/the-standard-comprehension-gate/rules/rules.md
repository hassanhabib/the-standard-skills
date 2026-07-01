# The Standard Comprehension Gate — Rules

These rules govern when to gate an action, how hard to press, how to author questions
that cannot be faked, how to judge answers, and when it is legitimate to proceed. The
machine-readable form is [rules.json](rules.json).

---

## 1. Scope — what to gate

The test is not "is this a git command." It is three questions about the action itself.
If any answer is "yes," gate it.

1. **Is it hard to reverse?** Can the human cleanly undo it in seconds, or does undoing
   require heroics (restoring a backup, revert-and-redeploy, apologizing to users)?
2. **Does it change shared or persistent state?** Does it touch something other people or
   future runs depend on — a shared branch, a database, infra, a published artifact?
3. **Is it visible outside this machine?** Network calls, deploys, messages, published
   packages, spent money?

**Clearly gate:** push, merge, force-push, rebase onto shared branches, rewriting
published history, tags, releases; deploys, releases, publishing packages/images;
migrations, backfills, `DELETE`/`UPDATE`/`DROP`/`TRUNCATE`, bulk edits, cache/queue
purges; side-effecting scripts (write outside the repo, call a paid/external API, mutate
infra, send communications); secrets, env vars, IAM/permissions, CI/CD, feature flags,
DNS, IaC apply; deleting or overwriting files you did not create.

**Do not gate:** reading files; `git status`/`diff`/`log`; running tests, builds,
linters, type-checks; drafting or editing uncommitted code; local dev servers; anything
undoable with a keystroke that never left the machine. Gating safe actions is not "extra
safe" — it trains people to click through, so the gate is gone when a dangerous action
wears the same clothes.

When two signals disagree (a "small" diff that drops a table), trust the **consequence**,
not the size. One line can be the most dangerous line.

---

## 2. Depth — scale to risk

| Risk | Looks like | Questions | Posture |
|------|-----------|-----------|---------|
| **Low** | Small commit on a feature branch; reversible local change | 1 | Quick gut-check. A vague-but-directionally-right answer can pass. |
| **Medium** | Push to a shared branch; routine deploy; a script that hits an external service | 2 | Expect specifics. Press once on a fuzzy answer. |
| **High** | Migrations, deletes, force-push, prod config/secrets, anything irreversible or user-facing | 3 | Hold the line. Vague answers do not pass; escalate until precise or abort. |

Bump the tier — not just the count — when you see amplifiers: production or shared
environments, large blast radius, no easy rollback, time pressure or fatigue (the "2am
text" conditions), or the human moving faster than they are reading.

---

## 3. Question craft — un-copy-pasteable, un-fakeable

A question is only worth asking if it is **impossible to answer correctly without actually
understanding this change.**

- **Anchor in this codebase (most important).** Reference real function and file names,
  the actual schema, the specific caller that breaks, the domain invariant this change
  touches. The test: *could a model with no access to this repo answer it?* If yes, it is
  too generic — rewrite it. Generic questions are one paste away from another chat window.
- **Aim at the blast radius, not trivia.** Ask about consequences ("what happens to the
  40k existing rows…"), not definitions ("what does force-push do").
- **Un-guessable from its own text.** No answers embedded in the phrasing; no yes/no; no
  leading. Prefer "what happens to…", "what state is X left in…", "which users are
  affected…", "what breaks if this fails halfway."
- **Worth holding for.** If a wrong answer would not stop you, cut the question.

See [examples/good](../examples/good) and [examples/bad](../examples/bad).

---

## 4. Judging answers

- **Correct** — accurately predicts the real consequence, including the risky part. Needs
  the right model, not your exact words.
- **Partial / vague** — directionally right but dodges the specific danger, or hedged.
  Press once.
- **Wrong** — predicts something that will not happen, or misses the failure mode. A
  *confident* wrong answer is the strongest signal to keep the gate closed.

Grade on an accurate model of the outcome, not on eloquence.

---

## 5. Escalation

1. Close the gap in a sentence or two — teach, do not scold.
2. **Teaching is not clearing.** You supplied the answer, which proves what you know, not
   what they know. The question stays open until they give it back correctly.
3. Re-probe the same gap plus one adjacent consequence, at higher difficulty. Narrow in.
4. After ~2–3 focused rounds with no correct answer: stop and do not take the action. Not
   shipping a misunderstood change is a success of the gate, not a failure of the human.

---

## 6. Outcomes

- **Proceed** only when *every* asked question is cleared by the human. No reclassifying a
  missed question as "secondary."
- **Hold** when understanding cannot be demonstrated.
- **Redesign** — the best outcome — when the human, now engaged, concludes the action is
  wrong and changes it. Accept it; do not force them back onto the original questions. But
  a redesign clears only on concrete reasoning, not confident vocabulary.

---

## 7. Override

The human may deliberately skip verification, but it must be effortful, not a shrug. State
plainly what is going unverified and the specific risk, and require explicit acceptance
("proceed without verifying — I accept the risk"). Friction, not a wall. An emergency is a
reason to be *more* awake, not less.
