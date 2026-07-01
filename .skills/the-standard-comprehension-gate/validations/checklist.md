# The Standard Comprehension Gate — Validation Checklist

Run this before any gated action, and again before you actually proceed.

---

## SCOPE — is a gate required?

- [ ] **cg-010** The action is hard to reverse, changes shared/persistent state, or is
      visible outside this machine (if none apply, do not gate — see cg-017).
- [ ] **cg-017** You are NOT gating read-only or trivially reversible local work.
- [ ] **cg-018** Where size and consequence disagree, you judged by consequence.

## DEPTH — pressed proportionally?

- [ ] **cg-020** Question count matches risk (low = 1, medium = 2, high = 3).
- [ ] **cg-021** You bumped the tier for amplifiers (prod, large blast radius, no
      rollback, fatigue/time pressure).

## QUESTIONS — fake-proof?

- [ ] **cg-030** Every question is anchored in this repo's real symbols/files/schema/
      callers — a model without repo access could not answer it.
- [ ] **cg-031** Questions ask about consequences and blast radius, not trivia.
- [ ] **cg-032** No question is guessable from its own text; none are yes/no or leading.
- [ ] **cg-033** Every question is one you would actually hold the action for.
- [ ] **cg-034** No answer leaked in the question, a hint, or the framing.
- [ ] **cg-035** You asked, then stopped and waited — you did not answer for them.

## JUDGING & ESCALATION — honest?

- [ ] **cg-040** Each answer graded correct / partial / wrong on the accuracy of its model.
- [ ] **cg-041** A confident wrong answer was weighted heavily, not waved through.
- [ ] **cg-050** On a miss, you taught briefly and re-probed the same gap harder.
- [ ] **cg-051** You did NOT count your own explanation as the human clearing the question.
- [ ] **cg-052** After ~2–3 rounds with no correct answer, you held the action.

## PROCEEDING — genuinely cleared?

- [ ] **cg-060** EVERY question you asked was cleared by the human — none reclassified as
      "secondary."
- [ ] **cg-061** If the human redesigned the action, you accepted it on concrete reasoning
      (not vocabulary) and it will face its own gate.
- [ ] **cg-062** You did not grade on a curve to reach "yes" in either direction.

## OVERRIDE — deliberate?

- [ ] **cg-071** Any override stated what goes unverified and the specific risk, and
      required explicit acceptance.
