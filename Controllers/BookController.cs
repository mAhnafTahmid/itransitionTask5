using Microsoft.AspNetCore.Mvc;
using Bogus;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    [HttpGet]
    public IActionResult GetBooks([FromQuery] string language, [FromQuery] int seed, [FromQuery] int page, [FromQuery] double avgLikes, [FromQuery] double avgReviews, [FromQuery] int limit = 20)
    {
        if (seed == 0)
        {
            return BadRequest("Seed must be provided.");
        }

        // Combine seed and page
        int combinedSeed = seed * 31 + page;
        Randomizer.Seed = new Random(combinedSeed);

        var faker = new Faker(language switch
        {
            "en-US" => "en",
            "de-DE" => "de",
            "ja-JP" => "ja",
            _ => "en"
        });

        var genres = new[] { "Fiction", "Non-Fiction", "Fantasy", "Biography", "Science", "Romance", "Thriller", "Horror", "Self-Help" };

        var books = new List<object>();
        for (int i = 1; i <= limit; i++)
        {
            var title = faker.Commerce.ProductName();
            var author = faker.Name.FullName();
            var genre = faker.PickRandom(genres);
            int randomYear = faker.Date.Between(new DateTime(1900, 1, 1), new DateTime(2025, 12, 31)).Year;
            var publisher = $"{faker.Company.CompanyName()}, {randomYear}";
            // var coverImageUrl = $"https://dummyimage.com/300x450/000/fff&text={Uri.EscapeDataString(title)}+\nby\n+{Uri.EscapeDataString(author)}";
            var coverImageUrl = $"https://picsum.photos/seed/{Uri.EscapeDataString(title)}-{Uri.EscapeDataString(author)}/300/450";


            // Calculate likes and reviews based on averages
            int likeCount = (int)Math.Floor(avgLikes);
            if (faker.Random.Double() < avgLikes - likeCount)
            {
                likeCount++;
            }

            int reviewCount = (int)Math.Floor(avgReviews);
            if (faker.Random.Double() < avgReviews - reviewCount)
            {
                reviewCount++;
            }

            var reviews = new List<object>();
            for (int j = 0; j < reviewCount; j++)
            {
                reviews.Add(new
                {
                    Author = faker.Name.FullName(),
                    Text = faker.Lorem.Sentence(10)
                });
            }

            books.Add(new
            {
                Index = i,
                ISBN = faker.Random.ReplaceNumbers("###-##########"),
                Title = title,
                Author = author,
                Publisher = publisher,
                Genre = genre,
                Likes = likeCount,
                Reviews = reviewCount,
                ReviewsDetails = reviews,
                CoverImageUrl = coverImageUrl
            });
        }

        return Ok(books);
    }

}
