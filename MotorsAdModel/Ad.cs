using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotorsAdModel
{
    /// <summary>
    /// Classe modelo de Ad (Anuncio)
    /// </summary>
    [Table("tb_AnuncioWebmotors")]
    public class Ad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }

        [StringLength(45)]
        [Required]
        public string marca { get; set; }

        [Required]
        [StringLength(45)]
        public string modelo { get; set; }

        [Required]
        [StringLength(45)]
        public string versao { get; set; }

        [Required]
        public int ano { get; set; }

        [Required]
        public int quilometragem { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string observacao { get; set; }

    }
}