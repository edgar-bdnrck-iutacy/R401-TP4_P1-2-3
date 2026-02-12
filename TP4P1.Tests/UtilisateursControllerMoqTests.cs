using Microsoft.AspNetCore.Mvc;
using Moq;
using TP4P1.Controllers;
using TP4P1.Models.EntityFramework;
using TP4P1.Models.Repository;

namespace TP4P1.Tests;

[TestClass]
public class UtilisateursControllerMoqTests
{
    [TestMethod]
    public void PostUtilisateur_ModelValidated_CreationOK_AvecMoq()
    {
        // Arrange
        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        var controller = new UtilisateursController(mockRepo.Object);

        var user = new Utilisateur
        {
            Nom = "POISSON",
            Prenom = "Pascal",
            Mobile = "0600000000",
            Mail = "poisson@gmail.com",
            Pwd = "Toto12345678!",
            Rue = "Chemin de Bellevue",
            CodePostal = "74940",
            Ville = "Annecy-le-Vieux",
            Pays = "France",
            Latitude = null,
            Longitude = null,
            DateCreation = DateTime.Today
        };

        // simule l'assignation de l'ID par la BD lors du AddAsync
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Utilisateur>()))
                .Callback<Utilisateur>(u => u.UtilisateurId = 1)
                .Returns(Task.CompletedTask);

        // Act
        var actionResult = controller.PostUtilisateur(user).Result;

        // Assert
        Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult));
        var created = actionResult.Result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.IsInstanceOfType(created.Value, typeof(Utilisateur));

        var returned = (Utilisateur)created.Value!;
        Assert.AreEqual(1, returned.UtilisateurId);
        Assert.AreEqual(user, returned);
    }

    [TestMethod]
    public void GetUtilisateurById_ExistingIdPassed_ReturnsRightItem_AvecMoq()
    {
        // Arrange
        var user = new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Calida",
            Prenom = "Lilley",
            Mobile = "0653930778",
            Mail = "clilleymd@last.fm",
            Pwd = "Toto12345678!",
            Rue = "Impasse des bergeronnettes",
            CodePostal = "74200",
            Ville = "Allinges",
            Pays = "France",
            Latitude = 46.344795F,
            Longitude = 6.4885845F,
            DateCreation = DateTime.Today
        };

        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.GetUtilisateurById(1).Result;

        // Assert
        Assert.IsNotNull(actionResult.Value);
        Assert.AreEqual(user, actionResult.Value);
    }

    [TestMethod]
    public void GetUtilisateurById_UnknownIdPassed_ReturnsNotFoundResult_AvecMoq()
    {
        // Arrange
        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByIdAsync(0)).ReturnsAsync((ActionResult<Utilisateur>)null!);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.GetUtilisateurById(0).Result;

        // Assert
        Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void GetUtilisateurByEmail_Existing_ReturnsRightItem_AvecMoq()
    {
        // Arrange
        var user = new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Calida",
            Prenom = "Lilley",
            Mobile = "0653930778",
            Mail = "clilleymd@last.fm",
            Pwd = "Toto12345678!",
            Rue = "Impasse des bergeronnettes",
            CodePostal = "74200",
            Ville = "Allinges",
            Pays = "France",
            Latitude = 46.344795F,
            Longitude = 6.4885845F,
            DateCreation = DateTime.Today
        };

        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByStringAsync("clilleymd@last.fm")).ReturnsAsync(user);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.GetUtilisateurByEmail("clilleymd@last.fm").Result;

        // Assert
        Assert.IsNotNull(actionResult.Value);
        Assert.AreEqual(user, actionResult.Value);
    }

    [TestMethod]
    public void GetUtilisateurByEmail_Unknown_ReturnsNotFound_AvecMoq()
    {
        // Arrange
        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByStringAsync("unknown@example.com")).ReturnsAsync((ActionResult<Utilisateur>)null!);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.GetUtilisateurByEmail("unknown@example.com").Result;

        // Assert
        Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void DeleteUtilisateurTest_AvecMoq()
    {
        // Arrange
        var user = new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Calida",
            Prenom = "Lilley",
            Mobile = "0653930778",
            Mail = "clilleymd@last.fm",
            Pwd = "Toto12345678!",
            Rue = "Impasse des bergeronnettes",
            CodePostal = "74200",
            Ville = "Allinges",
            Pays = "France",
            Latitude = 46.344795F,
            Longitude = 6.4885845F,
            DateCreation = DateTime.Today
        };

        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Utilisateur>())).Returns(Task.CompletedTask);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.DeleteUtilisateur(1).Result;

        // Assert
        Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
    }

    [TestMethod]
    public void PutUtilisateur_Success_AvecMoq()
    {
        // Arrange
        var existing = new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Calida",
            Prenom = "Lilley",
            Mail = "clilleymd@last.fm",
            Pwd = "Toto12345678!",
            DateCreation = DateTime.Today
        };

        var updated = new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Calida",
            Prenom = "MODIF",
            Mail = "clilleymd@last.fm",
            Pwd = "Toto12345678!",
            DateCreation = DateTime.Today
        };

        var mockRepo = new Mock<IDataRepository<Utilisateur>>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Utilisateur>(), It.IsAny<Utilisateur>()))
                .Returns(Task.CompletedTask);

        var controller = new UtilisateursController(mockRepo.Object);

        // Act
        var actionResult = controller.PutUtilisateur(1, updated).Result;

        // Assert
        Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
    }
}
