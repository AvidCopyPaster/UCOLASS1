using System.Windows;
using System.Windows.Controls;
using MovieLibraryApp.Models;
using MovieLibraryApp.Services;

namespace MovieLibraryApp;

public partial class MainWindow : Window
{
    private MovieLibrary library = new MovieLibrary();

    public MainWindow()
    {
        InitializeComponent();
        LoadSampleData();
        RefreshGrid();
        RefreshQueueGrid();
    }

    private void LoadSampleData()
{
    library.AddMovie(new Movie
    {
        MovieID = "M001",
        Title = "The Matrix",
        Director = "The Wachowskis",
        Genre = "Sci-Fi",
        ReleaseYear = 1999,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M002",
        Title = "Inception",
        Director = "Christopher Nolan",
        Genre = "Sci-Fi",
        ReleaseYear = 2010,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M003",
        Title = "The Godfather",
        Director = "Francis Ford Coppola",
        Genre = "Crime",
        ReleaseYear = 1972,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M004",
        Title = "Avengers: Secret Wars",
        Director = "Marvel Studios",
        Genre = "Action",
        ReleaseYear = 2026,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M005",
        Title = "The Batman Part II",
        Director = "Matt Reeves",
        Genre = "Action",
        ReleaseYear = 2026,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M006",
        Title = "Supergirl: Woman of Tomorrow",
        Director = "Craig Gillespie",
        Genre = "Sci-Fi",
        ReleaseYear = 2026,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M007",
        Title = "Shrek 5",
        Director = "DreamWorks",
        Genre = "Animation",
        ReleaseYear = 2026,
        Availability = "Available"
    });

    library.AddMovie(new Movie
    {
        MovieID = "M008",
        Title = "Toy Story 5",
        Director = "Pixar",
        Genre = "Animation",
        ReleaseYear = 2026,
        Availability = "Available"
    });
}

    private void RefreshGrid()
    {
        MoviesDataGrid.ItemsSource = null;
        MoviesDataGrid.ItemsSource = library.GetAllMovies().ToList();
    }

    private void RefreshQueueGrid()
    {
        QueueDataGrid.ItemsSource = null;
        QueueDataGrid.ItemsSource = library.GetQueueDetails();
    }

    private bool IsExampleText(TextBox textBox)
    {
        return textBox.Foreground == System.Windows.Media.Brushes.Gray;
    }

    private void AddMovie_Click(object sender, RoutedEventArgs e)
    {
        if (IsExampleText(MovieIDTextBox) ||
            IsExampleText(TitleTextBox) ||
            IsExampleText(DirectorTextBox) ||
            IsExampleText(GenreTextBox) ||
            IsExampleText(YearTextBox))
        {
            MessageBox.Show("Please replace the example text with real movie details.");
            return;
        }

        if (!int.TryParse(YearTextBox.Text, out int year))
        {
            MessageBox.Show("Release year must be a number.");
            return;
        }

        Movie movie = new Movie
        {
            MovieID = MovieIDTextBox.Text.Trim(),
            Title = TitleTextBox.Text.Trim(),
            Director = DirectorTextBox.Text.Trim(),
            Genre = GenreTextBox.Text.Trim(),
            ReleaseYear = year,
            Availability = "Available",
            CurrentBorrower = "",
            BorrowerHistory = new List<string>()
        };

        if (string.IsNullOrWhiteSpace(movie.MovieID) || string.IsNullOrWhiteSpace(movie.Title))
        {
            MessageBox.Show("Movie ID and Title are required.");
            return;
        }

        bool added = library.AddMovie(movie);

        if (!added)
        {
            MessageBox.Show("Duplicate Movie ID. Movie was not added.");
            return;
        }

        MessageBox.Show("Movie added successfully.");
        StatusTextBlock.Text = "Movie added successfully.";

        RefreshGrid();
        RefreshQueueGrid();
    }

    private void SearchTitle_Click(object sender, RoutedEventArgs e)
    {
        if (IsExampleText(SearchTextBox))
        {
            MessageBox.Show("Please enter a movie title.");
            return;
        }

        Movie? movie = library.LinearSearchByTitle(SearchTextBox.Text.Trim());

        if (movie == null)
        {
            MessageBox.Show("Movie not found.");
            return;
        }

        MoviesDataGrid.ItemsSource = new List<Movie> { movie };
        StatusTextBlock.Text = "Search by title completed using Linear Search.";
    }

    private void SearchID_Click(object sender, RoutedEventArgs e)
    {
        if (IsExampleText(SearchTextBox))
        {
            MessageBox.Show("Please enter a Movie ID.");
            return;
        }

        Movie? movie = library.BinarySearchByMovieID(SearchTextBox.Text.Trim());

        if (movie == null)
        {
            MessageBox.Show("Movie not found.");
            return;
        }

        MoviesDataGrid.ItemsSource = new List<Movie> { movie };
        StatusTextBlock.Text = "Search by Movie ID completed using Binary Search.";
    }

    private void ShowAll_Click(object sender, RoutedEventArgs e)
    {
        RefreshGrid();
        StatusTextBlock.Text = "Showing all movies.";
    }

    private void SortTitle_Click(object sender, RoutedEventArgs e)
    {
        library.BubbleSortByTitle();
        RefreshGrid();
        RefreshQueueGrid();
        StatusTextBlock.Text = "Movies sorted by title using Bubble Sort.";
    }

    private void SortYear_Click(object sender, RoutedEventArgs e)
    {
        library.MergeSortByReleaseYear();
        RefreshGrid();
        RefreshQueueGrid();
        StatusTextBlock.Text = "Movies sorted by release year using Merge Sort.";
    }

    private void Borrow_Click(object sender, RoutedEventArgs e)
    {
        if (IsExampleText(BorrowMovieTextBox) || IsExampleText(BorrowerTextBox))
        {
            MessageBox.Show("Please enter a Movie ID or Title and borrower name.");
            return;
        }

        string movieInput = BorrowMovieTextBox.Text.Trim();
        string borrower = BorrowerTextBox.Text.Trim();

        string message = library.BorrowMovie(movieInput, borrower);

        MessageBox.Show(message);
        StatusTextBlock.Text = message;

        RefreshGrid();
        RefreshQueueGrid();
    }

    private void Return_Click(object sender, RoutedEventArgs e)
    {
        if (IsExampleText(BorrowMovieTextBox))
        {
            MessageBox.Show("Please enter a Movie ID or Title.");
            return;
        }

        string movieInput = BorrowMovieTextBox.Text.Trim();

        string message = library.ReturnMovie(movieInput);

        MessageBox.Show(message);
        StatusTextBlock.Text = message;

        RefreshGrid();
        RefreshQueueGrid();
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        library.ExportToJson("movies.json");

        MessageBox.Show("Movies exported to movies.json.");
        StatusTextBlock.Text = "Movies exported.";
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        library.ImportFromJson("movies.json");

        RefreshGrid();
        RefreshQueueGrid();

        MessageBox.Show("Movies imported from movies.json.");
        StatusTextBlock.Text = "Movies imported.";
    }

    private void RefreshQueue_Click(object sender, RoutedEventArgs e)
    {
        RefreshQueueGrid();
        StatusTextBlock.Text = "Queue list refreshed.";
    }

    private void ClearExampleText(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.Foreground == System.Windows.Media.Brushes.Gray)
        {
            textBox.Text = "";
            textBox.Foreground = System.Windows.Media.Brushes.Black;
        }
    }

    private void RestoreExampleText(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Foreground = System.Windows.Media.Brushes.Gray;

            if (textBox.Name == "MovieIDTextBox")
                textBox.Text = "Example: M004";
            else if (textBox.Name == "TitleTextBox")
                textBox.Text = "Example: Interstellar";
            else if (textBox.Name == "DirectorTextBox")
                textBox.Text = "Example: Christopher Nolan";
            else if (textBox.Name == "GenreTextBox")
                textBox.Text = "Example: Sci-Fi";
            else if (textBox.Name == "YearTextBox")
                textBox.Text = "Example: 2014";
            else if (textBox.Name == "SearchTextBox")
                textBox.Text = "Example: M001 or Inception";
            else if (textBox.Name == "BorrowMovieTextBox")
                textBox.Text = "Movie ID or Title";
            else if (textBox.Name == "BorrowerTextBox")
                textBox.Text = "Example: John";
        }
    }
}