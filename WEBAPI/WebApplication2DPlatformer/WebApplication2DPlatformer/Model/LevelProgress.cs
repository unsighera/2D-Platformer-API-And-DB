using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2DPlatformer.Model
{
    public class LevelProgress
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey ("Level")]
        public int Level_ID { get; set; }
        public virtual Level Level { get; set; }

        [ForeignKey ("User")]
        public int User_ID { get; set; }
        public virtual User User { get; set; }

        public int LevelStars { get; set; }
        public int LevelScore { get; set; }
    }
}
