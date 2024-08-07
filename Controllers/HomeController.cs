using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DatingApplication.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var currentUserProfile = await _context.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (currentUserProfile == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var interestedInGender = currentUserProfile.InterestedIn;
            ViewBag.Preferences = GetPreferences();
            var profiles = await _context.Profiles.Include(p => p.User).Where(p => p.UserId != userId && p.Gender == interestedInGender)
                .ToListAsync();

            return View(profiles);
        }

        public async Task<IActionResult> SearchProfiles(string query, string preferences, string ageRange)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUserProfile = await _context.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (currentUserProfile == null)
            {
                return PartialView("_ProfilesPartial", new List<Profile>());
            }

            var interestedInGender = currentUserProfile.InterestedIn;
            var profilesQuery = _context.Profiles
                .Include(p => p.User)
                .Where(p => p.Gender == interestedInGender && p.UserId != userId);

            if (!string.IsNullOrEmpty(query))
            {
                profilesQuery = profilesQuery.Where(p => p.User.Name.Contains(query) || p.Bio.Contains(query));
            }

            if (!string.IsNullOrEmpty(preferences))
            {
                var preferencesList = preferences.Split(',').ToList();
                profilesQuery = profilesQuery.Where(p => preferencesList.All(pref => p.Preferences.Contains(pref)));
            }

            var profiles = await profilesQuery.ToListAsync();

            if (!string.IsNullOrEmpty(ageRange))
            {
                var ageRangeParts = ageRange.Split('-');
                int minAge = int.Parse(ageRangeParts[0]);
                int maxAge = ageRangeParts.Length > 1 ? int.Parse(ageRangeParts[1]) : int.MaxValue;

                profiles = profiles
                    .Where(p => CalculateAge(p.Day, p.Month, p.Year) >= minAge && CalculateAge(p.Day, p.Month, p.Year) <= maxAge)
                    .ToList();
            }

            ViewBag.Preferences = GetPreferences();
            return PartialView("_ProfilesPartial", profiles);
        }

        private List<string> GetPreferences()
        {
            return new List<string> {
            "90s Kid", "Harry Potter", "SoundCloud", "Spa Self Care", "Maggi", "Heavy Metal",
            "House Parties", "Gin Tonic", "Gymnastics", "Ludo", "Hockey", "Cafe Hopping",
            "Walking", "Hot Yoga", "Biryani", "Basketball", "Sneakers", "Meditation", "Sushi",
            "Spotify", "Slam Poetry", "Home Workout", "Theater", "Aquarium", "Instagram",
            "Hot Springs", "Running", "Travel", "Language Exchange", "Movies", "Guitarists",
            "Social Development", "Gym", "Social Media", "Hip Hop", "K-Pop", "Skateboarding",
            "Gospel", "Potterhead", "Trying New Things", "Photography", "Bollywood", "Bhangra",
            "Reading", "Singing", "Sports", "Poetry", "Stand Up Comedy", "Coffee", "Karaoke",
            "Fortnite", "Free Diving", "Self Development", "Mental Health Awareness", "Foodie Tour",
            "Voter Rights", "Jiu-Jitsu", "Climate Change", "Exhibition", "Walking My Dog",
            "LGBTQIA+ Rights", "Feminism", "VR Room", "Escape Cafe", "Shopping", "Brunch",
            "Investment", "Jetski", "Reggaeton", "Second-Hand Apparel", "Black Lives Matter",
            "Jogging", "Road Trips", "Vintage Fashion", "Voguing", "Couchsurfing", "Happy Hour",
            "Inclusivity", "Country Music", "Football", "Inline Skate", "Investing", "Tennis",
            "Ice Cream", "Ice Skating", "Human Rights", "West End Musicals", "Expositions",
            "Snowboarding", "Pig Roast", "Skiing", "Canoeing", "Dilatos", "Pentathlon", "Broadway",
            "PlayStation", "Five-a-Side Football", "Cheerleading", "Car Racing", "Choir", "Pole Dancing",
            "Pinterest", "Festivals", "Pub Quiz", "Catan", "Cosplay", "Motor Sports", "Coffee Stall",
            "Content Creation", "E-Sports", "Bicycle Racing", "Binge-Watching TV Shows", "Songwriter",
            "Bodycombat", "Tattoos", "Painting", "Bodyjam", "Paddle Boarding", "Padel", "Blackpink",
            "Surfing", "Bowling", "Grime", "90s Britpop", "Upcycling", "Equality", "Bodypump",
            "Beach Bars", "Bodystep", "Paragliding", "Astrology", "Motorcycles", "Equestrian",
            "Entrepreneurship", "Sake", "BTS", "Cooking", "Environmental Protection", "Fencing",
            "Soccer", "Saxophonist", "Sci-Fi", "Dancing", "Film Festival", "Freeletics", "Gardening",
            "Amateur Cook", "Exchange Program", "Sauna", "Art", "Politics", "Flamenco", "Museum",
            "Activism", "DAOs", "Real Estate", "Podcasts", "Disability Rights", "Rave", "Pimms",
            "Drive-Thru Cinema", "Rock Climbing", "BBQ", "Craft Beer", "Iced Tea", "Drummer",
            "Tea", "Board Games", "Roblox", "Pubs", "Rock", "Tango", "Drawing", "Trivia", "Pho",
            "Volunteering", "Environmentalism", "Rollerskating", "Wine", "Dungeons & Dragons",
            "Weightlifting", "Live Music", "Writing", "Xbox", "Vlogging", "Electronic Music", "Ramen",
            "World Peace", "Wrestling", "Literature", "Youth Empowerment", "Manga", "Pride",
            "Marathon", "Makeup", "YouTube", "Martial Arts", "Marvel", "Luge", "Ice Hockey",
            "Taekwondo", "Trampoline", "Water Polo", "Rhythmic Gymnastics", "Vegan Cooking",
            "Rowing", "Vermut", "Korean Food", "Sports Shooting", "Twitter", "Squash", "Volleyball",
            "Walking Tour", "Vinyasa", "Virtual Reality", "League of Legends", "Karate", "Lacrosse",
            "NFTs", "Bar Hopping", "Nintendo", "Baseball", "Parties", "Ballet", "Band", "Online Games",
            "Battle Ground", "Beach Tennis", "Nightlife", "Online Shopping", "Sailing", "Bassist",
            "Online Broker", "Military", "Memes", "Among Us", "Motorbike Racing", "Muay Thai",
            "Motorcycling", "Metaverse", "Mindfulness", "Acapella", "Musical Instrument", "Art Galleries",
            "Musical Writing", "Baking", "Camping", "Blogging", "Collecting", "Cars", "Startups",
            "Boba Tea", "High School Sports", "Badminton", "Active Lifestyle", "Fashion", "Anime",
            "NBA", "MLB", "Funk Music", "Diving", "Caipirinha", "Flag Football", "Handball",
            "Artistic Swimming", "Athletics", "Indoor Activities", "Softball", "Beach Volleyball",
            "Tempeh", "DIY", "Town Festivities"
        };
        }

        private int CalculateAge(int day, int month, int year)
        {
            var today = DateTime.Today;
            var age = today.Year - year;

            if (today.Month < month || (today.Month == month && today.Day < day))
            {
                age--;
            }

            return age;
        }


        [HttpPost]
        public async Task<IActionResult> LikeProfile(string matchedUserId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var match = new Match
            {
                UserId = userId,
                MatchedUserId = matchedUserId,
                IsLiked = true,
                IsRejected = false
            };
            _context.Matches.Add(match);

            var notification = new Notification
            {
                UserId = matchedUserId,
                Message = "You have a new like!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DislikeProfile(string dislikedUserId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var match = new Match
            {
                UserId = userId,
                MatchedUserId = dislikedUserId,
                IsLiked = false,
                IsRejected = true
            };
            _context.Matches.Add(match);

            await _context.SaveChangesAsync();

            return Ok();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
