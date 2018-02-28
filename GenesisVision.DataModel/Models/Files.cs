using System;

namespace GenesisVision.DataModel.Models
{
    public class Files
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }

        public Guid? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
