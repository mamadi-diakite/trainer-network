using TrainerNetwork.Domain;

namespace TrainerNetwork.Infrastructure.Seeding;

internal sealed record DemoAccount(string Email, string DisplayName, UserRole Role);

internal sealed record DemoProfession(string Name, string Slug, DemoTopic[] Topics);

internal sealed record DemoTopic(string Name, string Slug, string Description, DemoPage[] Pages);

internal sealed record DemoPage(string Title, string Summary, string AuthorEmail, DemoSection[] Sections);

internal sealed record DemoSection(string Title, string Content);

/// <summary>Fictional accounts and content for local demos.</summary>
internal static class DemoContent
{
    private const string Claire = "claire.martin@example.com";
    private const string Karim = "karim.benali@example.com";
    private const string Sophie = "sophie.leroy@example.com";

    public static readonly DemoAccount[] Accounts =
    [
        new("moderator@example.com", "Demo Moderator", UserRole.Moderator),
        new(Claire, "Claire Martin", UserRole.Trainer),
        new(Karim, "Karim Benali", UserRole.Trainer),
        new(Sophie, "Sophie Leroy", UserRole.Trainer),
        new("reader@example.com", "Demo Reader", UserRole.Reader),
    ];

    public static readonly DemoProfession[] Professions =
    [
        new("Occupational health and safety", "occupational-health-safety",
        [
            new("Risk assessment", "risk-assessment", "Spotting hazards and ranking risks with learners.",
            [
                new("Running a workplace walk-through with learners", "Take the group onto the floor before any slides.", Claire,
                [
                    new("Why start on the floor", """
                        Learners remember what they saw themselves. A 30-minute walk-through gives you
                        real examples to work with for the rest of the day, instead of stock photos.
                        """),
                    new("Before the walk", """
                        - Agree on the route with the site manager.
                        - Hand out a one-page grid: hazard, who is exposed, how often, what is in place.
                        - Split the group into pairs, one writer and one observer.
                        """),
                    new("During the walk", """
                        Stay quiet for the first ten minutes. Let pairs spot things on their own,
                        then ask open questions: *who walks here at night?*, *what happens when it rains?*
                        """),
                    new("Debrief", """
                        Back in the room, put every finding on the wall and rank them together.
                        Disagreements about the ranking are the best part: let them argue with facts.
                        """),
                ]),
            ]),
            new("First aid at work", "first-aid-at-work", "Teaching first aid skills that stay after the course.",
            [
                new("Practising the recovery position without boring the group", "Short rounds, many partners, quick feedback.", Claire,
                [
                    new("The problem", """
                        Ten people watching one volunteer on the floor is not practice.
                        Most learners leave having done the move once, or not at all.
                        """),
                    new("Rotating pairs", """
                        1. Run rounds of two minutes.
                        2. Change partners every round.
                        3. The one lying down gives one piece of feedback before switching.

                        After five rounds everyone has done it at least five times.
                        """),
                    new("Mistakes to watch for", """
                        - Head not tilted back, so the airway stays closed.
                        - Top leg not bent, so the person rolls back.
                        - Nobody checks breathing again after the move.
                        """),
                ]),
            ]),
        ]),
        new("Information technology", "information-technology",
        [
            new("Teaching Git", "teaching-git", "Making version control click for beginners.",
            [
                new("Explaining branches with a paper timeline", "No terminal for the first twenty minutes.", Karim,
                [
                    new("Why paper first", """
                        Beginners get lost between the commands and the idea. Drawing the history first
                        separates the two: once the picture makes sense, the commands are just names.
                        """),
                    new("The setup", """
                        Give each pair a long strip of paper and sticky notes. Each note is a commit,
                        with a short message. The strip is `main`.
                        """),
                    new("Branching and merging", """
                        Ask one learner to start a second strip from a note in the middle: that is a branch.
                        Add notes on both strips, then ask how to bring them back together.
                        Let them invent the merge before you name it.
                        """),
                    new("Moving to the terminal", """
                        Replay the same story with `git switch -c`, `git commit` and `git merge`,
                        and keep the paper strips on the table so they can check where they are.
                        """),
                ]),
            ]),
            new("Web development basics", "web-development-basics", "First steps with HTML, CSS and the browser.",
            [
                new("A first HTML page in 30 minutes", "Something visible on screen before any theory.", Karim,
                [
                    new("Start with the result", """
                        Show a finished page about something the group likes, then open the source.
                        Learners see that a page is just text with tags.
                        """),
                    new("Build it together", """
                        Type along, one tag at a time: `h1`, `p`, `img`, `a`. Refresh the browser
                        after each change. The fast loop matters more than the vocabulary.
                        """),
                    new("Let them break it", """
                        Ask everyone to remove a closing tag on purpose and see what happens.
                        It takes the fear out of mistakes and opens the topic of browser tools.
                        """),
                ]),
            ]),
        ]),
        new("Accounting", "accounting",
        [
            new("Bookkeeping", "bookkeeping", "Teaching the basics of recording transactions.",
            [
                new("Teaching double-entry with a corner shop case", "One small business, one month, every entry by hand.", Sophie,
                [
                    new("The case", """
                        A fictional corner shop opens on the first of the month. Learners get
                        twelve documents: invoices, receipts and bank statements.
                        """),
                    new("One rule at a time", """
                        Start with cash only. Add the bank in round two, then supplier credit.
                        Each round adds one idea, so learners know where an error comes from.
                        """),
                    new("Checking the balance", """
                        At the end, every group builds a trial balance. If it does not balance,
                        groups swap their journals and look for each other's errors.
                        """),
                ]),
            ]),
            new("Payroll", "payroll", "Explaining payslips and payroll steps.", []),
        ]),
        new("Sales", "sales",
        [
            new("Prospecting", "prospecting", "Finding and qualifying new customers.", []),
            new("Negotiation", "negotiation", "Preparing and running a negotiation.",
            [
                new("Role plays that feel real", "Give each side information the other side does not have.", Sophie,
                [
                    new("Why most role plays fall flat", """
                        When both sides read the same brief, there is nothing to discover.
                        Learners play a script instead of listening.
                        """),
                    new("Split briefs", """
                        Write one brief for the buyer and one for the seller. Each has a limit,
                        a deadline and one hidden constraint. They must find it by asking questions.
                        """),
                    new("Observers with one job", """
                        Give each observer a single thing to track: open questions, silences, or concessions.
                        Their feedback is short and precise.
                        """),
                    new("Debrief", """
                        Reveal both briefs at the end. Compare the deal reached with what was possible,
                        and ask which question would have changed the outcome.
                        """),
                ]),
            ]),
        ]),
    ];
}
