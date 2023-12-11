using System.ComponentModel.DataAnnotations;

namespace StajyerTakipSistemi.Web.Models
{
    public class SFinal
    {
        public int Id { get; set; }
        public int InternId { get; set; }
        public string RelatedDocuments { get; set; }
        public string GitHubLink { get; set; }
        public string YouTubeLink { get; set; }
        public bool? EvaluationStatus { get; set; } = false;
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; }
    }
}
