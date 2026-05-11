namespace MovieLibraryApp.Models;

public class Movie
{
    public string MovieID { get; set; } = "";
    public string Title { get; set; } = "";
    public string Director { get; set; } = "";
    public string Genre { get; set; } = "";
    public int ReleaseYear { get; set; }
    public string Availability { get; set; } = "Available";
    public string CurrentBorrower { get; set; } = "";

    public List<string> BorrowerHistory { get; set; } = new List<string>();
}
