# The Standard Comprehension Gate — Anti-Patterns

Each of these makes the gate *look* like it is working while it verifies nothing. They are
the specific ways to fail; see [examples/bad](../examples/bad) for dialogue.

---

## Accepting assertion (violates cg-000)

Treating "I understand," "looks good," or "yep, all good" as a pass. These are precisely
what someone says right before they break production. Confidence is not comprehension.

## Teaching, then proceeding (violates cg-051)

Explaining the answer to a missed question and then acting as if the human knew it. That
verifies *your* understanding, not theirs. After teaching, hand the question back.

## Reclassifying a miss as "secondary" (violates cg-060)

Downgrading a question you asked, after a wrong answer, so you can proceed. If it mattered
enough to ask, a wrong answer must block. If it did not matter, it should not have been
asked.

## Generic, copy-pasteable questions (violates cg-030)

Questions any model can answer without the repo ("what does force-push do?"). The human
pastes them into another chat and launders the thinking through a machine. Anchor every
question in this codebase.

## Leaking the answer (violates cg-034)

Embedding the answer in the question, a "hint," or the framing. A question that contains
its own answer verifies nothing.

## Over-gating trivial actions (violates cg-017)

Quizzing on a typo fix or a read-only command. This trains the human to bulldoze through
the gate on reflex, so it is gone when a dangerous action arrives. Reserve friction for
consequence.

## Interrogating past the off-ramp (violates cg-052)

Grinding through round after round when the human plainly cannot answer. The verdict is
already in: do not take the action. Switch to walking through it, shrinking it, or
shelving it.

## Rubber-stamping confident vocabulary (violates cg-062)

Accepting a redesign because it *sounds* sophisticated ("idempotent," "open–closed,"
"feature-flag it"). A redesign clears only on concrete reasoning. Mirror it back in
specific terms and confirm the safety actually holds.

## Silent override (violates cg-071)

Skipping verification because the human said "just do it," without naming what goes
unverified and the risk. An override must be effortful and explicit, or it is just the
assertion failure wearing a badge.
