namespace TP4P1.Models.EntityFramework
{
    public partial class Utilisateur
    {
        public override bool Equals(object? obj)
        {
            if (obj is not Utilisateur other) return false;

            return UtilisateurId == other.UtilisateurId
                && Nom == other.Nom
                && Prenom == other.Prenom
                && Mobile == other.Mobile
                && Mail == other.Mail
                && Pwd == other.Pwd
                && Rue == other.Rue
                && CodePostal == other.CodePostal
                && Ville == other.Ville
                && Pays == other.Pays
                && Latitude == other.Latitude
                && Longitude == other.Longitude
                && DateCreation.Date == other.DateCreation.Date;
        }

        public override int GetHashCode()
        {
            HashCode hc = new HashCode();

            hc.Add(UtilisateurId);
            hc.Add(Nom);
            hc.Add(Prenom);
            hc.Add(Mobile);
            hc.Add(Mail);
            hc.Add(Pwd);
            hc.Add(Rue);
            hc.Add(CodePostal);
            hc.Add(Ville);
            hc.Add(Pays);
            hc.Add(Latitude);
            hc.Add(Longitude);
            hc.Add(DateCreation.Date);

            return hc.ToHashCode();
        }

    }
}
