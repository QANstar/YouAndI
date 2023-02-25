using YouAndI_Entity;

namespace YouAndI_API.Utils
{
    public static class RecommendationWeight
    {
        private const int RandomWeight = 2;
        private const float TagWeight = 15;
        private const float JoinWeight = 18;

        public static float RecommendationAlgorithm(List<ActivityTag> activtiyTags, List<UserTag> userTags)
        {

            Random rd = new Random();
            float weight = rd.Next(0, 10) * RandomWeight;
            int haveUserTagNum = activtiyTags.Count(x => userTags.FirstOrDefault(y => y.tagId == x.tagId) != null);
            weight = weight + haveUserTagNum * TagWeight;
            return weight;
        }
    }
}
