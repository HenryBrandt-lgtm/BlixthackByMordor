namespace BlixthackByMordor.Models
{
    public class UserFavoriteModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public int AnswerId { get; set; }
        public AnswerModel Answer { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
