using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP4P1.Controllers;
using TP4P1.Models.EntityFramework;

namespace TP4P1.Tests;

[TestClass]
public class UtilisateursControllerTests
{
    private readonly FilmRatingsDBContext _context;
    private readonly UtilisateursController _controller;

    // ⚠️ Même chaîne que ton appsettings.json (adapte si besoin)
    private const string ConnStr =
        "Host=localhost;Port=5432;Database=filmrating;Username=postgres;Password=postgres";

    public UtilisateursControllerTests()
    {
        var options = new DbContextOptionsBuilder<FilmRatingsDBContext>()
            .UseNpgsql(ConnStr)
            .Options;

        _context = new FilmRatingsDBContext(options);
        _controller = new UtilisateursController(_context);
    }

    private Utilisateur EnsureAtLeastOneUser()
    {
        var existing = _context.Utilisateurs.FirstOrDefault();
        if (existing != null) return existing;

        // Si la BD est vide, on crée un user pour que les tests "GET OK" aient un cas réel
        var rnd = new Random();
        var u = new Utilisateur
        {
            Nom = "TEST",
            Prenom = "SEED",
            Mobile = "0600000000",
            Mail = $"seed{rnd.Next(1, 1_000_000_000)}@example.com",
            Pwd = "info",
            Rue = null,
            CodePostal = null,
            Ville = null,
            Pays = "France",
            Latitude = null,
            Longitude = null,
            DateCreation = DateTime.Today
        };

        _context.Utilisateurs.Add(u);
        _context.SaveChanges();
        return u;
    }

    [TestMethod]
    public void GetUtilisateurs_GetAll_OK()
    {
        // Arrange
        var expected = _context.Utilisateurs.ToList();

        // Act
        var actionResult = _controller.GetUtilisateurs().Result;
        var resultList = actionResult.Value?.ToList();

        // Assert
        Assert.IsNotNull(resultList, "La liste retournée est null.");
        Assert.AreEqual(expected.Count, resultList.Count, "Le nombre d'utilisateurs retournés ne correspond pas.");
    }

    [TestMethod]
    public void GetUtilisateurById_Existing_OK()
    {
        // Arrange
        var u = EnsureAtLeastOneUser();

        // Act
        var actionResult = _controller.GetUtilisateurById(u.UtilisateurId).Result;

        // Assert
        Assert.IsNotNull(actionResult.Value, "Utilisateur retourné null.");
        Assert.AreEqual(u.UtilisateurId, actionResult.Value.UtilisateurId);
    }

    [TestMethod]
    public void GetUtilisateurById_NotExisting_NotFound()
    {
        // Arrange
        var notExistingId = int.MaxValue;

        // Act
        var actionResult = _controller.GetUtilisateurById(notExistingId).Result;

        // Assert (quand NotFound, Value est null et Result est NotFoundResult)
        Assert.IsNull(actionResult.Value);
        Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void GetUtilisateurByEmail_Existing_OK()
    {
        // Arrange
        var u = EnsureAtLeastOneUser();

        // Act
        var actionResult = _controller.GetUtilisateurByEmail(u.Mail).Result;

        // Assert
        Assert.IsNotNull(actionResult.Value);
        Assert.AreEqual(u.UtilisateurId, actionResult.Value.UtilisateurId);
    }

    [TestMethod]
    public void GetUtilisateurByEmail_NotExisting_NotFound()
    {
        // Arrange
        var email = $"doesnotexist{Guid.NewGuid():N}@example.com";

        // Act
        var actionResult = _controller.GetUtilisateurByEmail(email).Result;

        // Assert
        Assert.IsNull(actionResult.Value);
        Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void PostUtilisateur_ModelValidated_CreationOK()
    {
        // Arrange (mail unique obligatoire)
        var rnd = new Random();
        var u = new Utilisateur
        {
            Nom = "MACHIN",
            Prenom = "Luc",
            Mobile = "0606070809",
            Mail = $"machin{rnd.Next(1, 1_000_000_000)}@gmail.com",
            Pwd = "info",
            Rue = "Chemin de Bellevue",
            CodePostal = "74940",
            Ville = "Annecy",
            Pays = "France",
            Latitude = null,
            Longitude = null,
            DateCreation = DateTime.Today
        };

        // Act
        var action = _controller.PostUtilisateur(u).Result;

        // Assert: l'utilisateur doit exister en BD
        var uDb = _context.Utilisateurs.FirstOrDefault(x => x.Mail.ToUpper() == u.Mail.ToUpper());
        Assert.IsNotNull(uDb, "Utilisateur non inséré en base.");

        // On récupère l'ID auto
        Assert.IsTrue(uDb.UtilisateurId > 0);
        Assert.AreEqual(u.Mail, uDb.Mail);
    }

    [TestMethod]
    public void DeleteUtilisateur_SuppressionOK()
    {
        // Arrange: créer un user via DbSet comme demandé par le TP
        var rnd = new Random();
        var u = new Utilisateur
        {
            Nom = "DEL",
            Prenom = "ME",
            Mobile = "0600000001",
            Mail = $"del{rnd.Next(1, 1_000_000_000)}@example.com",
            Pwd = "info",
            Pays = "France",
            DateCreation = DateTime.Today
        };

        _context.Utilisateurs.Add(u);
        _context.SaveChanges();

        var id = u.UtilisateurId;

        // Act
        var result = _controller.DeleteUtilisateur(id).Result;

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        var stillThere = _context.Utilisateurs.FirstOrDefault(x => x.UtilisateurId == id);
        Assert.IsNull(stillThere, "Utilisateur non supprimé.");
    }
}
