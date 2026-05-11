using System.Collections;
using System.IO;
using System.Text.Json;
using MovieLibraryApp.Models;

namespace MovieLibraryApp.Services;

public class MovieLibrary
{
    private LinkedList<Movie> movies = new LinkedList<Movie>();
    private Hashtable movieTable = new Hashtable();
    private Dictionary<string, Queue<string>> waitingLists = new Dictionary<string, Queue<string>>();
    private Queue<string> notifications = new Queue<string>();

    public LinkedList<Movie> GetAllMovies()
    {
        return movies;
    }

    public Queue<string> GetNotifications()
    {
        return notifications;
    }

    public bool AddMovie(Movie movie)
    {
        if (string.IsNullOrWhiteSpace(movie.MovieID))
        {
            return false;
        }

        if (movieTable.ContainsKey(movie.MovieID))
        {
            return false;
        }

        if (movie.BorrowerHistory == null)
        {
            movie.BorrowerHistory = new List<string>();
        }

        movies.AddLast(movie);
        movieTable.Add(movie.MovieID, movie);
        waitingLists[movie.MovieID] = new Queue<string>();

        return true;
    }

    public Movie? FindMovieByIdOrTitle(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        Movie? movieById = movieTable[input] as Movie;

        if (movieById != null)
        {
            return movieById;
        }

        foreach (Movie movie in movies)
        {
            if (movie.Title.Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return movie;
            }
        }

        return null;
    }

    public Movie? LinearSearchByTitle(string title)
    {
        foreach (Movie movie in movies)
        {
            if (movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return movie;
            }
        }

        return null;
    }

    public Movie? BinarySearchByMovieID(string movieID)
    {
        List<Movie> sortedMovies = movies.OrderBy(m => m.MovieID).ToList();

        int left = 0;
        int right = sortedMovies.Count - 1;

        while (left <= right)
        {
            int middle = (left + right) / 2;

            int comparison = string.Compare(
                sortedMovies[middle].MovieID,
                movieID,
                StringComparison.OrdinalIgnoreCase
            );

            if (comparison == 0)
            {
                return sortedMovies[middle];
            }
            else if (comparison < 0)
            {
                left = middle + 1;
            }
            else
            {
                right = middle - 1;
            }
        }

        return null;
    }

    public void BubbleSortByTitle()
    {
        List<Movie> list = movies.ToList();

        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                if (string.Compare(list[j].Title, list[j + 1].Title, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Movie temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }

        RebuildCollection(list);
    }

    public void MergeSortByReleaseYear()
    {
        List<Movie> list = movies.ToList();
        list = MergeSort(list);
        RebuildCollection(list);
    }

    private List<Movie> MergeSort(List<Movie> list)
    {
        if (list.Count <= 1)
        {
            return list;
        }

        int middle = list.Count / 2;

        List<Movie> left = list.Take(middle).ToList();
        List<Movie> right = list.Skip(middle).ToList();

        return Merge(MergeSort(left), MergeSort(right));
    }

    private List<Movie> Merge(List<Movie> left, List<Movie> right)
    {
        List<Movie> result = new List<Movie>();

        while (left.Count > 0 && right.Count > 0)
        {
            if (left[0].ReleaseYear <= right[0].ReleaseYear)
            {
                result.Add(left[0]);
                left.RemoveAt(0);
            }
            else
            {
                result.Add(right[0]);
                right.RemoveAt(0);
            }
        }

        result.AddRange(left);
        result.AddRange(right);

        return result;
    }

    public string BorrowMovie(string movieInput, string borrowerName)
    {
        Movie? movie = FindMovieByIdOrTitle(movieInput);

        if (movie == null)
        {
            return "Movie not found.";
        }

        if (movie.Availability == "Available")
        {
            movie.Availability = "Borrowed";
            movie.CurrentBorrower = borrowerName;

            movie.BorrowerHistory.Add($"{borrowerName} borrowed {movie.Title}");

            return $"{borrowerName} borrowed {movie.Title}.";
        }

        waitingLists[movie.MovieID].Enqueue(borrowerName);

        movie.BorrowerHistory.Add($"{borrowerName} joined the waiting queue");

        return $"{borrowerName} added to queue.";
    }

    public string ReturnMovie(string movieInput)
    {
        Movie? movie = FindMovieByIdOrTitle(movieInput);

        if (movie == null)
        {
            return "Movie not found.";
        }

        if (!string.IsNullOrWhiteSpace(movie.CurrentBorrower))
        {
            movie.BorrowerHistory.Add($"{movie.CurrentBorrower} returned {movie.Title}");
        }

        if (waitingLists[movie.MovieID].Count > 0)
        {
            string nextBorrower = waitingLists[movie.MovieID].Dequeue();

            movie.CurrentBorrower = nextBorrower;
            movie.Availability = "Borrowed";

            movie.BorrowerHistory.Add($"{nextBorrower} automatically borrowed {movie.Title}");

            return $"{nextBorrower} automatically borrowed {movie.Title}.";
        }

        movie.CurrentBorrower = "";
        movie.Availability = "Available";

        return $"{movie.Title} returned successfully.";
    }

    public List<object> GetQueueDetails()
    {
        List<object> queueDetails = new List<object>();

        foreach (Movie movie in movies)
        {
            string queueText = "None";

            if (waitingLists.ContainsKey(movie.MovieID))
            {
                queueText = string.Join(", ", waitingLists[movie.MovieID]);

                if (string.IsNullOrWhiteSpace(queueText))
                {
                    queueText = "None";
                }
            }

            string historyText = "None";

            if (movie.BorrowerHistory.Count > 0)
            {
                historyText = string.Join(" | ", movie.BorrowerHistory);
            }

            queueDetails.Add(new
            {
                movie.MovieID,
                movie.Title,
                movie.CurrentBorrower,
                WaitingQueue = queueText,
                BorrowerHistory = historyText
            });
        }

        return queueDetails;
    }

    private void RebuildCollection(List<Movie> list)
    {
        movies.Clear();

        foreach (Movie movie in list)
        {
            movies.AddLast(movie);
        }
    }
}
