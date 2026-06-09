using Microsoft.EntityFrameworkCore;

namespace ResidenceEtudiante.Models
{
    /// <summary>
    /// Liste paginée générique qui hérite de List<T> et ajoute des fonctionnalités de pagination.
    /// </summary>
    /// <remarks>
    /// Documentation générée par GitHub Copilot.
    /// Code tiré de : https://learn.microsoft.com/fr-fr/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-10.0&authuser=0#add-paging-to-the-students-index
    /// @author Code du professeur
    /// </remarks>
    /// <typeparam name="T">
    /// Le type d'éléments contenus dans la liste. Le paramètre générique T permet de rendre cette classe 
    /// réutilisable pour n'importe quel type d'objet. 
    /// </typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Obtient l'index de la page actuelle (commence à 1).
        /// </summary>
        public int PageIndex { get; private set; } = 1;

        /// <summary>
        /// Obtient le nombre total de pages disponibles.
        /// </summary>
        public int TotalPages { get; private set; } = 1;

        /// <summary>
        /// Indique s'il existe une page précédente.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Indique s'il existe une page suivante.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Constructeur qui crée une nouvelle instance de PaginatedList.
        /// </summary>
        /// <param name="items">Les éléments de type T à afficher sur la page actuelle.</param>
        /// <param name="count">Le nombre total d'éléments dans la source de données (avant pagination).</param>
        /// <param name="pageIndex">L'index de la page actuelle (commence à 1).</param>
        /// <param name="pageSize">Le nombre d'éléments par page.</param>
        /// <remarks>
        /// Le calcul du nombre total de pages : TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        /// 
        /// - count / (double)pageSize : On convertit pageSize en double pour effectuer une division décimale.
        ///   Sans cette conversion, la division serait entière et tronquerait le résultat.
        ///   Exemple : 25 / 10 = 2 (division entière) vs 25 / 10.0 = 2.5 (division décimale)
        /// 
        /// - Math.Ceiling() : Cette fonction arrondit vers le haut au prochain entier.
        ///   Elle garantit qu'on a assez de pages pour afficher tous les éléments.
        ///   Exemple : Ceiling(2.5) = 3
        /// 
        /// - (int) : Convertit le résultat en entier pour l'assigner à TotalPages (qui est de type int).
        /// </remarks>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        /// <summary>
        /// Crée une instance de PaginatedList de manière synchrone à partir d'une liste.
        /// </summary>
        /// <param name="source">La liste source contenant tous les éléments à paginer.</param>
        /// <param name="pageIndex">L'index de la page à récupérer (commence à 1).</param>
        /// <param name="pageSize">Le nombre d'éléments par page.</param>
        /// <returns>Une PaginatedList contenant uniquement les éléments de la page demandée.</returns>
        /// <remarks>
        /// Cette méthode prend une liste complète et extrait uniquement les éléments nécessaires pour la page demandée.
        /// 
        /// Exemple : Pour la page 2 avec 10 éléments par page, on saute les 10 premiers éléments 
        /// (Skip((2-1)*10) = Skip(10)) et on prend les 10 suivants (Take(10)).
        /// </remarks>
        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
        {
            int count = source.Count;
            List<T> items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Crée une instance de PaginatedList de manière asynchrone.
        /// </summary>
        /// <param name="source">La requête LINQ qui représente la source de données à paginer.</param>
        /// <param name="pageIndex">L'index de la page à récupérer (commence à 1).</param>
        /// <param name="pageSize">Le nombre d'éléments par page.</param>
        /// <returns>Une tâche qui représente l'opération asynchrone et contient la PaginatedList créée.</returns>
        /// <remarks>
        /// Cette méthode effectue deux requêtes à la base de données :
        /// 1. CountAsync() : Compte le nombre total d'éléments dans la source.
        /// 2. Skip().Take().ToListAsync() : Récupère uniquement les éléments de la page demandée.
        /// 
        /// Exemple : Pour la page 2 avec 10 éléments par page, on saute les 10 premiers éléments 
        /// (Skip((2-1)*10) = Skip(10)) et on prend les 10 suivants (Take(10)).
        /// </remarks>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = await source.CountAsync<T>();
            List<T> items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
