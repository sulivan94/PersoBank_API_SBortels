using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace API_PersoBank.Models
{
    public class PersoBankDBInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override async void Seed(ApplicationDbContext context)
        {
            // ****************************  Catégories  *******************************

            Category[] newCategories =
            {
                new Category() { CategoryId = 1, AverageAmount = 38, Label = "Restaurant", Expense = true },
                new Category() { CategoryId = 2, AverageAmount = 20, Label = "Bar", Expense = true },
                new Category() { CategoryId = 3, AverageAmount = 60, Label = "Vetements", Expense = true },
                new Category() { CategoryId = 4, AverageAmount = 39.50M, Label = "Santé", Expense = true },
                new Category() { CategoryId = 5, AverageAmount = 12.85M, Label = "Livres/Revues", Expense = true },
                new Category() { CategoryId = 6, AverageAmount = 115, Label = "Voiture", Expense = true },
                new Category() { CategoryId = 7, AverageAmount = 85, Label = "Famille", Expense = true },
                new Category() { CategoryId = 8, AverageAmount = 16.50M, Label = "Cinéma", Expense = true },
                new Category() { CategoryId = 9, AverageAmount = 55, Label = "Divertissement", Expense = true },
                new Category() { CategoryId = 10, AverageAmount = 41.99M, Label = "Carburant", Expense = true },
                new Category() { CategoryId = 11, AverageAmount = 112.45M, Label = "Maison", Expense = true },
                new Category() { CategoryId = 12, AverageAmount = 285, Label = "Assurance", Expense = true },
                new Category() { CategoryId = 13, AverageAmount = null, Label = "Autre dépense", Expense = true },
                new Category() { CategoryId = 14, AverageAmount = 1500, Label = "Salaire", Expense = false },
                new Category() { CategoryId = 15, AverageAmount = 230, Label = "Revenu financier", Expense = false },
                new Category() { CategoryId = 16, AverageAmount = 45, Label = "Gains de jeu", Expense = false },
                new Category() { CategoryId = 17, AverageAmount = 1228.65M, Label = "Pension", Expense = false },
                new Category() { CategoryId = 18, AverageAmount = null, Label = "Petits boulots", Expense = false },
                new Category() { CategoryId = 19, AverageAmount = 550.89M, Label = "Loyer", Expense = false },
                new Category() { CategoryId = 20, AverageAmount = 185, Label = "Epargne", Expense = false },
                new Category() { CategoryId = 21, AverageAmount = null, Label = "Autre revenu", Expense = false },
            };

            context.Categories.AddRange(newCategories);
            context.SaveChanges();

            // ****************************  Lieux  *******************************

            Place[] newPlaces =
            {
                new Place() { PlaceId = 1, AverageAmount = 15, Street = "Rue de la gare", StreetNumber = 16, PostalCode = 5100, City = "Jambes",
                    Name = "Acinapolis", CategoryId = 6 },
                new Place() { PlaceId = 2, AverageAmount = 14.50M, Street = "Grand Rue", StreetNumber = 141, PostalCode = 6000, City = "Charleroi",
                    Name = "Pathé", CategoryId = 6 },
                new Place() { PlaceId = 3, AverageAmount = 9.20M, Street = "Rue Godefroid", StreetNumber = 64, PostalCode = 5000, City = "Namur",
                    Name = "Green Fairy", CategoryId = 2 },
                new Place() { PlaceId = 4, AverageAmount = 8.60M, Street = "Rue Lelievre", StreetNumber = 26, PostalCode = 5000, City = "Namur",
                    Name = "Le monde à l'envers", CategoryId = 2 }
            };

            context.Places.AddRange(newPlaces);
            context.SaveChanges();

            // ****************************  Rôles  *******************************

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await roleManager.CreateAsync(new IdentityRole { Name = "User" });

            // ****************************  Utilisateur (admin)  *******************************

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // Création de l'administrateur à l'initialisation de la DB, par facilité
            var user = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                BirthDate = new DateTime(1994, 9, 3),
                FirstName = "Merlin",
                LastName = "Enchanteur",
                InscriptionDate = DateTime.Now,
                Sex = true,
            };

            await manager.CreateAsync(user, "Admin_password1");

            var admin = await manager.FindByNameAsync("admin");
            await manager.AddToRoleAsync(admin.Id, "Admin");
        }
    }
}