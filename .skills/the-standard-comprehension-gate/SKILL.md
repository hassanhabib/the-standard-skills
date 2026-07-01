---
name: The Standard Comprehension Gate
description: Ensures the human demonstrably understands any impactful, hard-to-reverse action before it is taken — committing, pushing, merging, deploying, running migrations or side-effecting scripts, deleting data, changing infra/config. Verifies comprehension actively with project-anchored questions instead of accepting "looks good" or "I understand," and only proceeds once understanding is proven.
the standard version: v2.13.0
skill version: v0.1.0.0
---

# The Standard — Comprehension Gate

## What this skill is

This skill is the awareness layer between intent and consequence. It exists to keep a
conscious human in the loop at the exact moment an agent is about to do something to a
system the human will be held responsible for.

An agent can generate a change faster than a human can understand it. That speed
creates a quiet danger: the human clicks "yes," the code ships, and nobody in the loop
actually knew what would happen — until it does. The 2017 S3 outage that took down half
the internet was one engineer running a routine command with a wrong parameter; the
machine did exactly what it was told while no fully-present human stood between the
command and its blast radius. That is the failure mode this skill defends against.

The governing principle: **understanding must be demonstrated, not asserted.** "I
understand," "looks good," and "just do it" are worth nothing here — they are exactly
what someone says right before they break production. The only proof of understanding is
answering questions that cannot be answered *without* understanding.

Think of it as the "solve a math problem before sending a 2am text" check, applied to
shipping code. A small, deliberate speed bump that confirms the person is actually
present before an irreversible thing happens.

## Explicit coverage map

This skill explicitly covers:

- Which actions require a comprehension gate (scope)
- How to scale gate depth to risk (reversibility, shared state, external visibility)
- How to author project-anchored questions that cannot be faked or copy-pasted
- How to judge answers honestly (correct / partial / wrong)
- How to escalate when answers miss, without wandering or interrogating
- The rule that teaching an answer never counts as the human knowing it
- The three outcomes: proceed, hold, or redesign the action
- Deliberate, effortful override when the human knowingly accepts unverified risk

## When to use

Activate this skill **before taking any action, on the human's behalf, that is hard to
reverse, changes shared or persistent state, or is visible outside the local machine** —
commits, pushes, merges, force-pushes, deploys, releases, database migrations,
destructive queries, side-effecting scripts, infra/config/secret changes, or deleting
files you did not create.

Do **not** gate cheap, reversible, local, read-only work (reading files, `git
status`/`diff`, running tests, building, drafting uncommitted code). Friction on safe
actions just trains people to click through, which destroys the gate when it matters.

Full scope taxonomy and depth scaling: [rules/rules.md](rules/rules.md) and the risk
table therein. This skill is standalone — it has no prerequisite skills.

## Mandatory operating rules

0. Understanding is demonstrated, not asserted. "I understand" is never a pass.
1. Only gate actions that are hard to reverse, change shared state, or leave the machine.
2. Scale the number and difficulty of questions to the risk.
3. Every question must be answerable only by someone who has read *this* codebase — never
   generic, never copy-pasteable to another chat window.
4. Every question you ask is a gate you must clear. No reclassifying a missed answer as
   "secondary" to justify proceeding.
5. Teaching the answer is not clearing it. After you explain, the human must give it back
   correctly before it counts.
6. Proceed only when every question is genuinely cleared.
7. The human, now engaged, concluding the action is wrong and redesigning it is the gate's
   best outcome — not a loose end to force back onto the original questions.
8. Honor a deliberate override, but make it effortful: name what goes unverified and the
   risk, and require explicit acceptance.

## The workflow

### 1. Understand it yourself first

You cannot quiz someone on something you do not understand. Read the actual diff (`git
diff`, not your memory of what you wrote), the command and its flags, the state it
touches, and what happens if it fails halfway. Name the **blast radius** — the worst
realistic outcome and who it hits. If you cannot, investigate before gating.

### 2. Assess risk and set depth

Match questions to stakes (full table in [rules/rules.md](rules/rules.md)):

- **Low** (small reversible commit on a feature branch): 1 question.
- **Medium** (push to a shared branch, routine deploy): 2 questions.
- **High** (migrations, deletes, force-push, prod config, anything irreversible): 3
  questions, and hold the line harder on vague answers.

### 3. Ask questions that cannot be faked or copy-pasted

Generate 1–3 questions aimed at the most consequential, most-likely-misunderstood parts
of the change — not trivia, not definitions, not things guessable from the question. Two
non-negotiable properties (craft details in
[rules/rules.md](rules/rules.md), examples in [examples/good](examples/good) and
[examples/bad](examples/bad)):

- **Un-copy-pasteable.** Anchor each question in this project's real symbols, files,
  schema, and callers, so a model that has never seen this repo cannot answer it. If a
  stranger LLM could answer it, it is too generic — the human could paste it into another
  chat and launder the thinking through a machine. That copy-paste *is* the brain-death
  this gate exists to stop.
- **Worth holding for.** If a wrong answer would not stop you, it is noise; cut it.

Ask, then **stop and wait.** Do not answer them yourself, do not hint, do not proceed.

### 4. Judge the answers honestly

For each: **correct**, **partial/vague**, or **wrong**. Be fair but not generous — a
confident wrong answer is the most dangerous signal there is, because that misconception
is the exact thing about to cause harm. Reward terse-and-right over fluent-and-wrong.

### 5. Escalate when they miss

Getting it wrong is the gate doing its job. When an answer is wrong or vague:

1. Close the specific gap in a sentence or two — teach, do not scold. But teaching does
   not clear the question; you supplied the answer, which proves what *you* know.
2. Re-probe the **same gap** plus one adjacent consequence, at higher difficulty. Narrow
   in; do not wander to new topics.
3. Repeat. Questions get harder, not easier, while answers stay wrong — the human climbs
   up to understanding; the action does not come down to meet them.

If after ~2–3 focused rounds the human still cannot answer, **that is the answer: do not
take the action.** Offer the real off-ramps — walk through it together, shrink the change
to something they understand, or shelve it.

### 6. Proceed only when every question is cleared

Proceed only when every question you asked has landed a correct answer — from *them*, in
their words, after any teaching. Not "most," not "the ones that mattered." Letting a
wrong answer slide because you decided it did not really matter is the gate becoming
theater.

### 7. Watch for the best outcome: the human redesigns the action

There is a third result besides *proceed* and *hold*, and it is the highest one: the
human, now engaged, realizes the action should not be taken as designed and changes it —
"don't drop that column on a live table; add the new schema alongside it and roll over
slowly." Do not rigidly insist they answer the original questions first; the redesign
usually demonstrates deeper understanding than the questions asked. Recognize it, drop
the plan, help build the safer one (which faces its own gate when ready).

Guard against the opposite failure: a redesign clears the gate only if the reasoning is
concrete, not because it sounds sophisticated. Confident vocabulary ("open–closed," "just
make it idempotent") is not a pass — mirror the new plan back in specific terms and
confirm the safety actually holds.

## Conduct

- **Do not leak the answer** — not in the question, not in a "hint," not in the framing.
- **Do not accept assertions of understanding.** Redirect warmly to the actual question.
- **Do not grade on a curve to reach "yes"** — in either direction (teaching-then-
  proceeding, or rubber-stamping confident architecture-speak).
- **Do not be a jerk.** The tone is a sharp colleague doing a genuine gut-check, not a
  bouncer. Keep it short; scale down for trivial changes.
- **Honor a deliberate override, but make it deliberate.** State what goes unverified and
  the specific risk, and require explicit acceptance ("proceed without verifying — I
  accept the risk"). Friction, not a wall. An emergency is a reason to be *more* awake.

## Worked example

See [examples/good/example_gate_migration.md](examples/good/example_gate_migration.md)
for a full gate on a destructive migration where the first confident answer is wrong, the
follow-up gets harder, and the outcome improves. See
[examples/bad/example_theater_gate.md](examples/bad/example_theater_gate.md) for the
failure modes to avoid.

## Files

- [rules/rules.md](rules/rules.md) / [rules/rules.json](rules/rules.json) — scope, depth
  scaling, question craft, verification, and outcome rules (human + machine readable).
- [validations/checklist.md](validations/checklist.md) — run before any gated action.
- [validations/anti-patterns.md](validations/anti-patterns.md) — the ways a gate becomes
  theater.
- [examples/good](examples/good) / [examples/bad](examples/bad) — anchored vs. generic
  questions, real gates vs. rubber stamps.
- [contracts/contracts.json](contracts/contracts.json) — machine contract for inputs,
  outcomes, and thresholds.
