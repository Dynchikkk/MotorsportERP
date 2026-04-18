using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Interfaces.Security;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Persistence.Settings;

namespace MotorsportErp.Infrastructure.Persistence;

public class DbInitializer
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SeedSettings _options;

    public DbInitializer(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> options)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _options = options.Value;
    }

    public async Task InitializeAsync()
    {
        if (_context.Database.IsRelational())
        {
            await _context.Database.MigrateAsync();
        }
    }

    public async Task SeedAsync()
    {
        await SeedCoreDataAsync();

        if (_options.SeedDevelopmentData)
        {
            await SeedDevelopmentDataAsync();
        }
    }

    private async Task SeedCoreDataAsync()
    {
        const string adminEmail = "admin@motorsport.erp";

        if (!await _context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Nickname = "SuperAdmin",
                Email = adminEmail,
                PasswordHash = _passwordHasher.Hash(_options.AdminPassword),
                Roles = UserRole.SuperAdmin | UserRole.Moderator | UserRole.Organizer | UserRole.Racer,
                IsBlocked = false,
                RaceCount = 48,
                Bio = "System Administrator"
            };

            _ = await _context.Users.AddAsync(admin);
            _ = await _context.SaveChangesAsync();
        }
    }

    private async Task SeedDevelopmentDataAsync()
    {
        const string markerEmail = "vera.marshal@motorsport.erp";

        if (await _context.Users.AnyAsync(u => u.Email == markerEmail))
        {
            return;
        }

        var admin = await _context.Users.FirstAsync(u => u.Email == "admin@motorsport.erp");
        var passwordHash = _passwordHasher.Hash("DevRacer_12345!");
        var now = DateTime.UtcNow;

        var vera = CreateUser("Vera Marshal", markerEmail, passwordHash, 28, UserRole.Moderator | UserRole.Racer,
            "Следит за качеством трасс и помогает организаторам доводить сетку участников.");
        var ilya = CreateUser("Ilya Vector", "ilya.vector@motorsport.erp", passwordHash, 24, UserRole.Organizer | UserRole.Racer,
            "Проводит тайм-атак и спринты, любит плотную сетку заявок и аккуратные регламенты.");
        var sofia = CreateUser("Sofia Enduro", "sofia.enduro@motorsport.erp", passwordHash, 31, UserRole.Organizer | UserRole.Racer,
            "Отвечает за длинные форматы и публикацию итоговых результатов.");
        var anna = CreateUser("Anna Apex", "anna.apex@motorsport.erp", passwordHash, 18, UserRole.Racer,
            "Ездит тайм-атак и часто тестирует новые трассы.");
        var boris = CreateUser("Boris Drift", "boris.drift@motorsport.erp", passwordHash, 12, UserRole.Racer,
            "Любит мощные заднеприводные машины и вечерние заезды.");
        var maria = CreateUser("Maria Sprint", "maria.sprint@motorsport.erp", passwordHash, 15, UserRole.Racer,
            "Регулярно выходит в топ любительских спринтов.");
        var kirill = CreateUser("Kirill Street", "kirill.street@motorsport.erp", passwordHash, 7, UserRole.Racer,
            "Ездит городские сессии и набирает стаж до крупных турниров.");
        var oleg = CreateUser("Oleg Rookie", "oleg.rookie@motorsport.erp", passwordHash, 2, UserRole.Racer,
            "Новичок системы, собирает первые зачётные старты.");
        var pavel = CreateUser("Pavel Blocked", "pavel.blocked@motorsport.erp", passwordHash, 9, UserRole.Racer,
            "Аккаунт временно ограничен после серии нарушений на площадке.", isBlocked: true);

        var users = new[]
        {
            vera, ilya, sofia, anna, boris, maria, kirill, oleg, pavel
        };

        var cars = new[]
        {
            CreateCar(admin.Id, CarClass.SuperSport, "Chevrolet", "Corvette Z06", 2023, "Администраторский демонстрационный автомобиль для стенда."),
            CreateCar(anna.Id, CarClass.Sport, "Toyota", "GR86 Cup", 2022, "Лёгкое купе для тайм-атака и тренировочных сессий."),
            CreateCar(anna.Id, CarClass.Street, "Mazda", "MX-5 ND", 2020, "Городской трек-дей кар на каждый день."),
            CreateCar(boris.Id, CarClass.SuperSport, "BMW", "M4 Competition", 2021, "Основная машина для быстрых уикендов."),
            CreateCar(boris.Id, CarClass.Sport, "Subaru", "WRX STI", 2018, "Используется для стартов в любительском зачёте."),
            CreateCar(maria.Id, CarClass.Sport, "Honda", "Civic Type R", 2023, "Боевой хэтчбек для спринтов."),
            CreateCar(maria.Id, CarClass.SuperSport, "Nissan", "GT-R", 2019, "Подготовленный трековый сетап под длинные заезды."),
            CreateCar(kirill.Id, CarClass.Street, "Volkswagen", "Golf GTI", 2017, "Первая машина для участия в рейтинговых сессиях."),
            CreateCar(oleg.Id, CarClass.Stock, "Lada", "Granta Sport", 2024, "Стартовый автомобиль без серьёзных доработок."),
            CreateCar(ilya.Id, CarClass.SuperSport, "Porsche", "911 GT3", 2022, "Организаторский референс-кар и участник витринных заездов."),
            CreateCar(sofia.Id, CarClass.Street, "Mini", "John Cooper Works", 2021, "Лёгкий и быстрый автомобиль для технических трасс."),
            CreateCar(vera.Id, CarClass.Sport, "Hyundai", "Elantra N", 2023, "Машина для выездных проверок трасс и стартов в клубном классе."),
        };

        var nurburgring = CreateTrack(admin.Id, "Nürburgring Nordschleife", "Nürburg, Germany", TrackStatus.Official, 10);
        var moscowRaceway = CreateTrack(vera.Id, "Moscow Raceway Club", "Volokolamsk, Russia", TrackStatus.Confirmed, 8);
        var kazanRing = CreateTrack(sofia.Id, "Kazan Ring Canyon", "Kazan, Russia", TrackStatus.Official, 8);
        var smolenskLoop = CreateTrack(anna.Id, "Smolensk Training Loop", "Smolensk, Russia", TrackStatus.Unofficial, 6);
        var nevaSprint = CreateTrack(ilya.Id, "Neva Sprint Layout", "Saint Petersburg, Russia", TrackStatus.Confirmed, 7);

        var trackVotes = new[]
        {
            CreateTrackVote(boris.Id, smolenskLoop.Id, true),
            CreateTrackVote(maria.Id, smolenskLoop.Id, true),
            CreateTrackVote(kirill.Id, smolenskLoop.Id, true),
            CreateTrackVote(oleg.Id, smolenskLoop.Id, false),
        };

        var springCup = CreateTournament(
            ilya.Id,
            moscowRaceway.Id,
            "Spring Time Attack Cup",
            TournamentStatus.RegistrationOpen,
            now.AddDays(18),
            now.AddDays(19),
            CarClass.Sport,
            5,
            6,
            "Открытая весенняя серия с ручным подтверждением заявок и акцентом на плотную квалификацию.");

        var nightGrip = CreateTournament(
            sofia.Id,
            nurburgring.Id,
            "Night Grip Challenge",
            TournamentStatus.Confirmed,
            now.AddDays(9),
            now.AddDays(10),
            CarClass.SuperSport,
            10,
            4,
            "Вечерний турнир на быстрых машинах. Сетка уже собрана, ждём переход в активную фазу.");

        var citySprint = CreateTournament(
            ilya.Id,
            kazanRing.Id,
            "City Sprint Session",
            TournamentStatus.Active,
            now.AddDays(-1),
            now.AddDays(1),
            CarClass.Street,
            3,
            3,
            "Идёт текущая гоночная сессия: заявки закрыты, организатор заносит результаты прямо по ходу дня.");

        var autumnFinal = CreateTournament(
            sofia.Id,
            nurburgring.Id,
            "Autumn Endurance Final",
            TournamentStatus.Finished,
            now.AddDays(-40),
            now.AddDays(-39),
            CarClass.Sport,
            8,
            4,
            "Финальный этап прошлого сезона с уже опубликованными результатами.");

        var iceMeet = CreateTournament(
            vera.Id,
            nevaSprint.Id,
            "Ice Drift Meetup",
            TournamentStatus.Cancelled,
            now.AddDays(-12),
            now.AddDays(-11),
            CarClass.Stock,
            0,
            5,
            "Небольшой зимний выезд, отменённый из-за погодных условий.");

        var tournaments = new[]
        {
            springCup, nightGrip, citySprint, autumnFinal, iceMeet
        };

        var organizers = new[]
        {
            CreateOrganizer(springCup.Id, ilya.Id),
            CreateOrganizer(springCup.Id, vera.Id),
            CreateOrganizer(nightGrip.Id, sofia.Id),
            CreateOrganizer(nightGrip.Id, admin.Id),
            CreateOrganizer(citySprint.Id, ilya.Id),
            CreateOrganizer(autumnFinal.Id, sofia.Id),
            CreateOrganizer(autumnFinal.Id, ilya.Id),
            CreateOrganizer(iceMeet.Id, vera.Id),
        };

        var adminSuperSport = cars[0];
        var annaSport = cars[1];
        var annaStreet = cars[2];
        var borisSuperSport = cars[3];
        var borisSport = cars[4];
        var mariaSport = cars[5];
        var mariaSuperSport = cars[6];
        var kirillStreet = cars[7];
        var olegStock = cars[8];
        var ilyaSuperSport = cars[9];
        var sofiaStreet = cars[10];
        var veraSport = cars[11];

        var applications = new[]
        {
            CreateApplication(anna.Id, springCup.Id, annaSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(vera.Id, springCup.Id, veraSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(boris.Id, springCup.Id, borisSport.Id, TournamentApplicationStatus.Pending),
            CreateApplication(maria.Id, springCup.Id, mariaSport.Id, TournamentApplicationStatus.Rejected),

            CreateApplication(boris.Id, nightGrip.Id, borisSuperSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(maria.Id, nightGrip.Id, mariaSuperSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(ilya.Id, nightGrip.Id, ilyaSuperSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(admin.Id, nightGrip.Id, adminSuperSport.Id, TournamentApplicationStatus.Approved),

            CreateApplication(anna.Id, citySprint.Id, annaStreet.Id, TournamentApplicationStatus.Approved),
            CreateApplication(kirill.Id, citySprint.Id, kirillStreet.Id, TournamentApplicationStatus.Approved),
            CreateApplication(sofia.Id, citySprint.Id, sofiaStreet.Id, TournamentApplicationStatus.Approved),

            CreateApplication(anna.Id, autumnFinal.Id, annaSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(boris.Id, autumnFinal.Id, borisSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(maria.Id, autumnFinal.Id, mariaSport.Id, TournamentApplicationStatus.Approved),
            CreateApplication(vera.Id, autumnFinal.Id, veraSport.Id, TournamentApplicationStatus.Approved),

            CreateApplication(oleg.Id, iceMeet.Id, olegStock.Id, TournamentApplicationStatus.Approved),
            CreateApplication(kirill.Id, iceMeet.Id, kirillStreet.Id, TournamentApplicationStatus.Cancelled),
        };

        var results = new[]
        {
            CreateResult(citySprint.Id, anna.Id, 1, TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(42)),
            CreateResult(autumnFinal.Id, maria.Id, 1, TimeSpan.FromMinutes(7) + TimeSpan.FromSeconds(58)),
            CreateResult(autumnFinal.Id, anna.Id, 2, TimeSpan.FromMinutes(8) + TimeSpan.FromSeconds(2)),
            CreateResult(autumnFinal.Id, vera.Id, 3, TimeSpan.FromMinutes(8) + TimeSpan.FromSeconds(7)),
            CreateResult(autumnFinal.Id, boris.Id, 4, TimeSpan.FromMinutes(8) + TimeSpan.FromSeconds(11)),
        };

        await _context.Users.AddRangeAsync(users);
        await _context.Tracks.AddRangeAsync(nurburgring, moscowRaceway, kazanRing, smolenskLoop, nevaSprint);
        await _context.TrackVotes.AddRangeAsync(trackVotes);
        await _context.Tournaments.AddRangeAsync(tournaments);
        await _context.TournamentOrganizers.AddRangeAsync(organizers);
        await _context.Cars.AddRangeAsync(cars);
        await _context.TournamentApplications.AddRangeAsync(applications);
        await _context.TournamentResults.AddRangeAsync(results);

        await _context.SaveChangesAsync();
    }

    private static User CreateUser(
        string nickname,
        string email,
        string passwordHash,
        int raceCount,
        UserRole roles,
        string bio,
        bool isBlocked = false)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Nickname = nickname,
            Email = email,
            PasswordHash = passwordHash,
            RaceCount = raceCount,
            Roles = roles,
            Bio = bio,
            IsBlocked = isBlocked,
        };
    }

    private static Car CreateCar(Guid ownerId, CarClass carClass, string brand, string model, int year, string description)
    {
        return new Car
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            CarClass = carClass,
            Brand = brand,
            Model = model,
            Year = year,
            Description = description,
        };
    }

    private static Track CreateTrack(Guid creatorId, string name, string location, TrackStatus status, int confirmationThreshold)
    {
        return new Track
        {
            Id = Guid.NewGuid(),
            CreatedById = creatorId,
            Name = name,
            Location = location,
            Status = status,
            ConfirmationThreshold = confirmationThreshold,
        };
    }

    private static TrackVote CreateTrackVote(Guid userId, Guid trackId, bool isPositive)
    {
        return new TrackVote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TrackId = trackId,
            IsPositive = isPositive,
        };
    }

    private static Tournament CreateTournament(
        Guid creatorId,
        Guid trackId,
        string name,
        TournamentStatus status,
        DateTime startDate,
        DateTime endDate,
        CarClass allowedCarClass,
        int requiredRaceCount,
        int requiredParticipants,
        string description)
    {
        return new Tournament
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            TrackId = trackId,
            Name = name,
            Description = description,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            AllowedCarClass = allowedCarClass,
            RequiredRaceCount = requiredRaceCount,
            RequiredParticipants = requiredParticipants,
        };
    }

    private static TournamentOrganizer CreateOrganizer(Guid tournamentId, Guid userId)
    {
        return new TournamentOrganizer
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            UserId = userId,
        };
    }

    private static TournamentApplication CreateApplication(
        Guid userId,
        Guid tournamentId,
        Guid carId,
        TournamentApplicationStatus status)
    {
        return new TournamentApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TournamentId = tournamentId,
            CarId = carId,
            Status = status,
        };
    }

    private static TournamentResult CreateResult(Guid tournamentId, Guid userId, int position, TimeSpan? bestLapTime)
    {
        return new TournamentResult
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            UserId = userId,
            Position = position,
            BestLapTime = bestLapTime,
        };
    }
}
