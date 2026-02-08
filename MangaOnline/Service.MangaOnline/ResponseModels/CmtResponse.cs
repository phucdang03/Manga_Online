namespace Service.MangaOnline.ResponseModels
{
    public class CmtResponse
    {
        public Guid Id { get; set; }
        public Guid MangaId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public string? date { get; set; }
        public int LikedCount { get; set; }
        public int DislikedCount { get; set; }

        public string? NameUser { get; set; }

        public string? ImgUser { get; set; }

    }
}
