namespace FlappyBlooper
{
    public struct StageInfo
    {
        public string Number { get; set; }
        public bool Unlocked { get; set; }
        public StageData StageData { get; set; }
        public ItemTarget[] Rewards { get; set; }
        public int Progress => StageData.progress;

        public bool IsRewarded(int rewardIndex)
        {
            return StageData.progress > rewardIndex;
        }
    }
}