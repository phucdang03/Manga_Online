using System.ComponentModel.DataAnnotations;

namespace Service.MangaOnline.RequestModels
{
    public class HistoryRequest
    {
        [Required] public string User { get; set; } = null!;
        [Required] public string Hash { get; set; } = null!;
        [Required] public string From { get; set; } = null!;
        [Required] public string To { get; set; } = null!;
        [Required] public string Value { get; set; } = null!; 
    
        
    }
}
