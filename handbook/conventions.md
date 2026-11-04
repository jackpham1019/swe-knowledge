# Knowledge Base Conventions

## Organization

- Group notes by broad software engineering domain.
- Use `index.md` to orient readers within a larger topic.
- Create separate notes when a concept needs meaningful explanation, not for every new term.
- Link related topics across folders when useful.

## Naming

- Prefer descriptive, lowercase file names with hyphens, e.g. `concurrency-control.md`.
- Use `README.md` for the repository's entry point and `index.md` for topic hubs.

## Suggested topic note structure

Use only the sections needed for the topic:

```markdown
# Topic Name

## Problem / Purpose

## Mental model / Mechanism

## Relationships and dependencies

## Diagram or flow

## Failure modes and scenarios

## Practical example

## Related topics and Anki tags
```

## Diagrams

- Prefer Mermaid in Markdown for flows, relationships, sequences, and state transitions.
- If using a visual tool, retain an editable source and export a portable image where useful.
- Write a short explanation of the diagram's key relationships.

## Anki integration

- Use stable topic tags such as `k8s::networking` and `db::transactions`.
- Keep extended explanations in notes, not in every card.
- Do not create a card merely because a new note exists.

## Portability and Git

- Prefer standard Markdown links (`[Title](relative-path.md)`).
- Avoid making core notes depend on application-specific features.
- Commit meaningful changes with descriptive messages.
- Do not track device-specific Obsidian workspace state (e.g. `.obsidian/workspace.json`).

## Maintenance

- Update a note when practical work exposes a gap.
- Avoid reorganizing folders unless navigation becomes a problem.
- Aim for usefulness, not documentation completeness.
