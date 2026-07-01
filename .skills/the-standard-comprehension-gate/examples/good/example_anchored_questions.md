# Good example — anchored questions that cannot be copy-pasted

The single most important property of a comprehension question: it must be answerable
only by someone who has read *this* codebase. If a developer can paste it into another
chat window and paste the reply back, the gate verified nothing.

**Action:** `git push --force origin main` after a rebase that orphans teammates' commits.

## Generic (bad) — one paste away

> "What happens to remote commits when you force-push?"
>
> "What does `git push --force-with-lease` do differently from `--force`?"

Any model answers these cold. They test git general knowledge, not understanding of what
*this* push does to *this* repo.

## Anchored (good) — must have read the code

> **1.** The commits about to be orphaned are Marcus's `refund webhook handler` and its
> tests. Given how `PaymentsController` dispatches refunds in this repo, what specifically
> stops working in *our* flow if those vanish from `main` — and which endpoint goes dark?
>
> **2.** Priya's commit bumped the Stripe SDK to 14.2. Our `PaymentIntent` construction in
> `checkout.ts` — does it rely on anything that changed between the pinned version and
> 14.2, and if her commit is orphaned, does the lockfile roll back or not? What actually
> deploys?

Neither can be answered without tracing `PaymentsController`, `checkout.ts`, and the
lockfile. The cheapest way through the gate is to actually understand the change.

## How to build an anchored question

Start from the generic consequence, then **bind it to a concrete thing in the diff** — a
function that will now behave differently, a caller that will break, a real record in the
schema, a named downstream consumer. If you cannot name a project-specific anchor, you do
not yet understand the change well enough to gate it — read more first.

**Honest limit:** a determined developer can point another agent *at the repo* and have it
answer. You cannot cryptographically prevent that. But specificity raises the cost of
offloading above the cost of just understanding — and questions that span several files or
a domain invariant make "have a machine read the whole project" slower than reading it
yourself. Friction plus depth, not an unbeatable lock.
