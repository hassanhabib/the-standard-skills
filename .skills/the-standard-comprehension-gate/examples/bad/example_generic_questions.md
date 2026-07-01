# Bad example — generic questions the developer can offload

A generic, project-agnostic question is the most common way a comprehension gate leaks.
The developer copies it into another chat window, pastes back the textbook answer, and the
gate signs off on thinking that never happened.

## The questions (all bad)

> "What does `DROP COLUMN` do to a table?"
> "Why is force-pushing to a shared branch dangerous?"
> "What's the difference between a migration and a backfill?"
> "What happens if a deploy fails halfway?"

Every one of these can be answered by a model that has never seen the repo. They quiz
general engineering knowledge, which is one paste away — so they verify nothing about
whether the developer understands *this* change.

## Why they fail the test

The test for any question is: **could a model with no access to this project answer it
correctly?** For all of the above, yes. That makes them too generic.

## The fix — bind each to the actual change

| Generic (bad) | Anchored (good) |
|---|---|
| "What does `DROP COLUMN` do?" | "`BillingService.resolvePlan()` running in prod reads `legacy_plan_tier` as a fallback when `plan_id` is null. If we drop that column before the new code deploys, what happens to a live request for a null-`plan_id` user?" |
| "Why is force-push dangerous?" | "Marcus's 3 refund-webhook commits are on `origin/main` and not in your local history. After this force-push, which endpoint in our API stops responding, and why?" |
| "What happens if a deploy fails halfway?" | "Our deploy runs `migrate:prod` then `deploy`. In the window between them, what does the still-running old `PlanResolver` do when the column it expects is already gone?" |

The anchored versions cannot be answered without tracing real code in this repo — which is
exactly the understanding the gate exists to confirm.
