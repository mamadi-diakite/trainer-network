namespace TrainerNetwork.Infrastructure.Seeding;

public sealed class DemoDataSeeder(TrainerNetworkDbContext db, IPasswordHasher passwordHasher, TimeProvider timeProvider)
{
    public const string DemoPassword = "TestDemo123!";

    /// <returns><c>false</c> when the database already holds users or professions.</returns>
    public async Task<bool> SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken) || await db.Professions.AnyAsync(cancellationToken))
        {
            return false;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var users = DemoContent.Accounts.ToDictionary(
            account => account.Email,
            account => new User(account.Email, account.DisplayName, passwordHasher.Hash(DemoPassword), account.Role, now));
        db.Users.AddRange(users.Values);

        for (var p = 0; p < DemoContent.Professions.Length; p++)
        {
            var professionData = DemoContent.Professions[p];
            var profession = new Profession(professionData.Name, professionData.Slug, p);
            db.Professions.Add(profession);

            for (var t = 0; t < professionData.Topics.Length; t++)
            {
                var topicData = professionData.Topics[t];
                var topic = new Topic(profession.Id, topicData.Name, topicData.Slug, topicData.Description, t);
                db.Topics.Add(topic);

                foreach (var pageData in topicData.Pages)
                {
                    var page = new Page(topic.Id, users[pageData.AuthorEmail].Id, pageData.Title, pageData.Summary, now);
                    foreach (var section in pageData.Sections)
                    {
                        page.AddSection(section.Title, section.Content, now);
                    }

                    db.Pages.Add(page);
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
