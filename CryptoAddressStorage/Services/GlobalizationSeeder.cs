using CryptoAddressStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Services
{
    public static class GlobalizationSeeder
    {
        public static void RunSeeding(ISiteRepository repo)
        {
            ClearTranslations(repo);

            #region Layout
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Home", Text_En = "Home", Text_Fr = "Acceuil" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Privacy", Text_En = "Privacy", Text_Fr = "Confidentialité"});
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_ManageUsers", Text_En = "Manage Users", Text_Fr = "Gestion des Usagers" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Search", Text_En = "Search", Text_Fr = "Chercher" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Friends", Text_En = "Friends", Text_Fr = "Amis" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Logout", Text_En = "Logout", Text_Fr = "Déconnecter" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Register", Text_En = "Register", Text_Fr = "Créer un compte" });
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Layout_Nav_Login", Text_En = "Login", Text_Fr = "Connexion" });
            #endregion

            #region Homepage
            repo.AddTranslationResource(new TranslationResource() { ResourceKey = "Home_Index_MainHeader", Text_En = "Homepage", Text_Fr = "Page d'acceuil"});

            #endregion

            repo.SaveChanges();
        }

        public static void ClearTranslations(ISiteRepository repo)
        {
            repo.ClearAllTranslations();
        }
    }
}
